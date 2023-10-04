// Author: Kristián Chovančák
// Created: 04.10.2023
// Copyright (c) Noxgames
// http://www.noxgames.com/

using Entities.Utils;
using Entities.WordProcessing;
using TMPro;
using UnityEngine;

namespace Entities.Gameplay
{
    public class WordTripleGate : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _TitleWord;

        [SerializeField]
        private TMP_Text _WordA;

        [SerializeField]
        private TMP_Text _WordB;

        [SerializeField]
        private Collider _ColliderA;

        [SerializeField]
        private Collider _ColliderB;

        [SerializeField]
        private Collider _NeutralCollider;

        private bool _isASelected;
        private WordTriple _currentTriple;

        private bool IsASelected => _isASelected;
        private bool IsBSelected => !_isASelected;

        private TMP_Text CurrentCorrectText => IsASelected ? _WordA : _WordB;
        private TMP_Text CurrentIncorrectText => IsASelected ? _WordB : _WordA;

        private Collider CurrentCorrectCollider => IsASelected ? _ColliderA : _ColliderB;
        private Collider CurrentIncorrectCollider => IsASelected ? _ColliderB : _ColliderA;

        public void SetWordTriple(WordTriple wordTriple)
        {
            _currentTriple = wordTriple;

            _isASelected = RandomUtils.RandomBool();
            if (RandomUtils.RandomBool())
            {
                _TitleWord.text = wordTriple.PairWordA;
                CurrentCorrectText.text = wordTriple.PairWordB;
            }
            else
            {
                _TitleWord.text = wordTriple.PairWordB;
                CurrentCorrectText.text = wordTriple.PairWordA;
            }

            CurrentIncorrectText.text = wordTriple.IncorrectWord;
        }
    }
}