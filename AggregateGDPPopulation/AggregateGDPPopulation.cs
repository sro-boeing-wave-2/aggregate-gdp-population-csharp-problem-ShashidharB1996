using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AggregateGDPPopulation
{
    public static class FileOperations
    {
        public static async Task<string> ReadfileAsync(string filepath)
        {
            string data;
            using (StreamReader fileRead = new StreamReader(filepath))
            {
                data = await fileRead.ReadToEndAsync();
            }
            return data;
        }

        public static async void WriteFileAsync(string outputpath, string result)
        {
            using (StreamWriter fileWrite = new StreamWriter(outputpath))
            {
                await fileWrite.WriteAsync(result);
            }
        }
    }

    public class GDPPopulationClass
    {
        public float POPULATION_2012 { get; set; }
        public float GDP_2012 { get; set; }
    }

    public class AggregateGDPPopulationClass
    {
        public Dictionary<string, GDPPopulationClass> AggGDPPopulation;

        public AggregateGDPPopulationClass()
        {
            AggGDPPopulation = new Dictionary<string, GDPPopulationClass>();
        }

        public void AddOrUpdateAggGDPPopulation(string Continent, float GDP, float Population)
        {
            try
            {
                AggGDPPopulation[Continent].GDP_2012 += GDP;
                AggGDPPopulation[Continent].POPULATION_2012 += Population;
            }
            catch (Exception)
            {
                AggGDPPopulation.Add(Continent, new GDPPopulationClass() { GDP_2012 = GDP, POPULATION_2012 = Population });
            }
        }


    }

    public static class JSONOperations
    {
        public static JObject DeSerializeString(string s)
        {
            return JObject.Parse(s);
        }
        public static string SerializeJObject(Dictionary<string, GDPPopulationClass> Object)
        {
            return JsonConvert.SerializeObject(Object, Formatting.Indented);
        }
    }

    public class AggregateCalculationsClass
    {
        public string CSVPath;
        public string CountryContinentMapPath;
        public string OutputPath;
        public AggregateGDPPopulationClass AggregatedData;
        public AggregateCalculationsClass()//string CSVPath, string CountryContinentMapPath)
        {
            CSVPath = "../../../../AggregateGDPPopulation/data/datafile.csv";
            CountryContinentMapPath = "../../../../AggregateGDPPopulation/data/cc-mapping.json";
            OutputPath = "../../../../AggregateGDPPopulation/output/output.json";
            AggregatedData = new AggregateGDPPopulationClass();
            //AggregatedJSON = new JObject();
        }
        public async Task<JObject> AggregateCalculations()
        {
            Task<string> FileContentsTask = FileOperations.ReadfileAsync(CSVPath);
            Task<string> JSONMapTask = FileOperations.ReadfileAsync(CountryContinentMapPath);
            string FileContentsComplete = await FileContentsTask;
            string[] FileContents = FileContentsComplete.Split('\n');
            string headerText = FileContents[0];
            string[] headers = headerText.Replace("\"", "").Split(',');

            int IndexOfCountry = Array.IndexOf(headers, "Country Name"); ;
            int IndexOfPopulation = Array.IndexOf(headers, "Population (Millions) 2012");
            int IndexOfGDP = Array.IndexOf(headers, "GDP Billions (USD) 2012");

            string CountryContinentJSONFileContents = await JSONMapTask;

            var CountryContinentMap = JSONOperations.DeSerializeString(CountryContinentJSONFileContents);
            for (int i = 1; i < FileContents.Length-1; i++)
            {
                List<string> RowOfData = FileContents[i].Split(',').ToList();
                string Country = RowOfData[IndexOfCountry].Trim('\"');
                float Population = float.Parse(RowOfData[IndexOfPopulation].Trim('\"'));
                float Gdp = float.Parse(RowOfData[IndexOfGDP].Trim('\"'));
                try
                {
                    string Continent = CountryContinentMap.GetValue(RowOfData[IndexOfCountry].Trim('\"')).ToString();
                    AggregatedData.AddOrUpdateAggGDPPopulation(Continent, Gdp, Population);
                }
                catch (Exception) { }
            }
            var JSONOutput = JSONOperations.SerializeJObject(AggregatedData.AggGDPPopulation);
            var AggregatedJSON = JSONOperations.DeSerializeString(JSONOutput);
            FileOperations.WriteFileAsync(OutputPath, JSONOutput);
            return AggregatedJSON;
        }
    }



}
