using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Json
{
    public class JsonInitializer : MonoBehaviour
    {
        private const string JsonInitializePlayerPrefs = "JsonInitialized";

        public static event Action OnJsonInitialized;

        private void Start()
        {
            InitializeJsonFile();
        }

        private void InitializeJsonFile()
        {
            //PlayerPrefs.DeleteKey(JsonInitializePlayerPrefs);
            
            if (PlayerPrefs.GetInt(JsonInitializePlayerPrefs,0) == 0)
            {
                JSONSaveSystem.InitializeLevels();
                PlayerPrefs.SetInt(JsonInitializePlayerPrefs, 1);
            }
            
            OnJsonInitialized?.Invoke();
        }
    }
}