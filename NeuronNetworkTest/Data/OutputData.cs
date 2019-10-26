using Microsoft.ML.Data;

namespace NeuronNetworkTest.Data
{
    internal class OutputData
    {
        [ColumnName("PredictedLabel")]
        public string Prediction { get; set; }
        public float[] Score { get; set; }
    }
}
