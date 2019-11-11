using DataAccessApp;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using NeuronNetworkTest.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeuronNetworkTest.Model
{
    internal class ModelProvider
    {
        private readonly MLContext mlContext;
        private readonly ModelData data;
        private const int trainingSheetNumber = 0;

        public ModelProvider(ModelData data)
        {
            this.data = data;
            //Seed - for repeatable results
            mlContext = new MLContext(seed: 1);
        }

        public void Create()
        {
            //Data from training file sheet
            IDataView trainingDataView = mlContext.Data.LoadFromEnumerable(GetDataFromExcel(trainingSheetNumber));

            //Creating pipeline
            IEstimator<ITransformer> trainingPipeline = GetTrainingPipeline();

            //Show metrics of made pipeline
            PrintAccuracyOfModel(trainingDataView, trainingPipeline);

            //TRAINING process
            ITransformer mlModel = TrainModel(trainingDataView, trainingPipeline);

            //Saving model for future predictions
            SaveModel(mlModel, trainingDataView.Schema);
        }

        private IEstimator<ITransformer> GetTrainingPipeline()
        {
            //first parameter is OUTPUT column, second INPUT column
            var dataPipeline = mlContext.Transforms.Conversion.MapValueToKey("Category", "Category") // Category name to key
                                    .Append(mlContext.Transforms.Text.FeaturizeText("Word_vector", "Word")) // words to vector in new column
                                    .Append(mlContext.Transforms.CopyColumns("Features", "Word_vector")) // new column using during training
                                    .Append(mlContext.Transforms.NormalizeMinMax("Features", "Features"))
                                    //.Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName: "Category", featureColumnName: "Features"))
                                    .AppendCacheCheckpoint(mlContext);

            var trainer = GetTrainer_SgdCalibrated();
            var trainingPipeline = dataPipeline.Append(trainer); // creating training pipeline
            return dataPipeline;
        }

        private EstimatorChain<KeyToValueMappingTransformer> GetTrainer_AveragedPerceptron()
        {
            return mlContext.MulticlassClassification.Trainers.OneVersusAll(
                         mlContext.BinaryClassification.Trainers.AveragedPerceptron(
                                          new AveragedPerceptronTrainer.Options() // one of trainers
                                          {
                                              LearningRate = data.LearningRate,
                                              DecreaseLearningRate = false,
                                              L2Regularization = data.L2Regularization,
                                              NumberOfIterations = data.NumberOfIterations, // iterations
                                              InitialWeightsDiameter = data.InitialWeightsDiameter,
                                              Shuffle = true,
                                              LabelColumnName = "Category",
                                              FeatureColumnName = "Features"
                                          }), labelColumnName: "Category")
                       .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));
        }

        private EstimatorChain<KeyToValueMappingTransformer> GetTrainer_LinearSvm()
        {
            return mlContext.MulticlassClassification.Trainers.OneVersusAll(mlContext.BinaryClassification.Trainers.LinearSvm(labelColumnName: "Category", featureColumnName: "Features"), labelColumnName: "Category")
                            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));
        }

        private EstimatorChain<KeyToValueMappingTransformer> GetTrainer_SgdCalibrated()
        {
             return mlContext.MulticlassClassification.Trainers.OneVersusAll(mlContext.BinaryClassification.Trainers.SgdCalibrated(labelColumnName: "Category", featureColumnName: "Features"), labelColumnName: "Category")
                                      .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));
        }

        private void PrintAccuracyOfModel(IDataView trainingDataView, IEstimator<ITransformer> trainingPipeline)
        {
            //return metrics that provide data about created model
            var crossValidationResults = mlContext.MulticlassClassification.CrossValidate(trainingDataView, trainingPipeline, numberOfFolds: 5, labelColumnName: "Category");
            var metricsInMultipleFolds = crossValidationResults.Select(r => r.Metrics);

            var microAccuracyValues = metricsInMultipleFolds.Select(m => m.MicroAccuracy);
            var microAccuracyAverage = microAccuracyValues.Average();

            var macroAccuracyValues = metricsInMultipleFolds.Select(m => m.MacroAccuracy);
            var macroAccuracyAverage = macroAccuracyValues.Average();

            Console.WriteLine($"Metrics for Multi-class Classification model");
            Console.WriteLine($"Average MicroAccuracy:    {microAccuracyAverage:0.###}");
            Console.WriteLine($"Average MacroAccuracy:    {macroAccuracyAverage:0.###}");
        }

        public ITransformer TrainModel(IDataView trainingDataView, IEstimator<ITransformer> trainingPipeline)
        {
            Console.WriteLine("Training of model.....");
            ITransformer model = trainingPipeline.Fit(trainingDataView);
            Console.WriteLine("End of training!!!");

            return model;
        }

        private void SaveModel(ITransformer mlModel, DataViewSchema modelInputSchema)
        {
            // Savethe trained model to a .ZIP file
            Console.WriteLine("Saving model.....");
            mlContext.Model.Save(mlModel, modelInputSchema, GetAbsolutePath(data.ModelPath));
            Console.WriteLine("The model was saved!\nPath: {0}", GetAbsolutePath(data.ModelPath));
        }

        public static string GetAbsolutePath(string filePath)
        {
            string assemblyFolderPath = new FileInfo(typeof(Program).Assembly.Location).Directory.FullName;
            string fullPath = Path.Combine(assemblyFolderPath, filePath);

            return fullPath;
        }

        private List<InputData> GetDataFromExcel(int worksheetIndex)
        {
            return ExcelDataProvider.GetData(data.DataPath, worksheetIndex);
        }
    }
    
}
