using System;
using System.Collections;
using System.Collections.Generic;
using Json;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace LevelSystem
{
    public class LevelManager : MonoBehaviour
    {
        private int m_LevelIndex = 0;
        
        private List<Level> m_Levels;
        public static event Action<Level> OnGenerateLevel;

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
            ReadLevelsFromJson();
            GenerateNewLevel();
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
        }
        private void HandleOnLevelFinished()
        {
            m_LevelIndex++;
            //GenerateNewLevel();
        }
        private void AddListeners()
        {
            Grid.OnLevelFinished += HandleOnLevelFinished;
        }

        private void RemoveListeners()
        {
            Grid.OnLevelFinished -= HandleOnLevelFinished;
        }
    }
}
