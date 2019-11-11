using DataAccessApp;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Models;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;

namespace NNNewTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //LearningPipeline pipeline = new LearningPipeline();
            //pipeline.Add(new TextLoader(@"C:\Users\Дарья\Desktop\AdverbNounColumnInt.csv").CreateFrom<Model>(useHeader: true, separator: ','));
            ////pipeline.Add(new Dictionarizer(("Category", "Label")));
            ////pipeline.Add(new ColumnCopier("Category","Label" ));
            //pipeline.Add(new CategoricalOneHotVectorizer("Word") { MaxNumTerms = 26 });
            //pipeline.Add(new ColumnConcatenator("Features", "Word"));
            ////pipeline.Add(new PredictedLabelColumnOriginalValueConverter() { PredictedLabelColumn = "PredictedLabel" });
            //pipeline.Add(new FastTreeRegressor());
            ////Vec < R4, 472 >

            //var model = pipeline.Train<Model, CategoryPrediction>();

            //model.WriteAsync(@"C:\Users\Дарья\Desktop\NewModel.zip");

            //model = PredictionModel.ReadAsync<Model, CategoryPrediction>(@"C:\Users\Дарья\Desktop\NewModel.zip").Result;


            //var testData = new TextLoader(@"C:\Users\Дарья\Desktop\TestAdverbNoun.csv").CreateFrom<Model>(useHeader: true, separator: ',');
            //var evaluator = new ClassificationEvaluator();
            //var metrics = evaluator.Evaluate(model, testData);

            //Console.WriteLine($"AccuracyMicro: {metrics.AccuracyMicro}\nAccuracyMacro: {metrics.AccuracyMacro}");

            //var data = ExcelDataProvider.GetData(@"C:\Users\Дарья\Desktop\TestAdverbNoun.xlsx", 0);
            //List<Model> models = new List<Model>();
            //data.ForEach(x => models.Add(new Model()
            //{
            //    Category = x.Category,
            //    Word = x.Word
            //}));

            //foreach (var sample in models)
            //{
            //    var prediction = model.Predict(sample);

            //    Console.WriteLine($"Word: {sample.Word}");
            //    Console.WriteLine($"\nActual Category: {sample.Category} \nPredicted Category: {prediction.Category}\nPredicted Category scores: [{String.Join(",", prediction.Score)}]\n\n");

            //}
            Ex();
            Console.ReadLine();
        }


        public static void Ex()
        {
            LearningPipeline pipeline = new LearningPipeline();
            pipeline.Add(new TextLoader(@"C:\Users\Дарья\Desktop\AdverbNoun.csv").CreateFrom<Model>(useHeader: true, separator: ','));
            pipeline.Add(new Dictionarizer(("Category", "Label")));
            pipeline.Add(new TextFeaturizer("Features", "Word"));
            pipeline.Add(new StochasticDualCoordinateAscentClassifier());
            //Vec < R4, 472 >
            pipeline.Add(new PredictedLabelColumnOriginalValueConverter() { PredictedLabelColumn = "PredictedLabel" });
            var model = pipeline.Train<Model, CategoryPrediction>();

            model.WriteAsync(@"C:\Users\Дарья\Desktop\NewModel.zip");

            model = PredictionModel.ReadAsync<Model, CategoryPrediction>(@"C:\Users\Дарья\Desktop\NewModel.zip").Result;


            var testData = new TextLoader(@"C:\Users\Дарья\Desktop\TestAdverbNoun.csv").CreateFrom<Model>(useHeader: true, separator: ',');
            var evaluator = new ClassificationEvaluator();
            var metrics = evaluator.Evaluate(model, testData);

            Console.WriteLine($"AccuracyMicro: {metrics.AccuracyMicro}\nAccuracyMacro: {metrics.AccuracyMacro}");

            var data = ExcelDataProvider.GetData(@"C:\Users\Дарья\Desktop\TestAdverbNoun.xlsx", 0);
            List<Model> models = new List<Model>();
            data.ForEach(x => models.Add(new Model()
            {
                Category = x.Category,
                Word = x.Word
            }));

            foreach (var sample in models)
            {
                var prediction = model.Predict(sample);

                Console.WriteLine($"Word: {sample.Word}");
                Console.WriteLine($"\nActual Category: {sample.Category} \nPredicted Category: {prediction.Category}\nPredicted Category scores: [{String.Join(",", prediction.Score)}]\n\n");
            }

            Console.ReadLine();
        }
    }

    public class Model
    {
        [Column("0")]
        public string Category;

        [Column("1")]
        public string Word;
    }

    public class CategoryPrediction
    {
        [ColumnName("PredictedLabel")]
        public string Category;
        public float[] Score { get; set; }
    }
}
