using TMPro;
using UnityEngine;

namespace Entities.DataManagement
{
    public class HighscoreItem : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Text _NickText;
        
        [SerializeField]
        private TMP_Text _ScoreText;

        public void SetData(int position, string nickname, float score, bool isPlayer)
        {
            _NickText.text = position + ". " + nickname + ":";
            _ScoreText.text = Mathf.RoundToInt(score).ToString();
            _NickText.color = isPlayer ? Color.green : Color.white;
        }
    }
}