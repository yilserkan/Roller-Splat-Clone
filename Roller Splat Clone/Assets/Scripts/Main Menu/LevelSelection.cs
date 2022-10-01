using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainMenu
{
    public class LevelSelection : MonoBehaviour
    {
        [SerializeField] private Animation animation;
        
        private const string m_LevelClose = "Animation_CloseLevelSelect";
        
        public void _CloseLevelSelection()
        {
            animation.Play(m_LevelClose);
        }

        public void _DisableLevelSelectionGameobject()
        {
            gameObject.SetActive(false);
        }
    }
}