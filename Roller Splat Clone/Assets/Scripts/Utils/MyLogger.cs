using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Utils
{
    public class MyLogger : AbstractSingelton<MyLogger>

    {
        [SerializeField] private TextMeshProUGUI debugText;

        public void Log(string message)
        {
            //Debug.Log(message);
            debugText.text += message + "\n";
        }
    }
}