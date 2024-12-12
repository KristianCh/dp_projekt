using Entities.GameManagement;
using Entities.Utils;
using Entities.WordProcessing;
using TMPro;
using UnityEngine;

namespace Entities.Gameplay
{
    /// <summary>
    /// Controller handling rating gate behaviour.
    /// </summary>
    public class WordTripleGate : WorldObjectMovementController
    {
        [SerializeField]
        private TMP_Text _TitleWord;

        [SerializeField]
        private TMP_Text _WordA;

        [SerializeField]
        private TMP_Text _WordB;

        [SerializeField]
        private ColliderEvents _ColliderA;

        [SerializeField]
        private ColliderEvents _ColliderB;

        [SerializeField]
        private ColliderEvents _NeutralCollider;
        
        [Header("Particles")]
        [SerializeField]
        private ParticleSystem _CorrectParticleSystem;
        
        [SerializeField]
        private ParticleSystem _IncorrectParticleSystem;
        
        [SerializeField]
        private ParticleSystem _NeutralParticleSystem;

        private bool _isASelected;
        private WordTriple _currentTriple;

        private TMP_Text CurrentCorrectText => _isASelected ? _WordA : _WordB;
        private TMP_Text CurrentIncorrectText => _isASelected ? _WordB : _WordA;

        private ColliderEvents CurrentCorrectCollider => _isASelected ? _ColliderA : _ColliderB;
        private ColliderEvents CurrentIncorrectCollider => _isASelected ? _ColliderB : _ColliderA;
        
        private DatabaseHandler _databaseHandler;
        private WordProcessingManager _wordProcessingManager;

        
        /// <summary>
        /// Generates word to use.
        /// </summary>
        private void Awake()
        {
            _databaseHandler = GameManager.GetService<DatabaseHandler>();
            _wordProcessingManager = GameManager.GetService<WordProcessingManager>();
            SetWordTriple(_wordProcessingManager.GetWordTriple());
        }

        /// <summary>
        /// Updates text scale based on distance.
        /// </summary>
        public override void Update()
        {
            base.Update();
            var scale = Mathf.Clamp(1f + (transform.position.z - 20) * 0.025f, 1f, 4f);
            if(_TitleWord == null || _TitleWord.canvas == null) return;
            _TitleWord.canvas.transform.localScale = Vector3.one * scale;
        }

        /// <summary>
        /// Sets word displays.
        /// </summary>
        private void SetWordTriple(WordTriple wordTriple)
        {
            _currentTriple = wordTriple;

            _isASelected = RandomUtils.RandomBool();
            
            _TitleWord.text = _wordProcessingManager.NicifyWord(wordTriple.MainWord);
            CurrentCorrectText.text = _wordProcessingManager.NicifyWord(wordTriple.PairWord);
            CurrentIncorrectText.text = _wordProcessingManager.NicifyWord(wordTriple.IncorrectWord);
            
            _NeutralCollider.OnTriggerEnterEvent.AddOnce(OnNeutralGuess);
            CurrentCorrectCollider.OnTriggerEnterEvent.AddOnce(OnCorrectGuess);
            CurrentIncorrectCollider.OnTriggerEnterEvent.AddOnce(OnIncorrectGuess);
        }

        /// <summary>
        /// Handles correct guess.
        /// </summary>
        private void OnCorrectGuess(Collider _)
        {
            _levelManager.IncrementCombo();
            _CorrectParticleSystem.transform.position = _levelManager.PlayerObject.transform.position;
            _CorrectParticleSystem.Play();

            SendRating(_currentTriple.PairWord);
        }

        /// <summary>
        /// Handles incorrect guess.
        /// </summary>
        private void OnIncorrectGuess(Collider _)
        {
            _levelManager.ResetCombo();
            _IncorrectParticleSystem.transform.position = _levelManager.PlayerObject.transform.position;
            _IncorrectParticleSystem.Play();

            SendRating(_currentTriple.IncorrectWord);
        }

        /// <summary>
        /// Handles neutral guess.
        /// </summary>
        private void OnNeutralGuess(Collider _)
        {
            _levelManager.DecrementCombo();
            _NeutralParticleSystem.transform.position = _levelManager.PlayerObject.transform.position;
            _NeutralParticleSystem.Play();

            SendRating("");
        }

        /// <summary>
        /// Calls database handler to record rating and increments rated times.
        /// </summary>
        private void SendRating(string answeredWord)
        {
            _wordProcessingManager.IncrementRatedWeight(_currentTriple.MainWord);
            _databaseHandler.RecordGameRating
            (
                _currentTriple.MainWord,
                answeredWord,
                _currentTriple.PairWord,
                _currentTriple.IncorrectWord,
                _currentTriple.WordAOA
            );
        }
    }
}