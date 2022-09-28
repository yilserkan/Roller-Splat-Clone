using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    public void _Play()
    {
        SceneManager.LoadScene(1);
    }
    
    public void _Generate()
    {
        SceneManager.LoadScene(2);
    }
}
