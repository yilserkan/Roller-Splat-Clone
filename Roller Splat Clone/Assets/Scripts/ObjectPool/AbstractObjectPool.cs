using UnityEngine;
using UnityEngine.Pool;
using Utils;

namespace ObjectPool
{
    public abstract class AbstractObjectPool<T, G> : AbstractSingelton<G>
        where T : AbstractObjectPoolObject<T>
        where G : Component
    {
        [SerializeField] private Transform poolParent;
        [SerializeField] private GameObject prefab;
        [SerializeField] private int poolDefaultCapacity;
        [SerializeField] private int poolMaxCapacity;

        private ObjectPool<T> m_Pool;

        private void Start()
        {
            m_Pool = new ObjectPool<T>(
                OnCreateObject,
                OnGetObject,
                OnReleaseObject,
                OnDestroyObject,
                true,
                poolDefaultCapacity,
                poolMaxCapacity
            );
        }

        public T OnObjectPool(Vector3 position)
        {
            var pooled = m_Pool.Get();
            pooled.transform.position = position;
            pooled.transform.parent = poolParent;
            
            return pooled;
        }

        private T OnCreateObject()
        {
            T instansiated = Instantiate(prefab).GetComponent<T>();

            if (instansiated != null)
            {
                instansiated.SetPool(m_Pool);
            }

            return instansiated;
        }

        private void OnGetObject(T obj)
        {
            obj.gameObject.SetActive(true);
        }

        private void OnReleaseObject(T obj)
        {
            obj.gameObject.SetActive(false);
        }

        private void OnDestroyObject(T obj)
        {
            Destroy(obj.gameObject);
        }
    }
}