using UnityEngine;

namespace Player
{
    [CreateAssetMenu(menuName = "ScriptableObjects/PlayerSettings")]
    public class PlayerSettings : ScriptableObject
    {
        public float MoveSpeed = 10;
        public float RotateAngle = 5;
    }
}