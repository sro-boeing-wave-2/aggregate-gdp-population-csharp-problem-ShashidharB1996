using Xunit;
using System.IO;
using Newtonsoft.Json.Linq;
using System;

namespace AggregateGDPPopulation.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async void Test1()
        {
            //await Program.Main();
            //var actual = await Program.ReadAsync(@"../../../../AggregateGDPPopulation/output/output.json");
            //var expected = await Program.ReadAsync(@"../../../expected-output.json");
            //JObject actualJson = JObject.Parse(actual);
            //JObject expectedJson = JObject.Parse(expected);
            //Assert.Equal(actualJson, expectedJson);

            AggregateCalculationsClass gdp = new AggregateCalculationsClass();
            JObject actual = await gdp.AggregateCalculations();
            string ExpectedOutput = await FileOperations.ReadfileAsync("../../../expected-output.json");
            JObject expected = JSONOperations.DeSerializeString(ExpectedOutput);
            Assert.Equal(expected, actual);
        }

    }
}