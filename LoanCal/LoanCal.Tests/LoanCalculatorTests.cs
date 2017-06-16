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

        [Fact]
        public void LoanCalculatorLoanOnlyLarge()
        {
            LoanCalculator lc = new LoanCalculator(200000);
            
            Assert.Equal((decimal)1013.37, lc.CalculatePayment());
        }

        [Fact]
        public void LoanCalculatorLoanOnlySmall()
        {
            LoanCalculator lc = new LoanCalculator(10000);

            Assert.Equal((decimal)186.43, lc.CalculatePayment());
        }

    }
}
