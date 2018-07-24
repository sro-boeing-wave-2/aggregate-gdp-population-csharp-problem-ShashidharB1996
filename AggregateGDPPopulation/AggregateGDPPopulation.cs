using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AggregateGDPPopulation
{
    public class Program
    {
        public static async Task<string> ReadAsync(string filepath)
        {
            string data;
            using (StreamReader srRead = new StreamReader(filepath))
            {
                data = await srRead.ReadToEndAsync();
            }
            return data;
        }
        public static async void WriteAsync(string outputpath, string result)
        {
            using (StreamWriter srWrite = new StreamWriter(outputpath))
            {
                await srWrite.WriteAsync(result);
            }
        }
        public static async Task Main()
        {
            string DataCsvPath = @"../../../../AggregateGDPPopulation/data/datafile.csv";
            string MapPath = @"../../../../AggregateGDPPopulation/data/cc-mapping.json";
            

            Task<string> DataCsvtask = ReadAsync(DataCsvPath);
            Task<string> Maptask = ReadAsync(MapPath);
            string data = await DataCsvtask;
            string CCMap = await Maptask;
            
            Dictionary<string, string> CountryContinentMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(CCMap);   
            string[] dataarray = data.Replace("\"", "").Split('\n');            
            string[] header = dataarray[0].Split(',');
            int CI = Array.IndexOf(header, "Country Name");
            int PI = Array.IndexOf(header, "Population (Millions) 2012");
            int GDPI = Array.IndexOf(header, "GDP Billions (USD) 2012");

            Dictionary<string, OutputObject> OutputDict = new Dictionary<string, OutputObject>();
            for (int i = 1; i < dataarray.Length; i++)
            {
                float GDP = float.Parse(dataarray[i].Split(',')[GDPI]);
                float Population = float.Parse(dataarray[i].Split(',')[PI]);
                string Country = dataarray[i].Split(',')[CI];
                if (Country == "European Union")
                {
                    break;
                }

                if (!OutputDict.ContainsKey(CountryContinentMap[Country]))
                {
                    OutputDict.Add(CountryContinentMap[Country], new OutputObject() { GDP_2012 = GDP, POPULATION_2012 = Population });
                }
                else
                {
                    OutputDict[CountryContinentMap[Country]].GDP_2012 += GDP;
                    OutputDict[CountryContinentMap[Country]].POPULATION_2012 += Population;
                }
           }
            string OutputPath = @"../../../../AggregateGDPPopulation/output/output.json";
            string output = JsonConvert.SerializeObject(OutputDict, Formatting.Indented);
            WriteAsync(OutputPath, output);
        }
    }

    public class OutputObject
    {
        public float POPULATION_2012 { get; set; }
        public float GDP_2012 { get; set; }
    }
}