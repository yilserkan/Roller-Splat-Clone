using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace ObjectPool
{
    public class AbstractObjectPoolObject<T> : MonoBehaviour
        where T : Component
    {
        private IObjectPool<T> m_Pool;

        public void SetPool(IObjectPool<T> pool) => m_Pool = pool;

        public void ReleaseObject()
        {
            m_Pool.Release(this as T);
        }
    }
}