using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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


        public LoanCalculator(int v)
        {
            PurchasePrice = (decimal)v;
            if (v > HouseMin)
            {
                LoanType = LoanTypes.Home;
                LoanTermYears = defaultTermsHouse;
            }
            else if (v <= HouseMin && v > CarMin)
                LoanType = LoanTypes.Car;
            else
                LoanType = LoanTypes.Small;
        }

        public enum LoanTypes
        {
            Home,Car,Small
        }
        public LoanTypes LoanType;
        public decimal PurchasePrice { get; set; }
        public decimal DownPayment { get; set; } = 0;
        public decimal LoanAmount { get { return PurchasePrice - DownPayment; } }
        public decimal InterestRate { get; set; } = defaultInterest;
        public decimal LoanTermYears { get; set; } = defaultTermsCar;

        public decimal CalculatePayment()
        {

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
    }
}
