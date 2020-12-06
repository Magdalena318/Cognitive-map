using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.Data.Text;
using MathNet.Numerics.LinearAlgebra;
using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra.Double;
using System.IO;
using System.Linq;

namespace CognitiveMap
{
    class CognitiveMapStructure
    {
        Matrix<double> weights;
        Matrix<double> dWeights;
        Vector<double> nodes;
        ActivationFunction activationFunction;
        DatasetReader datasetReader;
        Adam adam;
        int windowSize;
        double epsilon;
        public CognitiveMapStructure(int size, Dataset dataset, double _stepSize, ActivationFunction function, double _epsilon)
        {
            epsilon = _epsilon;
            datasetReader = new DatasetReader(dataset);
            adam = new Adam(size, _stepSize);
            activationFunction = function;
            windowSize = size;
            var rand = new Random();
            weights = Matrix<double>.Build.Dense(size, size, (i, j) => rand.NextDouble());
            dWeights = Matrix<double>.Build.Dense(size, size);
            nodes = Vector<double>.Build.Dense(size, (i) => rand.NextDouble());
        }

        public void Train(int numEpochs)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var logger = new Logger();
            var dataInBatches = datasetReader.GetVectorBatches(1);
            var lastMinCost = double.PositiveInfinity;
            for (int i = 0; i < numEpochs; i++)
            {
                var totalCost = 0.0d;
                var lastMaxCost = 0.0d;
                var n = 0;
                foreach (var batch in dataInBatches)
                {
                    if (batch.Count > windowSize)
                    {
                        var cost = trainBatch(batch, i + 1);
                        totalCost += cost;
                        n++;
                        lastMaxCost = Math.Max(cost, lastMaxCost);
                        lastMinCost = Math.Min(cost, lastMinCost);
                    }
                }
                logger.Log(i, numEpochs, totalCost / n, lastMinCost, lastMaxCost);
                if (totalCost / n <= epsilon)
                    break;
            }
            sw.Stop();
            Console.WriteLine("\n\nElapsed={0}\n", sw.Elapsed);
        }

        double trainBatch(Vector<double> batch, int iteration)
        {
            dWeights.Clear();
            var count = batch.Count - windowSize;
            var cost = 0.0d;
            Vector<double> expected = batch.SubVector(0, windowSize);
            for (var i = 0; i < count; i++)
            {
                var input = expected;
                expected = batch.SubVector(i + 1, windowSize);
                var preOut = computePreOut(input);
                var output = preOut.Map(v => activationFunction.Evalutate(v), Zeros.Include);
                cost += calculateCost(expected, output);
                backpropagation(input, expected, preOut, output);
            }
            dWeights = dWeights.Divide(count);
            adam.updateWeights(ref weights, dWeights, iteration);
            weights = weights.PointwiseMaximum(-1.0d).PointwiseMinimum(1.0d);
            return cost / count;
        }

        Vector<double> computePreOut(Vector<double> input)
        {
            var output = Vector<double>.Build.Dense(input.Count);
            for (int i = 0; i < windowSize; i++)
                output[i] = weights.Row(i).PointwiseMultiply(input).Sum();
            return output;
        }


        double calculateCost(Vector<double> expected, Vector<double> predicted)
        {
            var cost = 0.0d;
            for (int i = 0; i < expected.Count; i++)
                cost += Math.Pow(expected[i] - predicted[i], 2);
            return cost / expected.Count;
        }

        void backpropagation(Vector<double> input, Vector<double> expected, Vector<double> preout, Vector<double> predicted)
        {
            for (int i = 0; i < windowSize; i++)
            {
                var costDerivative = 2 * (predicted[i] - expected[i]);
                var activationDerivative = activationFunction.EvaluateDerivative(preout[i]);
                for (int j = 0; j < windowSize; j++)
                {
                    dWeights[i, j] += input[j] * activationDerivative * costDerivative;
                }
            }
        }

        public void Test()
        {
            var predicted = new List<double>();
            var expected = new List<double>();
            var dataInBatches = datasetReader.GetTestData();
            var totalCost = 0.0d;
            var n = 0;
            foreach (var batch in dataInBatches)
            {
                if (batch.Count > windowSize)
                {
                    totalCost += testBatch(batch, ref predicted, ref expected);
                    n++;
                }
            }
            Console.WriteLine(String.Format("[AVRG COST] = {0:00.00000}", totalCost / n));
            var directory = Directory.GetCurrentDirectory();
            var path = Path.Combine(directory, datasetReader.Name);
            DelimitedWriter.Write(path + "_weights.txt", weights, ",");
            var numberList = Enumerable.Range(1, expected.Count).Select(i => (double)i).ToArray();
            var plt = new ScottPlot.Plot(1000, 400);
            plt.Title("Expected vs Predicted {" + datasetReader.Name + "}, " + String.Format("[AVRG COST] = {0:00.00000}", totalCost / n));
            plt.PlotScatter(numberList, predicted.ToArray(), label: "pred");
            plt.PlotScatter(numberList, expected.ToArray(), label: "exp");
            plt.Legend();
            plt.XLabel("#Data Point");
            plt.Axis(y1: -0.5d, y2: 1.5d);
            plt.SaveFig(path + "_plot.png");

        }

        double testBatch(Vector<double> batch, ref List<double> outPredicted, ref List<double> outExpected)
        {
            var expectedList = new List<double>();
            var count = batch.Count - windowSize;
            var cost = 0.0d;
            Vector<double> expected = batch.SubVector(0, windowSize);
            for (var i = 0; i < count; i++)
            {
                var input = expected;
                expected = batch.SubVector(i + 1, windowSize);
                var preOut = computePreOut(input);
                var output = preOut.Map(v => activationFunction.Evalutate(v), Zeros.Include);
                outPredicted.Add(output[windowSize - 1]);
                outExpected.Add(expected[windowSize - 1]);
                cost += calculateCost(expected, output);
            }
            return cost / count;
        }
    }
}
