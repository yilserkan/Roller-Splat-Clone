using System;
using System.Collections.Generic;
using Effects;
using GameUI;
using Json;
using MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelSystem
{
    public class LevelManager : MonoBehaviour
    {
        private int m_LevelIndex = 0;
        
        private List<Level> m_Levels;
        
        public static event Action<Level> OnGenerateLevel;
        public static event Action OnResetTiles;
        public static event Action<int> OnLevelIndexFound;
        
        private bool HasUnplayedLevels => m_LevelIndex < m_Levels.Count;

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void Start()
        {
            ReadCurrentLevelIndexFromPlayerPrefs();
            ReadLevelsFromJson();
            GenerateNewLevel();
        }

        private void ReadCurrentLevelIndexFromPlayerPrefs()
        {
            m_LevelIndex = PlayerPrefs.GetInt(LevelButton.PlayerPrefsCurrentLevelIndex, 0);
        }

        private void ReadLevelsFromJson()
        {
             // m_Levels = JSONSaveSystem.ReadLevels();
             m_Levels = JSONSaveSystem.ReadFromJson<Level>();
        }
   
        private void GenerateNewLevel()
        {
            if (HasUnplayedLevels)
            {
                OnLevelIndexFound?.Invoke(m_LevelIndex);
                Debug.Log("*------------------------- Level Finished");
                OnGenerateLevel?.Invoke(m_Levels[m_LevelIndex]);
                return;
            }
            // Go To Main Menu
            SceneManager.LoadScene(0);
        }

        private void HandleOnResetLevel()
        {
            OnGenerateLevel?.Invoke(m_Levels[m_LevelIndex]);
        }

        private void HandleOnLoadNextLevel()
        {
            OnResetTiles?.Invoke();
            m_LevelIndex++;
            GenerateNewLevel();
        }
        
        private void AddListeners()
        {
            LevelFinishEffects.OnLoadNextLevel += HandleOnLoadNextLevel;
            LevelMainUIHandler.OnResetLevel += HandleOnResetLevel;
        }

        private void RemoveListeners()
        {
            LevelFinishEffects.OnLoadNextLevel -= HandleOnLoadNextLevel;
            LevelMainUIHandler.OnResetLevel -= HandleOnResetLevel;
        }
    }
}
