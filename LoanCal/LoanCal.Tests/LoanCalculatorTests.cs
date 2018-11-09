using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using LoanCal;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Newtonsoft.Json;
using System.IO;

namespace LoanCal.Tests
{
    public class LoanCalculatorTests
    {

        const int amntHouse = 200000;
        const int amntCar = 10000;

        const decimal pmtHouseAt45For30 = 1013.37M;
        const decimal pmtcarAt45For5 = 186.43M;
        const decimal pmtCarAt45For30 = 50.67M;
        const decimal pmtHouseAt45For5 = 3728.60M;


        #region defaults tests
        [Fact]
        public void LoanCalculatorLoanOnlyLarge()
        {
            LoanCalculator lc = new LoanCalculator(amntHouse);
            
            Assert.Equal(pmtHouseAt45For30, lc.CalculatePayment());
            Assert.Equal(LoanCalculator.LoanTypes.Home, lc.LoanType);
        }

        [Fact]
        public void LoanCalculatorLoanOnlySmall()
        {
            LoanCalculator lc = new LoanCalculator(amntCar);

            Assert.Equal(pmtcarAt45For5, lc.CalculatePayment());
            Assert.Equal(LoanCalculator.LoanTypes.Car, lc.LoanType);
        }


        [Fact]
        public void LoanCalculatorSmallThenSetTypeToSmall()
        {
            LoanCalculator lc = new LoanCalculator(amntCar);
            lc.LoanType = LoanCalculator.LoanTypes.Small;

            Assert.Equal(pmtcarAt45For5, lc.CalculatePayment());
            Assert.Equal(LoanCalculator.LoanTypes.Small, lc.LoanType);
        }

        [Fact]
        public void LoanCalculatorSmallThenSetTypeToLarge()
        {
            LoanCalculator lc = new LoanCalculator(amntCar);
            lc.LoanType = LoanCalculator.LoanTypes.Home;

            Assert.Equal(pmtCarAt45For30, lc.CalculatePayment());
            Assert.Equal(LoanCalculator.LoanTypes.Home, lc.LoanType);
        }

        [Fact]
        public void LoanCalculatorLargeThenSetTypeToSmall()
        {
            LoanCalculator lc = new LoanCalculator(amntHouse);
            lc.LoanType = LoanCalculator.LoanTypes.Small;

            Assert.Equal(pmtHouseAt45For5, lc.CalculatePayment());
            Assert.Equal(LoanCalculator.LoanTypes.Small, lc.LoanType);
        }

        [Fact]
        public void LoanCalculatorLargeThenSetTypeToLarge()
        {
            LoanCalculator lc = new LoanCalculator(amntHouse);
            lc.LoanType = LoanCalculator.LoanTypes.Home;

            Assert.Equal(pmtHouseAt45For30, lc.CalculatePayment());
            Assert.Equal(LoanCalculator.LoanTypes.Home, lc.LoanType);
        }

        #endregion


        #region Set All Tests

        //lr.InterestRate = getSlot(intent, "InterestRate");
        //lr.LoanTermYears = getSlot(intent, "LoanTermsYears");
        //lr.DownPayment = getSlot(intent, "DownPayment");
        private void SetAllToZero(LoanCalculator lc)
        {
            lc.InterestRate = 0;
            lc.LoanTermYears = 0;
            lc.DownPayment = 0;
        }

        [Fact]
        public void SetAllLoanOnlyLarge()
        {
            LoanCalculator lc = new LoanCalculator(amntHouse);
            SetAllToZero(lc);

            Assert.Equal(pmtHouseAt45For30, lc.CalculatePayment());
            Assert.Equal(LoanCalculator.LoanTypes.Home, lc.LoanType);
        }

        [Fact]
        public void SetAllLoanOnlySmall()
        {
            LoanCalculator lc = new LoanCalculator(amntCar);
            SetAllToZero(lc);

            Assert.Equal(pmtcarAt45For5, lc.CalculatePayment());
            Assert.Equal(LoanCalculator.LoanTypes.Car, lc.LoanType);
        }


        [Fact]
        public void SetAllSmallThenSetTypeToSmall()
        {
            LoanCalculator lc = new LoanCalculator(amntCar);
            lc.LoanType = LoanCalculator.LoanTypes.Small;
            SetAllToZero(lc);

            Assert.Equal(pmtcarAt45For5, lc.CalculatePayment());
            Assert.Equal(LoanCalculator.LoanTypes.Small, lc.LoanType);
        }

        [Fact]
        public void SetAllSmallThenSetTypeToLarge()
        {
            LoanCalculator lc = new LoanCalculator(amntCar);
            lc.LoanType = LoanCalculator.LoanTypes.Home;
            SetAllToZero(lc);

            Assert.Equal(pmtCarAt45For30, lc.CalculatePayment());
            Assert.Equal(LoanCalculator.LoanTypes.Home, lc.LoanType);
        }

        [Fact]
        public void SetAllLargeThenSetTypeToSmall()
        {
            LoanCalculator lc = new LoanCalculator(amntHouse);
            lc.LoanType = LoanCalculator.LoanTypes.Small;
            SetAllToZero(lc);

            Assert.Equal(pmtHouseAt45For5, lc.CalculatePayment());
            Assert.Equal(LoanCalculator.LoanTypes.Small, lc.LoanType);
        }

        [Fact]
        public void SetAllLargeThenSetTypeToLarge()
        {
            LoanCalculator lc = new LoanCalculator(amntHouse);
            lc.LoanType = LoanCalculator.LoanTypes.Home;
            SetAllToZero(lc);

            Assert.Equal(pmtHouseAt45For30, lc.CalculatePayment());
            Assert.Equal(LoanCalculator.LoanTypes.Home, lc.LoanType);
        }
        #endregion



    }
}
