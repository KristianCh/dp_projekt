// Author: Kristián Chovančák
// Created: 04.10.2023
// Copyright (c) Noxgames
// http://www.noxgames.com/

using Entities.GameManagement;
using UnityEngine;

namespace Entities.WordProcessing
{
    public class WordProcessingManager : MonoBehaviour, IService
    {
        public void Awake()
        {
            GameManager.AddService(this);
        }

        public WordTriple GetWordTriple()
        {
            return new WordTriple("", "", "");
        }
    }
}