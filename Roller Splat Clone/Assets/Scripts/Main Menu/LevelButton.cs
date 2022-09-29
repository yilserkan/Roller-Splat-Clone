using TMPro;
using UnityEngine;
using Utils;

namespace MainMenu
{
    public class LevelButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;

        private int m_LevelIndex;
        
        public TextMeshProUGUI LevelText => levelText;

        public static string PlayerPrefsCurrentLevelIndex = "LevelIndex";
        
        public int LevelIndex
        {
            get
            {
                return m_LevelIndex;
            }
            set
            {
                m_LevelIndex = value;
            }
        }

        public void _OpenLevel()
        {
            PlayerPrefs.SetInt(PlayerPrefsCurrentLevelIndex, LevelIndex);
            SceneChanger.LoadScene(SceneChanger.GameScene);
        }
    }
}