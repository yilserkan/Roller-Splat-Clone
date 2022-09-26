using System;
using System.Collections;
using System.Collections.Generic;
using Json;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGeneratorUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField heightIF;
    [SerializeField] private TMP_InputField widthIF;
    [SerializeField] private TMP_InputField cyclesIF;

    private int m_Height;
    private int m_Width;
    private int m_Cycles;
    private int m_Seed;

    private bool m_InputFieldsEnabled;
    
    public static event Action<Level> OnGenerateLevel;
 
    private Level m_Levels;

    public void GenerateLevel()
    {
        Random.InitState((int)DateTime.Now.Ticks);
        
        m_Height = Int32.Parse(heightIF.text);
        m_Width = Int32.Parse(widthIF.text);
        m_Cycles = Int32.Parse(cyclesIF.text);
        m_Seed = Random.Range(0, 1000);

        m_Levels = new Level(m_Height,m_Width,m_Cycles,m_Seed);
        OnGenerateLevel?.Invoke(m_Levels);
    }

    public void SaveLevel()
    {

        JSONSaveSystem.SaveToJSON(m_Levels, true);
    }

    public void ToggleUI()
    {
        
    }
}
