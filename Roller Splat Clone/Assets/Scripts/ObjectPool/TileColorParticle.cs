using ObjectPool;
using UnityEngine;


public class TileColorParticle : AbstractObjectPoolObject<TileColorParticle>
{
    [SerializeField] private float lifeTime = 1f;
    
    private void OnEnable()
    {
        Invoke(nameof(ReleaseObject),lifeTime);
    }
}
