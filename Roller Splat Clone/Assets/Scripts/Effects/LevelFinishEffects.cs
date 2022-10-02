using System;
using System.Collections.Generic;
using GameUI;
using LevelSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Grid = GridSystem.Grid;

namespace Effects
{
    public class LevelFinishEffects : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelCompleteText;
        [SerializeField] private List<Image> images;
        [SerializeField] private Animation animation;

        private const string m_PlayLevelFinish = "Animation_LevelComplete";
        private const string m_PlayResetLevelFinish = "Animation_ResetLevelFinish";
        public static event Action OnLoadNextLevel;
        public static event Action OnResetTiles;
        
        private void OnEnable()
        {
            AddListeners();
        }
        
        private void Start()
        {
            PlayLevelResetAnimation();
        }
        private void OnDestroy()
        {
            RemoveListeners();
        }
        public void _TapToContinue()
        {
            OnResetTiles?.Invoke();
            OnLoadNextLevel?.Invoke();
            PlayLevelResetAnimation();
        }
        
        private void PlayLevelFinishAnimation()
        {
            animation.Play(m_PlayLevelFinish);
        }

        private void PlayLevelResetAnimation()
        {
            animation.Play(m_PlayResetLevelFinish);
        }
        
        private void ChangeImageColors(Color color)
        {
            for (int i = 0; i < images.Count; i++)
            {
                images[i].color = color;
            }
        }
        
        private void HandleOnLevelFinished()
        {
            PlayLevelFinishAnimation();
        }
        private void HandleOnColorSet(Color color)
        {
            ChangeImageColors(color);
        }
        
        private void HandleOnLevelIndexFound(int levelIndex)
        {
            int level = (levelIndex + 1);
            levelCompleteText.text = $"LEVEL {level}";
        }
        
        private void HandleOnResetLevel()
        {
            PlayLevelResetAnimation();
        }
        
        private void AddListeners()
        {
            Grid.OnLevelFinished += HandleOnLevelFinished;
            Grid.OnColorSet += HandleOnColorSet;
            LevelManager.OnLevelIndexFound += HandleOnLevelIndexFound;
            LevelMainUIHandler.OnResetLevel += HandleOnResetLevel;
        }
        
        private void RemoveListeners()
        {
            Grid.OnLevelFinished -= HandleOnLevelFinished;
            Grid.OnColorSet -= HandleOnColorSet;
            LevelManager.OnLevelIndexFound -= HandleOnLevelIndexFound;
            LevelMainUIHandler.OnResetLevel -= HandleOnResetLevel;
        }
    }
}