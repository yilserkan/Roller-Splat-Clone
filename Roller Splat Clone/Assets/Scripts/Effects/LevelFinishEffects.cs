using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Grid = GridSystem.Grid;

namespace Effects
{
    public class LevelFinishEffects : MonoBehaviour
    {
        [SerializeField] private List<Image> images;
        [SerializeField] private Animation animation;

        private static string PlayLevelFinish = "Animation_LevelComplete";
        private static string PlayResetLevelFinish = "Animation_ResetLevelFinish";
        
        public static event Action OnLoadNextLevel;
        public static event Action OnResetTiles;
        
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
            animation.Play(PlayResetLevelFinish);
        }

        public void _TapToContinue()
        {
            OnResetTiles?.Invoke();
            OnLoadNextLevel?.Invoke();
            animation.Play(PlayResetLevelFinish);
        }
        
        private void PlayLevelFinishAnimation()
        {
            animation.Play(PlayLevelFinish);
        }

        private void ChangeImageColors(Color color)
        {
            for (int i = 0; i < images.Count; i++)
            {
                images[i].color = color;
            }
        }
        
        private void HandleOnLevelFinished()
        {
            PlayLevelFinishAnimation();
        }
        private void HandleOnColorSet(Color color)
        {
            ChangeImageColors(color);
        }

        private void AddListeners()
        {
            Grid.OnLevelFinished += HandleOnLevelFinished;
            Grid.OnColorSet += HandleOnColorSet;
        }

        private void RemoveListeners()
        {
            Grid.OnLevelFinished -= HandleOnLevelFinished;
            Grid.OnColorSet -= HandleOnColorSet;
        }
        
    }
}