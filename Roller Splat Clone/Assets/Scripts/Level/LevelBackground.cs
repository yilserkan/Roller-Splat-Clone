using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Grid = GridSystem.Grid;

namespace LevelSystem
{
    public class LevelBackground : MonoBehaviour
    {
        [SerializeField] private Material backgroundMaterial;
        
        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void HandleOnBackgroundColorSet(Color color)
        {
            backgroundMaterial.color = color;
        }
        
        private void AddListeners()
        {
            Grid.OnBackgorundColorSet += HandleOnBackgroundColorSet;
        }

        private void RemoveListeners()
        {
            Grid.OnBackgorundColorSet -= HandleOnBackgroundColorSet;
        }
    }
}