namespace Entities.WordProcessing
{
    /// <summary>
    /// Class including correct word, incorrect word, pair word and the recorded AoA of the main word.
    /// </summary>
    public class WordTriple
    {
        public string MainWord { get; set; }
        public string PairWord { get; set; }
        public string IncorrectWord { get; set; }
        
        public float WordAOA { get; set; }

        public WordTriple(string mainWord, string pairWord, string incorrectWord, float wordAOA)
        {
            MainWord = mainWord;
            PairWord = pairWord;
            IncorrectWord = incorrectWord;
            WordAOA = wordAOA;
        }
    }
}