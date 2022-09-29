using UnityEngine.SceneManagement;

namespace Utils
{
    public static class SceneChanger 
    {
        public static int MainMenuScene = 0;
        public static int GameScene = 1;
        public static int LevelGeneratorScene = 2;
        
        public static void LoadScene(int levelIndex)
        {
            SceneManager.LoadScene(levelIndex);
        }
    }
}