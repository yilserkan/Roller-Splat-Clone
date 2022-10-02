using System;
using Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace LevelSystem
{
    public class LevelGeneratorManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField heightInputField;
        [SerializeField] private TMP_InputField widthInputField;
        [SerializeField] private TMP_InputField cyclesInputField;

        private int m_Height;
        private int m_Width;
        private int m_Cycles;
        private int m_Seed = -1;

        private bool m_InputFieldsEnabled;

        private bool IsSeedSet => m_Seed != -1;
        private bool AllInputFieldsFilled => 
            heightInputField.text.Length != 0 && 
            widthInputField.text.Length != 0 &&
            cyclesInputField.text.Length != 0;
        public static event Action<Level> OnGenerateLevel;
        public static event Action OnResetTiles;

        private Level m_Levels;

        public void _GenerateLevel()
        {
            if (!AllInputFieldsFilled)
            {
                return;
            }
            
            OnResetTiles?.Invoke();

            ResetSeed();
            ReadInputFields();
            
            GetRandomSeed();
            CreateLevel();
        }

        public void _SaveLevel()
        {
            if (AllInputFieldsFilled && IsSeedSet)
            {
                JSONSaveSystem.SaveToJSON(m_Levels, true);
            }
        }
        
        public void _ReturnToMainMenu()
        {
            SceneManager.LoadScene(0);
        }

        private void ReadInputFields()
        {
            m_Height = Int32.Parse(heightInputField.text);
            m_Width = Int32.Parse(widthInputField.text);
            m_Cycles = Int32.Parse(cyclesInputField.text);
        }

        private void ResetSeed()
        {
            Random.InitState(Environment.TickCount);
        }

        private void GetRandomSeed()
        {
            m_Seed = Random.Range(0, 1000);
        }

        private void CreateLevel()
        {
            m_Levels = new Level(m_Height, m_Width, m_Cycles, m_Seed);
            OnGenerateLevel?.Invoke(m_Levels);
        }
    }
}