using Accord;
using Accord.MachineLearning.Bayes;
using Accord.Math;
using Accord.Statistics.Filters;
using DataAccessApp;
using NeuronNetworkTest.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace AccordApp
{
    class Program
    {
        static void Main(string[] args)
        {
            DataTable data = new DataTable("Categories of words");
            data.Columns.Add("Category", "Word");
            List<InputData> words = ExcelDataProvider.GetData(@"C:\Users\Дарья\Desktop\AdverbNoun.xlsx", 0);

            foreach(var word in words)
            {
                data.Rows.Add(word.Category, word.Word);
            }

            Codification codebook = new Codification(data, "Category", "Word");

            DataTable symbols = codebook.Apply(data);
            int[][] inputs = symbols.ToJagged<int>("Category");
            int[] outputs = symbols.ToArray<int>("Word");

            var learner = new NaiveBayesLearning();
            NaiveBayes nb = learner.Learn(inputs, outputs);

            data = new DataTable("Categories of words");
            data.Columns.Add("Category", "Word");
            words = ExcelDataProvider.GetData(@"C:\Users\Дарья\Desktop\TestAdverbNoun.xlsx", 0);

            foreach (var word in words)
            {
                data.Rows.Add(word.Category, word.Word);
            }

            int[] instance = codebook.Translate("helpful");

            int c = nb.Decide(instance);

            string result = codebook.Translate("Category", c);

            double[] probs = nb.Probabilities(instance);

            Console.WriteLine(0);
        }
    }
}
