using TMPro;
using UnityEngine;

namespace LevelSystem
{
    public class LevelText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void HandleOnLevelIndexFound(int levelIndex)
        {
            int level = (levelIndex + 1);
            levelText.text = $"LEVEL {level}";
        }
        
        private void AddListeners()
        {
            LevelManager.OnLevelIndexFound += HandleOnLevelIndexFound;
        }

        private void RemoveListeners()
        {
            LevelManager.OnLevelIndexFound -= HandleOnLevelIndexFound;
        }
    }
}