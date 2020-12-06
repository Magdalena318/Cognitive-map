using System;
using System.Collections.Generic;
using System.IO;
using MathNet.Numerics.LinearAlgebra;

namespace CognitiveMap
{
    enum Dataset { PARKING, SNP_IBM, SNP_EBAY, FLU, DIPED, TEMP };
    class DatasetReader
    {
        public string Name;
        const double trainSplit = 0.8d;
        string[] datasetPath = { @"1. Birmigham parking\dataset.csv",
            @"2. S&P index\IBM_data.csv", @"2. S&P index\EBAY_data.csv",
            @"3. Seasonal influenza predictions (USA only)\influenza_weekly.csv",
            @"4. Dynamics in Political Economy Data\garrett1998.csv",
            @"5. Temperature time series (global)\global_temperature.csv"
        };
        Tuple<int, int>[] datasetKeyValueIdMap = {
            Tuple.Create(0, 4),
            Tuple.Create(-1, 2),
            Tuple.Create(-1, 2),
            Tuple.Create(-1, 5),
            Tuple.Create(0, 3),
            Tuple.Create(0, 4),
            Tuple.Create(0, 4),
        };
        Dictionary<string, List<double>> dataMap;
        public DatasetReader(Dataset dataset)
        {
            Name = dataset.ToString();
            var directory = Directory.GetCurrentDirectory();
            var path = Path.Combine(directory, "Datasets", datasetPath[(int)dataset]);
            dataMap = new Dictionary<string, List<double>>();
            int keyId = datasetKeyValueIdMap[(int)dataset].Item1;
            int valueId = datasetKeyValueIdMap[(int)dataset].Item2;
            if (keyId < 0)
            {
                dataMap.Add("data", new List<double>());
                using (var reader = new StreamReader(path))
                {
                    reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        dataMap["data"].Add(Double.Parse(values[valueId]));
                    }
                }
                return;
            }
            using (var reader = new StreamReader(path))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    if (!dataMap.ContainsKey(values[0])) dataMap.Add(values[0], new List<double>());
                    dataMap[values[0]].Add(Double.Parse(values[valueId]));
                }
            }
        }

        public List<Vector<double>> GetVectorBatches(int numBatchesPerType)
        {
            int nBatches = numBatchesPerType;
            var batches = new List<Vector<double>>();
            var numerator = dataMap.Keys.GetEnumerator();
            while (numerator.MoveNext())
            {
                var data = dataMap[numerator.Current];
                var count = (int)(data.Count * trainSplit);
                var dCount = (int)(count / nBatches);
                for (int i = 0; i < nBatches - 1; i++)
                    batches.Add(Vector<double>.Build.DenseOfArray(data.GetRange(i * dCount, dCount).ToArray()));
                var index = (nBatches - 1) * dCount;
                batches.Add(Vector<double>.Build.DenseOfArray(data.GetRange(index, count - index).ToArray()));
            }
            return batches;
        }

        public List<Vector<double>> GetTestData()
        {
            var batches = new List<Vector<double>>();
            var numerator = dataMap.Keys.GetEnumerator();
            while (numerator.MoveNext())
            {
                var data = dataMap[numerator.Current];
                var start = (int)(data.Count * trainSplit);
                batches.Add(Vector<double>.Build.DenseOfArray(data.GetRange(start, data.Count - start).ToArray()));
            }
            return batches;
        }
    }
}
