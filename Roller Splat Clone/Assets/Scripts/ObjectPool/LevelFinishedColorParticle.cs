using ObjectPool;
using UnityEngine;

public class LevelFinishedColorParticle : AbstractObjectPoolObject<LevelFinishedColorParticle>
{
    [SerializeField] private float lifeTime = 1f;
    
    private void OnEnable()
    {
        Invoke(nameof(ReleaseObject),lifeTime);
    }
}
