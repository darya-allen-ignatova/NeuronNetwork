using Microsoft.ML.Data;

namespace NeuronNetworkTest.Data
{
    public class Output
    {
        [ColumnName("PredictedLabel")]
        public uint Prediction { get; set; }

        /*public string Category
        {
            get
            {
                switch (Prediction)
                {
                    case 1: return "noun";
                    case 2: return "adverb";
                    case 3: return "adjective";
                }

                return null;
            }
        }*/
        public float[] Score { get; set; }
    }
}
