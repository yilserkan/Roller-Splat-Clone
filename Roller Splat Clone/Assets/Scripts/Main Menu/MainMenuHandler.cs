using System.Collections.Generic;
using Json;
using LevelSystem;
using System;
using System.Linq;
using UnityEngine;
using Utils;

namespace MainMenu
{
    public class MainMenuHandler : MonoBehaviour
    {
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Transform buttonParent;

        private List<Level> m_Levels;

        private void Start()
        {
            //JSONSaveSystem.CheckFileExistence();
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

        public void _Play()
        {
            //SceneManager.LoadScene(1);
            //SceneChanger.LoadScene(SceneChanger.GameScene);
        }

        public void _Generate()
        {
            //SceneManager.LoadScene(2);
            SceneChanger.LoadScene(SceneChanger.LevelGeneratorScene);
        }
    }
}