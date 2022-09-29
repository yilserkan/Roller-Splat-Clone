using System;
using System.Collections;
using System.Collections.Generic;
using Effects;
using Json;
using MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;
using Grid = GridSystem.Grid;

namespace LevelSystem
{
    public class LevelManager : MonoBehaviour
    {
        private int m_LevelIndex = 0;
        
        private List<Level> m_Levels;
        public static event Action<Level> OnGenerateLevel;
        public static event Action OnResetTiles;
        private bool HasUnplayedLevels => m_LevelIndex < m_Levels.Count;
        private void ReadLevelsFromJson() => m_Levels = JSONSaveSystem.ReadFromJson<Level>();
        
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

        private void GenerateNewLevel()
        {
            if (HasUnplayedLevels)
            {
                Debug.Log("*------------------------- Level Finished");
                OnGenerateLevel?.Invoke(m_Levels[m_LevelIndex]);
                return;
            }
            // Go To Main Menu
            SceneManager.LoadScene(0);
        }
        // private void HandleOnLevelFinished()
        // {
        //     m_LevelIndex++;
        //     //GenerateNewLevel();
        // }
        private void HandleOnLoadNextLevel()
        {
            OnResetTiles?.Invoke();
            m_LevelIndex++;
            GenerateNewLevel();
        }
        private void AddListeners()
        {
            // Grid.OnLevelFinished += HandleOnLevelFinished;
            LevelFinishEffects.OnLoadNextLevel += HandleOnLoadNextLevel;
        }

        private void RemoveListeners()
        {
            // Grid.OnLevelFinished -= HandleOnLevelFinished;
            LevelFinishEffects.OnLoadNextLevel -= HandleOnLoadNextLevel;
        }
    }
}
