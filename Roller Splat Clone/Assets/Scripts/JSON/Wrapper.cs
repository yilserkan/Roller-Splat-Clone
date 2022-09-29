using System;

namespace Json
{
    [Serializable]
    public class Wrapper<T>
    {
        public T[] Levels;
    }
}