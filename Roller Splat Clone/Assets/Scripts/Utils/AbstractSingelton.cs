using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public abstract class AbstractSingelton<T> : MonoBehaviour where T : Component
    {
        private static T m_Instance;

        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    T[] foundObjects = FindObjectsOfType<T>();
                    if (foundObjects.Length > 0)
                    {
                        m_Instance = foundObjects[0];

                        if (foundObjects.Length > 1)
                        {
                            Debug.Log($"There are multiple Singletons of type {nameof(T)}");
                        }
                    }

                    if (m_Instance == null)
                    {
                        var ob = new GameObject();
                        ob.name = nameof(T);

                        var singelton = ob.AddComponent<T>();
                        m_Instance = singelton;
                    }
                }

                return m_Instance;
            }
        }
    }
}