using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class Test_Seed : MonoBehaviour
{
    [SerializeField] private int seed;

    private void Start()
    {
        LogRandomValues();
    }

    private void LogRandomValues()
    {
        Debug.Log(Random.Range(1,5));
        Debug.Log(Random.Range(1,5));
        Debug.Log(Random.Range(1,5));
    }

    private void InitSeedValue(int seed)
    {
        UnityEngine.Random.InitState(seed);
    }
}
