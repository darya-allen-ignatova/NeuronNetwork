using Microsoft.ML.Data;
namespace NeuronNetworkTest.Data
{
    internal class InputData
    {
        public InputData(){ }

        public InputData(string category, string word)
        {
            Category = category;
            Word = word;
        }


        [ColumnName("Category"), LoadColumn(0)]
        public string Category { get; set; }


        [ColumnName("Word"), LoadColumn(1)]
        public string Word { get; set; }
    }
}
