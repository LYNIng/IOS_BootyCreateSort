using UnityEngine;


public abstract class Singleton<T> where T : Singleton<T>, new()
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
                _instance.OnAwake();
                if (_instance is IMsgObj)
                    MessageDispatch.BindMessage(_instance);
                _instance.RegistCommand();
            }
            return _instance;
        }
    }

    protected virtual void OnAwake() { }
    protected virtual void RegistCommand() { }
    protected virtual void UnRegistCommand() { }
    protected virtual void OnClear() { }
    public void Clear()
    {
        OnClear();
    }
}

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public abstract bool DontDestory { get; }

    private void Awake()
    {
        if (instance == null)
            instance = this as T;


        if (DontDestory)
        {
            if (transform.root != transform)
            {
                DontDestroyOnLoad(transform.root.gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        OnAwake();

    }

    private void OnEnable()
    {
        if (this is IMsgObj)
        {
            MessageDispatch.BindMessage(this);
        }
        RegistCommand();
    }

    private void OnDisable()
    {
        if (this is IMsgObj)
        {
            MessageDispatch.UnBindMessage(this);
        }
        UnRegistCommand();
    }

    private void OnDestroy()
    {

        BeforOnDestroy();
        instance = null;
        MountGameObject = null;
    }

    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<T>();

                if (MountGameObject != null)
                {
                    instance = MountGameObject.GetOrAddComponent<T>();
                }
                else if (instance == null)
                {
                    instance = new GameObject(typeof(T).ToString()).GetOrAddComponent<T>();
                    MountGameObject = instance.gameObject;
                }
            }
            return instance;
        }
    }

    public static GameObject MountGameObject { get; protected set; }

    protected virtual void OnAwake() { }
    protected virtual void BeforOnDestroy() { }
    protected virtual void RegistCommand() { }
    protected virtual void UnRegistCommand() { }

}

/// <summary>
/// 单例挂件
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class MonoCompositeSingleton<T> : MonoComposite where T : MonoBehaviour
{
    public abstract bool DontDestory { get; }

    private void Awake()
    {
        if (instance == null)
            instance = this as T;


        if (DontDestory)
        {
            if (transform.root != transform)
            {
                DontDestroyOnLoad(transform.root.gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        OnAwake();

    }

    private void OnEnable()
    {
        if (this is IMsgObj)
        {
            MessageDispatch.BindMessage(this);
        }
        RegistCommand();
    }

    private void OnDisable()
    {
        if (this is IMsgObj)
        {
            MessageDispatch.UnBindMessage(this);
        }
        UnRegistCommand();
    }

    private void OnDestroy()
    {

        BeforOnDestroy();
        instance = null;
        MountGameObject = null;
    }

    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<T>();

                if (MountGameObject != null)
                {
                    instance = MountGameObject.GetOrAddComponent<T>();
                }
                else if (instance == null)
                {
                    instance = new GameObject(typeof(T).ToString()).GetOrAddComponent<T>();
                    MountGameObject = instance.gameObject;
                }
            }
            return instance;
        }
    }

    public static GameObject MountGameObject { get; protected set; }

    protected virtual void _OnAwake_Internal() { }
    protected virtual void OnAwake() { }
    protected virtual void BeforOnDestroy() { }
    protected virtual void RegistCommand() { }
    protected virtual void UnRegistCommand() { }
}
