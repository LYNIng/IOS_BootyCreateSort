using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public class GameObjectPoolParent : MonoSingleton<GameObjectPoolParent>
{
    public override bool DontDestory => false;
}

public class GameObjectPool : IGameObjectPool
{
    public bool IsInited => _initState >= 0;
    public bool IsLoaded => _initState == 1;

    private ObjectPool<GameObject> _pool;
    private string _path;
    private GameObject _srcGo;

    private int _initState = -1;

    private AssetLoader<GameObject> assetLoader;

    public Transform PoolParent { get; set; }

    public GameObjectPool(string path, bool callInit = true)
    {
        _path = path;
        if (callInit) _ = AsyncInit();
    }
    public async Task AsyncInit()
    {
        if (_initState == 1)
        {
            return;
        }
        else if (_initState == 0)
        {
            await AsyncWaitLoaded();
            return;
        }
        _initState = 0;
        assetLoader = new AssetLoader<GameObject>(_path);
        await assetLoader.AsyncLoad();
        _initState = 1;
        _srcGo = assetLoader.Asset;
        _pool = new ObjectPool<GameObject>(() =>
        {
            var tmp = GameObject.Instantiate(_srcGo);
            return tmp;
        });
    }

    public GameObject Spawn(bool autoRelease = false, float releaseTime = 2f, Action<GameObject> onSet = null)
    {
        var result = _pool.Get();

        if (autoRelease)
        {
            var tc = result.GetOrAddComponent<TimeCounter>();
            tc.StartCounter(releaseTime, () => { _Back(result); });
        }
        onSet?.Invoke(result);
        result.gameObject.SetActive(true);
        return result;
    }

    private void _Back(GameObject get)
    {
        get.gameObject.SetActive(false);
        if (PoolParent != null)
        {
            get.transform.SetParent(PoolParent.transform);
        }
        _pool.Release(get);
    }

    public void Back(GameObject obj)
    {
        if (obj.TryGetComponent<TimeCounter>(out var tc))
        {
            tc.StopCounter();
        }
        _Back(obj);
    }

    public async Task AsyncWaitLoaded()
    {
        while (_initState != 1)
        {
            await Task.Yield();
        }
    }
}
