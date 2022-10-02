using System;
using UnityEngine;
using Utils;

namespace GameUI
{
    public class LevelMainUIHandler : MonoBehaviour
    {
        public static event Action OnResetLevel;
        
        public void _ResetLevel()
        {
            OnResetLevel?.Invoke();
        }

        public void _MainMenu()
        {
            SceneChanger.LoadScene(SceneChanger.MainMenuScene);
        }
    }
}