// BaseEffect.cs
using DG.Tweening;
using UnityEngine;

public abstract class BaseEffect : MonoBehaviour, IUIEffect
{
    [Header("Base Effect Settings")]
    public bool playOnStart = true;
    public bool waitForCompletion = true;

    [Header("Sequence Settings")]
    public float startDelay = 0f;
    public float effectDuration = 1f;

    // 接口实现
    public abstract bool IsPlaying { get; }
    public bool WaitForCompletion => waitForCompletion;

    protected Sequence currentTween;
    protected bool isInitialized = false;

    protected virtual void Awake()
    {
        InitializeEffect();
    }

    protected virtual void Start()
    {
        if (playOnStart)
            PlayEffect();
    }

    protected virtual void OnDestroy()
    {
        Cleanup();
    }

    protected virtual void OnEnable()
    {
        if (playOnStart && isInitialized)
            PlayEffect();
    }

    protected virtual void OnDisable()
    {
        if (playOnStart)
            StopEffect();
    }

    // 抽象方法 - 子类必须实现
    protected abstract void InitializeEffect();
    protected abstract void SetupTween();
    protected abstract void ResetEffect();

    // 公共方法
    public abstract void PlayEffect();
    public abstract void StopEffect();

    public virtual void RestartEffect()
    {
        StopEffect();
        PlayEffect();
    }

    public virtual float GetDuration()
    {
        return startDelay + effectDuration;
    }

    protected virtual void CreateBaseTween()
    {
        CleanupTween();

        currentTween = DOTween.Sequence();

        if (startDelay > 0f)
        {
            currentTween.AppendInterval(startDelay);
        }
    }

    protected virtual void CleanupTween()
    {
        if (currentTween != null)
        {
            currentTween.Kill();
            currentTween = null;
        }
    }

    protected virtual void Cleanup()
    {
        CleanupTween();
    }

    // 添加事件
    public System.Action<BaseEffect> OnEffectStarted;
    public System.Action<BaseEffect> OnEffectStopped;
    public System.Action<BaseEffect> OnEffectCompleted;

    protected virtual void InvokeEffectStarted()
    {
        OnEffectStarted?.Invoke(this);
    }

    protected virtual void InvokeEffectStopped()
    {
        OnEffectStopped?.Invoke(this);
    }

    protected virtual void InvokeEffectCompleted()
    {
        OnEffectCompleted?.Invoke(this);
    }
}