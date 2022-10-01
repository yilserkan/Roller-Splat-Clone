using System;
using Json;
using LevelSystem;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
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
        private int m_Seed;

        private bool m_InputFieldsEnabled;

        public static event Action<Level> OnGenerateLevel;
        public static event Action OnResetTiles;

        private Level m_Levels;

        public void _GenerateLevel()
        {
            //MyLogger.Instance.Log("On Generate Level");

            OnResetTiles?.Invoke();

            ResetSeed();
            ReadInputFields();
            GetRandomSeed();

            CreateLevel();
        }

        public void _SaveLevel()
        {

            JSONSaveSystem.SaveToJSON(m_Levels, true);
        }
        

        public void _ReturnToMainMenu()
        {
            SceneManager.LoadScene(0);
        }

        private void ReadInputFields()
        {
            //MyLogger.Instance.Log("On ReadInput Field Called");
            m_Height = Int32.Parse(heightInputField.text);
            m_Width = Int32.Parse(widthInputField.text);
            m_Cycles = Int32.Parse(cyclesInputField.text);
            //MyLogger.Instance.Log("On ReadInput Field Executed " + m_Width + " " + m_Height + " " + m_Cycles);
        }

        private void ResetSeed()
        {
            //MyLogger.Instance.Log("On Reset Seed Called");
            //Random.InitState((int)DateTime.Now.Ticks);
            Random.InitState(Environment.TickCount);
            //MyLogger.Instance.Log("On Reset Seed Executed + 10");
        }

        private void GetRandomSeed()
        {
            //MyLogger.Instance.Log("On Random Seed Called");
            m_Seed = Random.Range(0, 1000);
            //MyLogger.Instance.Log("On Random Seed Executed " +m_Seed);
        }

        private void CreateLevel()
        {
            m_Levels = new Level(m_Height, m_Width, m_Cycles, m_Seed);
            //MyLogger.Instance.Log("On Create Level Called " + m_Levels.Width + " " + m_Levels.Height + " " + m_Levels.Cycles + " " );
            OnGenerateLevel?.Invoke(m_Levels);
        }
    }
}