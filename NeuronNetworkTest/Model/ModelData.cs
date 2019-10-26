using System;
using System.Collections.Generic;
using System.Text;

namespace NeuronNetworkTest.Model
{
    internal class ModelData
    {
        public ModelData()
        {
            ModelPath = @"C:\Users\Дарья\Desktop\MLModel.zip";
            DataPath = @"C:\Users\Дарья\Desktop\exceldata.xlsx";
            LearningRate = 0.01f;
            L2Regularization = 0.2f;
            NumberOfIterations = 10;
            InitialWeightsDiameter = 0.5f;
        }

        public string ModelPath { get; set; }

        public string DataPath { get; set;  }

        public float LearningRate { get; set; }

        public float L2Regularization { get; set; }

        public int NumberOfIterations { get; set; }

        public float InitialWeightsDiameter { get; set; }
    }
}
