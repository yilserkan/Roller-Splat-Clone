using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Json
{
    [Serializable]
    public class Wrapper<T>
    {
        public T[] Levels;
    }
}