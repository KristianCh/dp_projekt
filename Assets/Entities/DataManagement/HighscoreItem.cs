using TMPro;
using UnityEngine;

namespace Entities.DataManagement
{
    public class HighscoreItem : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _PositionText;
        
        [SerializeField] 
        private TMP_Text _NickText;
        
        [SerializeField]
        private TMP_Text _ScoreText;

        public void SetData(int position, string nickname, float score)
        {
            _PositionText.text = position + ".";
            _NickText.text = nickname;
            _ScoreText.text = Mathf.RoundToInt(score).ToString();
        }
    }
}