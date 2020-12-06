using CognitiveMap;
using CommandLine;
using System;
using System.Linq;

namespace CognitiveMap
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args).WithParsed(options =>
            {
                var dataset = (Dataset)Enum.Parse(typeof(Dataset), options.DatasetName, true);
                var cognitiveMap = new CognitiveMapStructure(options.Nodes, dataset, options.StepSize, Options.GetFunctionByName(options.FunctionName), options.Epsilon);
                cognitiveMap.Train(options.Epoches);
                cognitiveMap.Test();
            }
            );
        }
    }
}

class Options
{
    [Option('d', "dataset", Required = true,
      HelpText = "DatasetName : [PARKING, SNP_IBM, SNP_EBAY, FLU, DIPED, TEMP]")]
    public string DatasetName { get; set; }

    [Option('l', "lr", Required = false, Default = 0.01d,
      HelpText = "Step size (0.0, 0.5)")]
    public double StepSize { get; set; }

    [Option('e', "epoches", Required = false, Default = 3000,
      HelpText = "Number of epoches to train (int)")]
    public int Epoches { get; set; }

    [Option('n', "nodes", Required = false, Default = 5,
     HelpText = "Number of nodes (int), Default = 5")]
    public int Nodes { get; set; }

    [Option('f', "func", Required = false, Default = "Sigmoid",
     HelpText = "Squashing Function [Sigmoid, Gausian]")]
    public String FunctionName { get; set; }

    public static ActivationFunction GetFunctionByName(string name)
    {
        var type = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from t in assembly.GetTypes()
                    where t.Name == name  // you could use the t.FullName aswel
                    select t).FirstOrDefault();

        if (type == null)
            throw new InvalidOperationException("Type not found");

        return (ActivationFunction)Activator.CreateInstance(type);
    }

    [Option('s', "epsilon", Required = false, Default = 0.0001,
     HelpText = "Parameter identifying if the network has converged")]
    public double Epsilon { get; set; }
}

