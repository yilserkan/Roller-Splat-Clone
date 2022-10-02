using UnityEngine;

namespace MainMenu
{
    public class LevelSelection : MonoBehaviour
    {
        [SerializeField] private Animation animation;
        
        private const string m_LevelClose = "Animation_CloseLevelSelectv2";
        
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