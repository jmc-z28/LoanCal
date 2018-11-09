using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Alexa.NET.Request;
using Newtonsoft.Json;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Newtonsoft.Json.Serialization;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace LoanCal
{
    public class Function
    {
        private ILambdaLogger log;
        private LoanCalculator lr;
        private Session _session;
        private decimal _interestRate = 0M;

        public List<MessageResource> GetResources()
        {
            List<MessageResource> resources = new List<MessageResource>();
            MessageResource enUSResource = new MessageResource("en-US");
            enUSResource.SkillName = "Loan Calcuator";
            enUSResource.WelcomeMessage = "What loan amount are you intersted in payments for";
            enUSResource.HelpMessage = "You can say, What would my monthly house payment be for a 250000 dollar loan at 4.5 percent interest";
            enUSResource.HelpReprompt = "How much of a loan are you interested in";
            enUSResource.StopMessage = "Goodbye!";

            resources.Add(enUSResource);
            return resources;
        }

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            SkillResponse response = new SkillResponse();
            response.Response = new ResponseBody();
            response.Response.ShouldEndSession = false;
            IOutputSpeech innerResponse = null;
            log = context.Logger;
            log.LogLine($"Skill Request Object:");
            log.LogLine(JsonConvert.SerializeObject(input));

            _session = input.Session;

            var allResources = GetResources();
            var resource = allResources.FirstOrDefault();

            if (input.GetRequestType() == typeof(LaunchRequest))
            {
                log.LogLine($"Default LaunchRequest made: 'Alexa, open Loan Calculator");
                innerResponse = new PlainTextOutputSpeech();
                (innerResponse as PlainTextOutputSpeech).Text = resource.WelcomeMessage;

            }
            else if (input.GetRequestType() == typeof(IntentRequest))
            {
                var intentRequest = (IntentRequest)input.Request;


                switch (intentRequest.Intent.Name)
                {
                    case "AMAZON.CancelIntent":
                        log.LogLine($"AMAZON.CancelIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.StopMessage;
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.StopIntent":
                        log.LogLine($"AMAZON.StopIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.StopMessage;
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.HelpIntent":
                        log.LogLine($"AMAZON.HelpIntent: send HelpMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.HelpMessage;
                        break;
                    case "GetInterestRate":
                        log.LogLine($"GetInterestRate sent:");
                        innerResponse = new PlainTextOutputSpeech();
                        getInterestRate(resource, intentRequest.Intent);
                        (innerResponse as PlainTextOutputSpeech).Text = getLoanPayments(resource, intentRequest.Intent);
                        break;
                    case "LoanIntent":
                        log.LogLine($"LoanIntent sent:");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = getLoanPayments(resource, intentRequest.Intent);
                        break;
                    default:
                        log.LogLine($"Unknown intent: " + intentRequest.Intent.Name);
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.HelpReprompt;
                        break;
                }
            }
            log.LogLine("Building Response");

            response.Response.OutputSpeech = innerResponse;
            if (lr != null)
            {
                response.SessionAttributes = new Dictionary<string, object>();
                response.SessionAttributes.Add("LoanAmount", lr.PurchasePrice.ToString());
                response.SessionAttributes.Add("InterestRate", lr.InterestRate.ToString());
                response.SessionAttributes.Add("LoanTermsYears", lr.LoanTermYears.ToString());
                response.SessionAttributes.Add("DownPayment", lr.DownPayment.ToString());
            }
            response.Version = "1.0";
            log.LogLine($"Skill Response Object...");
            log.LogLine(JsonConvert.SerializeObject(response));
            return response;
        }

        private void getInterestRate(MessageResource resource, Intent intent)
        {
            var interestRate = getSlot(intent, "InterestRate");
            var numerator = getSlot(intent, "Numerator");
            var denominator = calculateDenominator(getSlot(intent, "Denominator"));
            var fractional = getFractionalSlot(intent, "Fractional");

            log.LogLine($"Got values of interestRate:{interestRate}, numerator:{numerator}, denominator:{denominator}, fractional:{fractional}, _interestRate:{_interestRate}");

            if ((numerator + denominator + fractional) > 0)
            {
                log.LogLine($"Setting _interest rate based on num, den, fractional");
                _interestRate = numerator + denominator + fractional;
            }
            else
            {
                log.LogLine($"Setting _interest rate interestRate in Session");
                _interestRate = interestRate;
            }
        }

        public decimal calculateDenominator(decimal number)
        {
            log.LogLine($"calculateDenominator .{number.ToString()}");
            return decimal.Parse($".{number.ToString()}");
        }

        private string getLoanPayments(MessageResource resource, Intent intent)
        {
            try
            {
                log.LogLine($"getLoanPayments()");
                lr = new LoanCalculator(getSlot(intent, "LoanAmount"));

                lr.LoanType = getLoanTypeSlot(intent, "LoanType");

                lr.InterestRate = _interestRate;
                lr.LoanTermYears = getSlot(intent, "LoanTermsYears");
                lr.DownPayment = getSlot(intent, "DownPayment");


                log.LogLine("getting ready to calculate");
                log.LogLine($"LoanAmount:{lr.LoanAmount}");
                log.LogLine($"LoanAmount:{lr.CalculatePayment()}");
                log.LogLine($"LoanAmount:{lr.InterestRate}");
                log.LogLine($"LoanAmount:{lr.LoanTermYears}");
                log.LogLine($"LoanAmount:{lr.DownPayment}");

                if (lr.InterestRate > 100 || lr.InterestRate <= 0)
                    return $"An interest rate of {lr.InterestRate} is not valid, please provide an rate greater than zero and less than one hundred percent";
                if (lr.DownPayment < 0 || lr.PurchasePrice < 0 || lr.LoanTermYears < 0)
                    return $"Loan amount, down payment, and number of years for loan can not be negative";
                if (lr.DownPayment > lr.PurchasePrice)
                    return $"Your down payment must be smaller than your loan amount";
                if (lr.LoanAmount != 0)
                    return $"{lr.CalculatePayment()} dollars per month on a {lr.LoanAmount} dollar loan at {lr.InterestRate}, for {lr.LoanTermYears} years, with {lr.DownPayment} dollars down.  You can change any details, or you can exit. ";
                else
                {
                    log.LogLine($"LoanAmount missing value");
                    return "Please let me know the loan amount ";
                }
            }
            catch (Exception ex)
            {
                log.LogLine($"Error: {ex.Message}");
                return "Sorry I didn't understand that amount";
            }
        }



        private decimal getSlot(Intent intent, string slotValue)
        {
            decimal value = 0;
            log.LogLine($"Looking for {slotValue}");
            if (SlotInIntent(intent, slotValue))
            {
                log.LogLine($"Looking in Slots for {slotValue}");
                log.LogLine($"Slot value of {slotValue}: {intent.Slots[slotValue].Value}");
                value = decimal.Parse(intent.Slots[slotValue].Value);
                log.LogLine($"from Slot: {slotValue}: {value}");
            }
            else if (_session.Attributes != null && _session.Attributes.Count != 0)
            {
                log.LogLine($"Looking in Sesssion for {slotValue}");
                if (_session.Attributes.ContainsKey(slotValue))
                {
                    log.LogLine($"Sesssion value of {slotValue}: {_session.Attributes[slotValue].ToString()}");
                    value = decimal.Parse(_session.Attributes[slotValue].ToString());
                    log.LogLine($"from Session: {slotValue}: {value}");
                }
                else
                    log.LogLine($"{slotValue} not in Session");
            }
            return value;
        }

        private bool SlotInIntent(Intent intent, string slotValue)
        {
            log.LogLine($"SlotInIntent looking for {slotValue}");
            if (intent.Slots.ContainsKey(slotValue) && intent.Slots[slotValue].Value != null && intent.Slots[slotValue].Value != "?")
            {
                log.LogLine($"SlotInIntent has a value {slotValue}");
                return true;
            }
            else
            {
                log.LogLine($"Slot {slotValue} has no value");
                return false;
            }
        }

        private decimal getFractionalSlot(Intent intent, string slotValue)
        {
            decimal value = 0;
            log.LogLine($"Looking for {slotValue}");
            if (SlotInIntent(intent, slotValue))
            {
                var fraction = intent.Slots[slotValue].Value;
                log.LogLine($"from Slot: {slotValue}: {fraction}");

                switch (fraction)
                {
                    case "one eighth":
                    case "an eighth":
                    case "an 8th":
                        return .125M;
                    case "one quarter":
                    case "a quarter":
                    case "a 4th":
                        return .25M;
                    case "three eighths":
                    case "3 8ths":
                    case "three 8ths":
                    case "3 eighths":
                    case "3?8":
                        return .375M;
                    case "one half":
                    case "a half":
                        return .5M;
                    case "three quarters":
                    case "three fourths":
                    case "3 4ths":
                    case "3 fourths":
                    case "three 4ths":
                    case "3 quarters":
                        return .75M;
                    case "seven eighths":
                    case "7 8ths":
                    case "seven 8ths":
                    case "7 eighths":
                    case "7?8":
                        return .875M;
                    default:
                        return 0.0M;
                }
            }

            return value;
        }

        private LoanCalculator.LoanTypes getLoanTypeSlot(Intent intent, string slotValue)
        {
            var value = LoanCalculator.LoanTypes.None;
            log.LogLine($"Looking for {slotValue}");
            if (SlotInIntent(intent, slotValue))
            {
                if (intent.Slots[slotValue].Value == "house")
                {
                    value = LoanCalculator.LoanTypes.Home;
                    log.LogLine($"{slotValue}: house");
                }
                else if (intent.Slots[slotValue].Value == "car")
                {
                    value = LoanCalculator.LoanTypes.Car;
                    log.LogLine($"{slotValue}: car");
                }
                else
                {
                    value = LoanCalculator.LoanTypes.None;
                    log.LogLine($"{slotValue}: None");
                }
            }
            return value;
        }
    }








    public class MessageResource
    {
        public MessageResource(string language)
        {
            this.Language = language;
        }

        public string Language { get; set; }
        public string SkillName { get; set; }
        public string WelcomeMessage { get; set; }
        public string HelpMessage { get; set; }
        public string HelpReprompt { get; set; }
        public string StopMessage { get; set; }
    }

}