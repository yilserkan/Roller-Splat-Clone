using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace ObjectPool
{
    public class TileColorParticleSpawner : AbstractObjectPool<TileColorParticle, TileColorParticleSpawner>
    {
        [SerializeField] private Material particleMaterial;

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }  
        
        private void HandleOnColorSet(Color color)
        {
            particleMaterial.SetColor("_TintColor", color);
        }
        
        private void AddListeners()
        {
            Grid.OnColorSet += HandleOnColorSet;
        }
        
        private void RemoveListeners()
        {
            Grid.OnColorSet -= HandleOnColorSet;
        }
        
    }
}