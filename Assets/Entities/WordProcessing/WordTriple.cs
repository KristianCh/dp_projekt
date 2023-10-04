// Author: Kristián Chovančák
// Created: 04.10.2023
// Copyright (c) Noxgames
// http://www.noxgames.com/

namespace Entities.WordProcessing
{
    public class WordTriple
    {
        public string PairWordA { get; set; }
        public string PairWordB { get; set; }
        public string IncorrectWord { get; set; }

        public WordTriple(string pairWordA, string pairWordB, string incorrectWord)
        {
            PairWordA = pairWordA;
            PairWordB = pairWordB;
            IncorrectWord = incorrectWord;
        }
    }
}