using TMPro;
using UnityEngine;

namespace Entities.DataManagement
{
    /// <summary>
    /// Highscore entry display in the highscore menu.
    /// </summary>
    public class HighscoreItem : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Text _NickText;
        
        [SerializeField]
        private TMP_Text _ScoreText;

        /// <summary>
        /// Updates UI.
        /// </summary>
        public void SetData(int position, string nickname, float score, bool isPlayer)
        {
            _NickText.text = position + ". " + nickname + ":";
            _ScoreText.text = Mathf.RoundToInt(score).ToString();
            _NickText.color = isPlayer ? Color.green : Color.white;
        }
    }
}