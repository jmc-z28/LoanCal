using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Newtonsoft.Json;
using Alexa.NET.Request.Type;

namespace LoanCal
{
    public class LoanRequest: Alexa.NET.Request.Type.IntentRequest
    {

        [JsonProperty("Intent")]
        public LoanIntent Intent { get; set; }


    }

    public class LoanIntent : Alexa.NET.Request.Intent
    {


    }
}
