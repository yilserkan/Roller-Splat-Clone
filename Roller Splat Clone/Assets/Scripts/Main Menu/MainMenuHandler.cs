using System;
using System.Collections.Generic;
using Json;
using LevelSystem;
using UnityEngine;
using Utils;

namespace MainMenu
{
    public class MainMenuHandler : MonoBehaviour
    {
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Transform buttonParent;

        private List<Level> m_Levels;

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void HandleOnJsonInitialized()
        {
            ReadLevelsFromJson();
            CreateLevelButtons();
        }
        
        private void ReadLevelsFromJson()
        {
            m_Levels = JSONSaveSystem.ReadFromJson<Level>();
            // m_Levels = JSONSaveSystem.ReadLevels();
        }

        private void CreateLevelButtons()
        {
            for (int i = 0; i < m_Levels.Count; i++)
            {
                GameObject instansiatedButton = Instantiate(buttonPrefab, buttonParent);
                LevelButton levelButton = instansiatedButton.GetComponent<LevelButton>();

                levelButton.LevelIndex = i;
                levelButton.LevelText.text = $"LEVEL {i + 1}";

            }
        }
        
        public void _Generate()
        {
            SceneChanger.LoadScene(SceneChanger.LevelGeneratorScene);
        }

        private void AddListeners()
        {
            JsonInitializer.OnJsonInitialized += HandleOnJsonInitialized;
        }

        private void RemoveListeners()
        {
            JsonInitializer.OnJsonInitialized -= HandleOnJsonInitialized;
        }
    }
}