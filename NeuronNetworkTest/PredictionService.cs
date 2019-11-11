using DataAccessApp;
using Microsoft.ML;
using NeuronNetworkTest.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuronNetworkTest
{
    internal static class PredictionService
    {
        public static void ShowPredictResults(string dataPath, string modelPath, int predictDataSheetNumber)
        {
            MLContext mlContext = new MLContext();

            List<InputData> inputs = ExcelDataProvider.GetData(dataPath, predictDataSheetNumber);
            List<Input> input = new List<Input>();
            inputs.ForEach(x => input.Add(new Input(x.Category, x.Word)));
            IDataView dataView = mlContext.Data.LoadFromEnumerable<Input>(input);
            IEnumerable<Input> samples = mlContext.Data.CreateEnumerable<Input>(dataView, false);

            Console.WriteLine("Predictions");
            foreach (var sample in samples)
            {
                Output predictionResult = Predict(sample, modelPath);

                Console.WriteLine($"Word: {sample.Word}");
                //Console.WriteLine($"\nActual Category: {sample.Category} \nPredicted Category: {predictionResult.Category}\nPredicted Category scores: [{String.Join(",", predictionResult.Score)}]\n\n");
                Console.WriteLine($"\nActual Category: {sample.Category} \nPredicted Category: {predictionResult.Prediction}\nPredicted Category scores: [{String.Join(",", predictionResult.Score)}]\n\n");

            }

            Console.ReadKey();
        }

        private static Output Predict(Input input, string modelPath)
        {
            MLContext mlContext = new MLContext();
            var predictionEngine = mlContext.Model.CreatePredictionEngine<Input, Output>(mlContext.Model.Load(modelPath, out var modelInputSchema));

            Output result = predictionEngine.Predict(input);
            return result;
        }
    }
}
