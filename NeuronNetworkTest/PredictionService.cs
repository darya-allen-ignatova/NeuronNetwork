using DataAccessApp;
using Microsoft.ML;
using NeuronNetworkTest.Data;
using System;
using System.Collections.Generic;

namespace NeuronNetworkTest
{
    internal static class PredictionService
    {
        public static void ShowPredictResults(string dataPath, string modelPath, int predictDataSheetNumber)
        {
            MLContext mlContext = new MLContext();

            List<InputData> input = ExcelDataProvider.GetData(dataPath, predictDataSheetNumber);
            IDataView dataView = mlContext.Data.LoadFromEnumerable<InputData>(input);
            IEnumerable<InputData> samples = mlContext.Data.CreateEnumerable<InputData>(dataView, false);

            Console.WriteLine("Predictions");
            foreach (var sample in samples)
            {
                OutputData predictionResult = Predict(sample, modelPath);

                Console.WriteLine($"Word: {sample.Word}");
                Console.WriteLine($"\nActual Category: {sample.Category} \nPredicted Category: {predictionResult.Prediction}\nPredicted Category scores: [{String.Join(",", predictionResult.Score)}]\n\n");

            }

            Console.ReadKey();
        }

        private static OutputData Predict(InputData input, string modelPath)
        {
            MLContext mlContext = new MLContext();
            var predictionEngine = mlContext.Model.CreatePredictionEngine<InputData, OutputData>(mlContext.Model.Load(modelPath, out var modelInputSchema));

            OutputData result = predictionEngine.Predict(input);
            return result;
        }
    }
}
