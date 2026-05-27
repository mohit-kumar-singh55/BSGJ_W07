using UnityEngine;
using UnityEngine.Pool;

public abstract class Pool<T> : MonoBehaviour where T : MonoBehaviour
{
    private ObjectPool<T> _pool;

    public ObjectPool<T> CurPool => _pool;

    private void Awake()
    {
        _pool = new(CreateObject, OnTakeFromPool, OnReturnToPool, OnDestroyFromPool, true, 5, 20);
    }

    protected abstract T CreateObject();

    protected abstract void OnTakeFromPool(T pooledObject);

    protected abstract void OnReturnToPool(T pooledObject);

    protected abstract void OnDestroyFromPool(T pooledObject);
}