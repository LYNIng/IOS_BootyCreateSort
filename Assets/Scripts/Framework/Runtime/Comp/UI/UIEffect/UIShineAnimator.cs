// UIShineAnimator.cs
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIShineAnimator : BaseEffect
{
    [Header("Shine Settings")]
    public Color shineColor = Color.white;

    [Header("Animation Settings")]
    public float startValue = -0.5f;
    public float endValue = 1.5f;
    public Ease easeType = Ease.Linear;

    [Header("Start Time Settings")]
    public bool randomStartDelay = false;
    public Vector2 startDelayRange = new Vector2(0f, 2f);

    [Header("Interval Settings")]
    public bool randomInterval = false;
    public Vector2 intervalRange = new Vector2(1f, 3f);
    public float intervalTime = 0f;
    public bool useInterval = false;

    // 材质属性ID
    public static readonly int ShineColorId = Shader.PropertyToID("_ShineColor");
    public static readonly int ShineLocationId = Shader.PropertyToID("_ShineLocation");

    public Material shineMaterial { get; private set; }
    private Material originalMaterial;
    private Image targetImage;

    // 当前随机值
    private float currentStartDelay;
    private float currentInterval;

    public override bool IsPlaying => currentTween != null && currentTween.IsPlaying();

    protected override void InitializeEffect()
    {
        targetImage = GetComponent<Image>();
        if (targetImage == null)
        {
            Debug.LogError("UIShineAnimator requires an Image component!", this);
            return;
        }

        originalMaterial = targetImage.material;
        if (originalMaterial != null)
        {
            shineMaterial = new Material(originalMaterial);
            targetImage.material = shineMaterial;
        }
        else
        {
            Debug.LogError("Image material is missing!", this);
            return;
        }

        ResetEffect();
        isInitialized = true;
    }

    protected override void SetupTween()
    {
        // 生成随机时间
        GenerateRandomTimes();

        CreateBaseTween();

        // 使用当前随机开始延迟
        if (currentStartDelay > 0f)
        {
            currentTween.AppendInterval(currentStartDelay);
        }

        // 主光效动画
        currentTween.Append(
            shineMaterial.DOFloat(endValue, ShineLocationId, effectDuration)
                .SetEase(easeType)
        );

        // 使用当前随机间隔
        if (useInterval && currentInterval > 0f)
        {
            currentTween.AppendInterval(currentInterval);
        }

        // 重置位置并准备下一次随机
        currentTween.AppendCallback(() =>
        {
            if (shineMaterial != null)
            {
                shineMaterial.SetFloat(ShineLocationId, startValue);
            }

            // 为下一次循环生成新的随机时间
            if (currentTween != null && currentTween.IsPlaying())
            {
                GenerateRandomTimes();
            }
        });

        currentTween.SetLoops(-1, LoopType.Restart);
        currentTween.OnKill(() =>
        {
            currentTween = null;
            InvokeEffectStopped();
        });

        InvokeEffectStarted();
    }

    // 生成随机时间值
    private void GenerateRandomTimes()
    {
        // 开始延迟
        if (randomStartDelay)
        {
            currentStartDelay = Random.Range(startDelayRange.x, startDelayRange.y);
        }
        else
        {
            currentStartDelay = startDelay;
        }

        // 间隔时间
        if (randomInterval && useInterval)
        {
            currentInterval = Random.Range(intervalRange.x, intervalRange.y);
        }
        else
        {
            currentInterval = intervalTime;
        }
    }

    protected override void CreateBaseTween()
    {
        CleanupTween();

        currentTween = DOTween.Sequence();

        // 注意：这里不使用基类的startDelay，因为我们在SetupTween中自定义了延迟逻辑
    }

    protected override void ResetEffect()
    {
        if (shineMaterial != null)
        {
            shineMaterial.SetFloat(ShineLocationId, startValue);
            shineMaterial.SetColor(ShineColorId, Color.black);
        }
    }

    public override void PlayEffect()
    {
        if (!isInitialized || currentTween != null) return;

        shineMaterial.SetColor(ShineColorId, shineColor);
        shineMaterial.SetFloat(ShineLocationId, startValue);

        SetupTween();
    }

    public override void StopEffect()
    {
        CleanupTween();
        ResetEffect();
        InvokeEffectStopped();
    }

    protected override void Cleanup()
    {
        base.Cleanup();

        if (targetImage != null)
        {
            targetImage.material = originalMaterial;
        }

        if (shineMaterial != null)
        {
            Destroy(shineMaterial);
            shineMaterial = null;
        }
    }

    // 公共方法 - 随机时间控制
    public void SetRandomStartDelay(bool enable, float min = 0f, float max = 2f)
    {
        randomStartDelay = enable;
        if (enable)
        {
            startDelayRange = new Vector2(min, max);
        }

        if (IsPlaying) RestartEffect();
    }

    public void SetStartDelayRange(float min, float max)
    {
        startDelayRange = new Vector2(Mathf.Max(0f, min), Mathf.Max(min, max));
        if (IsPlaying) RestartEffect();
    }

    public void SetRandomInterval(bool enable, float min = 1f, float max = 3f)
    {
        randomInterval = enable;
        if (enable)
        {
            intervalRange = new Vector2(min, max);
        }

        if (IsPlaying) RestartEffect();
    }

    public void SetIntervalRange(float min, float max)
    {
        intervalRange = new Vector2(Mathf.Max(0f, min), Mathf.Max(min, max));
        if (IsPlaying) RestartEffect();
    }

    public void SetUseInterval(bool enable, bool random = false)
    {
        useInterval = enable;
        randomInterval = random;
        if (IsPlaying) RestartEffect();
    }

    // 获取当前随机值（用于调试）
    public float GetCurrentStartDelay() => currentStartDelay;
    public float GetCurrentInterval() => currentInterval;

    // 获取预估时间范围
    public Vector2 GetEstimatedCycleTimeRange()
    {
        float minCycleTime = effectDuration;
        float maxCycleTime = effectDuration;

        // 开始延迟
        if (randomStartDelay)
        {
            minCycleTime += startDelayRange.x;
            maxCycleTime += startDelayRange.y;
        }
        else
        {
            minCycleTime += startDelay;
            maxCycleTime += startDelay;
        }

        // 间隔时间
        if (useInterval)
        {
            if (randomInterval)
            {
                minCycleTime += intervalRange.x;
                maxCycleTime += intervalRange.y;
            }
            else
            {
                minCycleTime += intervalTime;
                maxCycleTime += intervalTime;
            }
        }

        return new Vector2(minCycleTime, maxCycleTime);
    }
}