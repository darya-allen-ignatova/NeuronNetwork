namespace NeuronNetworkTest.Data
{
    public class InputData
    {
        public InputData(){ }

        public InputData(string category, string word)
        {
            Category = category;
            Word = word;
        }

        public string Category { get; set; }

        public string Word { get; set; }
    }
}
