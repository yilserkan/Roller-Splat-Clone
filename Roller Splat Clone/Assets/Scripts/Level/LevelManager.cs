using System;
using System.Collections;
using System.Collections.Generic;
using Json;
using UnityEngine;
using Grid = GridSystem.Grid;

public class LevelManager : MonoBehaviour
{
    private int m_LevelIndex = 0;
    private List<Level> m_Levels;
    
    public static event Action<Level> OnGenerateLevel;

    private void OnEnable()
    {
        Grid.OnLevelFinished += HandleOnLevelFinished;
    }

    private void OnDisable()
    {
        Grid.OnLevelFinished -= HandleOnLevelFinished;
    }

    private void Start()
    {
        m_Levels = JSONSaveSystem.ReadFromJson<Level>();
        Debug.Log("------------------Level Count;" + m_Levels.Count);
        GenerateNewLevel();
    }
    
    private void HandleOnLevelFinished()
    {
        m_LevelIndex++;
        GenerateNewLevel();
    }

    private void GenerateNewLevel()
    {
        if (m_LevelIndex < m_Levels.Count)
        {
            Debug.Log("*------------------------- Level Finished");
            OnGenerateLevel?.Invoke(m_Levels[m_LevelIndex]);
            return;
        }
        // Go To Main Menu
    }
    
}
