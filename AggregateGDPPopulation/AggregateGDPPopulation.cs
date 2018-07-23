using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace AggregateGDPPopulation
{
    public class Program
    {
        public static void AggregateFunction()
        {
            string[] readdata = File.ReadAllLines(@"../../../../AggregateGDPPopulation/data/datafile.csv", Encoding.UTF8);
            JObject readCCMap = JObject.Parse(File.ReadAllText(@"../../../../AggregateGDPPopulation/data/cc-mapping.json", Encoding.UTF8));
            Dictionary<string, string> countryContinetMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(readCCMap.ToString());
            Dictionary<string, OutputObject> output = new Dictionary<string, OutputObject>();
            //Console.WriteLine(IndexOfCountry);
            string[] header = readdata[0].Replace('"', ' ').Trim().Split(',');
            int CI = Array.IndexOf(header, "Country Name ");
            int PI = Array.IndexOf(header, " Population (Millions) 2012 ");
            int GDPI = Array.IndexOf(header, " GDP Billions (USD) 2012 ");
            //Console.WriteLine(header[4]);
            //Console.WriteLine(GDPI);
            readdata = readdata.Skip(1).ToArray();

            foreach (string x in readdata)
            {
                string[] rowreaddata = x.Replace('"', ' ').Split(',');
                string Country = rowreaddata[CI];
                string Population = rowreaddata[PI];
                string GDP = rowreaddata[GDPI];
                if (Country.Trim() != "European Union")
                {
                    if (!output.ContainsKey(countryContinetMap[Country.Trim()]))
                    {
                        output.Add(countryContinetMap[Country.Trim()], new OutputObject() { GDP_2012 = float.Parse(GDP), POPULATION_2012 = float.Parse(Population) });
                    }
                    else
                    {
                        output[countryContinetMap[Country.Trim()]].GDP_2012 += float.Parse(GDP);
                        output[countryContinetMap[Country.Trim()]].POPULATION_2012 += float.Parse(Population);
                    }
                }
            }
            var outputJsonString = JsonConvert.SerializeObject(output);
            //Console.WriteLine(outputJsonString);
            //JObject jsonObject = JObject.Parse(outputJsonString);
            File.WriteAllText(@"../../../../AggregateGDPPopulation/output/output.json", outputJsonString);
            //Console.ReadLine();
        }
    }

    public class OutputObject
    {
        public float POPULATION_2012 { get; set; }
        public float GDP_2012 { get; set; }
    }
}