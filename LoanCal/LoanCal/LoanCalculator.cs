using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace LoanCal
{
    public class LoanCalculator
    {
        private const decimal Months = 12;
        private const int HouseMin = 100000;
        private const int CarMin = 10000;

        private const decimal defaultInterest = 4.5M;

        private const decimal defaultTermsHouse = 30M;
        private const decimal defaultTermsCar = 5M;


        public LoanCalculator(decimal v)
        {
            PurchasePrice = (decimal)v;
        }

        public LoanCalculator(decimal v, ILambdaLogger log) : this(v)
        {
            this.log = log;
        }

        public enum LoanTypes
        {
            None,Home,Car,Small
        }
        public LoanTypes LoanType;
        private ILambdaLogger log;

        public decimal PurchasePrice { get; set; }
        public decimal DownPayment { get; set; } = 0;
        public decimal LoanAmount { get { return PurchasePrice - DownPayment; } }
        public decimal InterestRate { get; set; } = defaultInterest;
        public decimal LoanTermYears { get; set; }

        public decimal CalculatePayment()
        {


            SetLoanType();
            SetLoanTerms();
            SetInterestRate();

            decimal payment = 0;

            if (LoanTermYears > 0)
            {
                if (InterestRate != 0)
                {
                    decimal rate = ((InterestRate / Months) / 100);
                    decimal factor = (rate + (rate / (decimal)(Math.Pow((double)rate + 1, (double)LoanTermYears * 12) - 1)));
                    payment = (LoanAmount * factor);
                }
                else payment = (LoanAmount / (decimal)LoanTermYears * 12);
            }
            return Math.Round(payment, 2);
        }

        private void SetInterestRate()
        {
            if (InterestRate == 0)
                InterestRate = defaultInterest;
        }

        private void SetLoanTerms()
        {
            logForCalculator($"SetLoanTerms() Currently LoanType={LoanType}, LoanTermYears={LoanTermYears}");
            if (this.LoanTermYears == 0)
            {
                if (LoanType == LoanTypes.Home)
                    LoanTermYears = defaultTermsHouse;
                else
                    LoanTermYears = defaultTermsCar;
            }
            logForCalculator($"SetLoanTerms() Now LoanTermYears={LoanTermYears}");
        }

        public void SetLoanType()
        {
            logForCalculator($"SetLoanType() Currently LoanType={LoanType}");
            if (this.LoanType == LoanTypes.None)
            {
                if (PurchasePrice > HouseMin)
                    LoanType = LoanTypes.Home;
                else 
                    LoanType = LoanTypes.Car;

            }
            logForCalculator($"SetLoanType() Now LoanType={LoanType}");
        }

        private void logForCalculator(string message)
        {
            if (log == null)
                Console.WriteLine(message);
            else
                log.LogLine(message);
        }

    }
}
