using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(menuName = "ScriptableObjects/PlayerSettings")]
    public class PlayerSettings : ScriptableObject
    {
        public float RaycastMultiplicator = 0.5f;
        public float MoveSpeed = 10;
        public float RotateAngle = 5;
        public float MaxDistance = 0.5f;
    }
}