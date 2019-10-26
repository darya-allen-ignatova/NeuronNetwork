using DataAccessApp;
using Microsoft.ML;
using NeuronNetworkTest.Data;
using NeuronNetworkTest.Model;
using System;
using System.Collections.Generic;

namespace NeuronNetworkTest
{
    class Program
    {
        const string dataPathCSV = @"C:\Users\Дарья\Desktop\exceldata.csv";
        const string dataPath = @"C:\Users\Дарья\Desktop\exceldataAVN.xlsx";
        const string modelPath = @"C:\Users\Дарья\Desktop\MLModel.zip";
        const int predictDataSheetNumber = 1;

        static void Main(string[] args)
        {
            ModelData modelData = new ModelData()
            {
                DataPath = dataPath,
                ModelPath = modelPath,
                InitialWeightsDiameter = 0.47f,
                LearningRate = 0.01f,
                L2Regularization = 0.01f,
                NumberOfIterations = 50
            };

            ModelProvider provider = new ModelProvider(modelData);
            provider.Create();
            PredictionService.ShowPredictResults(dataPath, modelPath, predictDataSheetNumber);
        }
    }
}
