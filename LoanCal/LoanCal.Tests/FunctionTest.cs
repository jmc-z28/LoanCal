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
    public class FunctionTest
    {

        LoanRequest sRequest;
        private const string ExamplesPath = @"Examples\";

        [Fact]
        public void Can_read_slot_example()
        {
            const string example = "GetUtterance.json";
            var json = File.ReadAllText(Path.Combine(ExamplesPath, example));
            var convertedObj = JsonConvert.DeserializeObject<SkillRequest>(json);

            var request = Assert.IsAssignableFrom<Alexa.NET.Request.Type.IntentRequest>(convertedObj.Request);
            var slot = request.Intent.Slots["Color"];
            Assert.Equal("blue", slot.Value);
        }



    }
}
