using Xunit;
using System.IO;
using Newtonsoft.Json.Linq;

namespace AggregateGDPPopulation.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Program.AggregateFunction();
            var actual = File.ReadAllText(@"../../../../AggregateGDPPopulation/output/output.json");
            var expected = File.ReadAllText(@"../../../expected-output.json");
            JObject actualJson = JObject.Parse(actual);
            JObject expectedJson = JObject.Parse(expected);
            Assert.Equal(actualJson, expectedJson);
        }
    }
}