using System;
using System.Collections.Generic;
using System.Linq;
using Effects;
using GameUI;
using Json;
using MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

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
            MyLogger.Instance.Log("On Get Level Index");
            m_LevelIndex = PlayerPrefs.GetInt(LevelButton.PlayerPrefsCurrentLevelIndex, 0);
            MyLogger.Instance.Log("On Level Index : " + m_LevelIndex);
        }

        private void ReadLevelsFromJson()
        {
             m_Levels = JSONSaveSystem.ReadLevels();
             MyLogger.Instance.Log("Levels Read from Json");
        }
   
        private void GenerateNewLevel()
        {
            if (HasUnplayedLevels)
            {
                MyLogger.Instance.Log("Generating new level");
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
            LevelMainUIHandler.OnResetLevel += HandleOnResetLevel;
        }

        private void RemoveListeners()
        {
            // Grid.OnLevelFinished -= HandleOnLevelFinished;
            LevelFinishEffects.OnLoadNextLevel -= HandleOnLoadNextLevel;
            LevelMainUIHandler.OnResetLevel -= HandleOnResetLevel;
        }
    }
}
