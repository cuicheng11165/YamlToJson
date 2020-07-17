using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using YamlDotNet.Serialization;

namespace YamlToJson
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = (Parsed<Options>)Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       if (!String.IsNullOrEmpty(o.YamlLocation))
                       {
                           Console.WriteLine($"Verbose output enabled. Current Arguments: -v {o.YamlLocation}");

                       }
                       if (!String.IsNullOrEmpty(o.JsonLocation))
                       {
                           Console.WriteLine($"Verbose output enabled. Current Arguments: -v {o.JsonLocation}");

                       }
                   });



            using (StreamReader streamReader = new StreamReader(result.Value.YamlLocation))
            {
                var deserializer = new Deserializer();
                var yamlObject = deserializer.Deserialize(streamReader);
                var list = new String[] { "spec", "template", "spec", "containers", "env" };
                var tmpObject = yamlObject;
                foreach (var path in list)
                {
                    var dictionaryObject = tmpObject as Dictionary<object, object>;
                    if (dictionaryObject != null)
                    {
                        tmpObject = dictionaryObject[path];
                    }
                    else
                    {
                        var listObject = tmpObject as List<object>;
                        tmpObject = listObject[0];

                        dictionaryObject = tmpObject as Dictionary<object, object>;
                        if (dictionaryObject != null)
                        {
                            tmpObject = dictionaryObject[path];
                        }
                    }
                }
                var tmpList = tmpObject as List<object>;
                var dic = tmpList.Cast<Dictionary<object, object>>().ToDictionary(d => d["name"], d => d["value"]);

                var serializer = new Newtonsoft.Json.JsonSerializer();

                using (var streamWriter = new StreamWriter(result.Value.JsonLocation))
                {
                    serializer.Serialize(streamWriter, dic);
                }
            }
        }
    }



    public class Options
    {
        [Option('y', "YamlLocation", Required = true, HelpText = "Input yaml file location")]
        public string YamlLocation { get; set; }


        [Option('j', "JsonLocation", Required = true, HelpText = "Output json file")]
        public string JsonLocation { get; set; }
    }
}
