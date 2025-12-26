using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ResponsiveScaler : MonoBehaviour, IUIBaseInit
{
    [Header("参考尺寸")]
    [SerializeField] private Vector2 referenceResolution = new Vector2(1080, 1920);

    [Header("基础缩放模式")]
    [SerializeField] private ScaleMode scaleMode = ScaleMode.Fit;

    [Header("宽高屏自适应")]
    [SerializeField] private bool enableAspectRatioAdaptation = false;
    [SerializeField] private ScaleMode wideScreenMode = ScaleMode.Fit;
    [SerializeField] private ScaleMode tallScreenMode = ScaleMode.Fill;
    [SerializeField] private float aspectRatioThreshold = 1.0f; // 宽高比阈值

    [Header("缩放限制")]
    [SerializeField] private bool enableScaleLimits = false;
    [SerializeField] private Vector2 maxScale = Vector2.one * 2f;
    [SerializeField] private Vector2 minScale = Vector2.one * 0.5f;

    [Header("Canvas检测")]
    [SerializeField] private bool autoDetectCanvas = true;
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private CanvasScaler canvasScaler;

    private RectTransform rectTransform;
    private Vector2 originalSize;
    private Vector2 originalScale;

    public enum ScaleMode
    {
        Fit,        // 适应屏幕，保持宽高比
        Fill,       // 填充屏幕，保持宽高比
        Width,      // 基于宽度缩放
        Height,     // 基于高度缩放
        MatchWidth, // 匹配宽度（不考虑Canvas缩放）
        MatchHeight // 匹配高度（不考虑Canvas缩放）
    }

    private void Awake()
    {
        Init();
    }
    public void OnUIBaseInitBefore()
    {
        Init();
    }

    private void Init()
    {
        rectTransform = GetComponent<RectTransform>();
        originalSize = rectTransform.sizeDelta;
        originalScale = rectTransform.localScale;

        // 自动检测Canvas和CanvasScaler
        if (autoDetectCanvas)
        {
            targetCanvas = GetComponentInParent<Canvas>();
            if (targetCanvas != null)
            {
                canvasScaler = targetCanvas.GetComponent<CanvasScaler>();
            }
        }

    }

    private void Start()
    {
        ApplyScaling();
    }

    private void Update()
    {
        // 如果屏幕尺寸或Canvas缩放发生变化，重新应用缩放
        if (Screen.width != lastScreenWidth ||
            Screen.height != lastScreenHeight ||
            HasCanvasScaleChanged())
        {
            ApplyScaling();
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            lastCanvasScaleFactor = GetCanvasScaleFactor();
        }
    }

    private int lastScreenWidth;
    private int lastScreenHeight;
    private float lastCanvasScaleFactor;

    /// <summary>
    /// 获取Canvas的缩放因子
    /// </summary>
    private float GetCanvasScaleFactor()
    {
        if (targetCanvas != null)
        {
            return targetCanvas.scaleFactor;
        }
        return 1f;
    }

    /// <summary>
    /// 检查Canvas缩放是否发生变化
    /// </summary>
    private bool HasCanvasScaleChanged()
    {
        return Mathf.Abs(GetCanvasScaleFactor() - lastCanvasScaleFactor) > 0.01f;
    }

    /// <summary>
    /// 获取有效的参考分辨率（考虑CanvasScaler）
    /// </summary>
    private Vector2 GetEffectiveReferenceResolution()
    {
        if (canvasScaler != null)
        {
            switch (canvasScaler.uiScaleMode)
            {
                case CanvasScaler.ScaleMode.ConstantPixelSize:
                    return referenceResolution;

                case CanvasScaler.ScaleMode.ScaleWithScreenSize:
                    return canvasScaler.referenceResolution;

                case CanvasScaler.ScaleMode.ConstantPhysicalSize:
                    // 对于ConstantPhysicalSize，我们使用当前屏幕尺寸作为参考
                    return new Vector2(Screen.width, Screen.height);
            }
        }

        return referenceResolution;
    }

    /// <summary>
    /// 获取考虑Canvas缩放后的屏幕尺寸
    /// </summary>
    private Vector2 GetScaledScreenSize()
    {
        float canvasScale = GetCanvasScaleFactor();
        Vector2 scaledScreenSize = new Vector2(Screen.width, Screen.height) / canvasScale;

        return scaledScreenSize;
    }

    /// <summary>
    /// 应用等比缩放
    /// </summary>
    public void ApplyScaling()
    {
        if (rectTransform == null) return;

        Vector2 effectiveReferenceRes = GetEffectiveReferenceResolution();
        Vector2 scaledScreenSize = GetScaledScreenSize();
        Vector2 scaleFactor = CalculateScaleFactor(scaledScreenSize, effectiveReferenceRes);

        rectTransform.localScale = new Vector3(scaleFactor.x, scaleFactor.y, 1);
    }

    /// <summary>
    /// 根据屏幕宽高比选择合适的缩放模式
    /// </summary>
    private ScaleMode GetAdaptiveScaleMode(Vector2 currentScreenSize)
    {
        if (!enableAspectRatioAdaptation)
            return scaleMode;

        float aspectRatio = currentScreenSize.x / currentScreenSize.y;

        // 如果宽高比大于阈值，认为是宽屏
        if (aspectRatio > aspectRatioThreshold)
        {
            return wideScreenMode;
        }
        else // 否则认为是高屏
        {
            return tallScreenMode;
        }
    }

    /// <summary>
    /// 计算缩放因子
    /// </summary>
    private Vector2 CalculateScaleFactor(Vector2 currentScreenSize, Vector2 referenceRes)
    {
        Vector2 scale = Vector2.one;

        // 获取实际使用的缩放模式（考虑自适应）
        ScaleMode effectiveScaleMode = GetAdaptiveScaleMode(currentScreenSize);

        switch (effectiveScaleMode)
        {
            case ScaleMode.Fit:
                scale = CalculateFitScale(currentScreenSize, referenceRes);
                break;

            case ScaleMode.Fill:
                scale = CalculateFillScale(currentScreenSize, referenceRes);
                break;

            case ScaleMode.Width:
                scale.x = currentScreenSize.x / referenceRes.x;
                scale.y = scale.x; // 保持宽高比
                break;

            case ScaleMode.Height:
                scale.y = currentScreenSize.y / referenceRes.y;
                scale.x = scale.y; // 保持宽高比
                break;

            case ScaleMode.MatchWidth:
                scale.x = currentScreenSize.x / referenceRes.x;
                scale.y = 1f; // 只缩放宽度
                break;

            case ScaleMode.MatchHeight:
                scale.x = 1f; // 只缩放高度
                scale.y = currentScreenSize.y / referenceRes.y;
                break;
        }

        // 应用缩放限制
        if (enableScaleLimits)
        {
            scale.x = Mathf.Clamp(scale.x, minScale.x, maxScale.x);
            scale.y = Mathf.Clamp(scale.y, minScale.y, maxScale.y);
        }

        return scale;
    }

    /// <summary>
    /// 计算适应屏幕的缩放（保持宽高比，完整显示内容）
    /// </summary>
    private Vector2 CalculateFitScale(Vector2 currentScreenSize, Vector2 referenceRes)
    {
        float scaleX = currentScreenSize.x / referenceRes.x;
        float scaleY = currentScreenSize.y / referenceRes.y;

        // 取较小的缩放值，确保内容完全显示
        float scale = Mathf.Min(scaleX, scaleY);

        return new Vector2(scale, scale);
    }

    /// <summary>
    /// 计算填充屏幕的缩放（保持宽高比，可能裁剪内容）
    /// </summary>
    private Vector2 CalculateFillScale(Vector2 currentScreenSize, Vector2 referenceRes)
    {
        float scaleX = currentScreenSize.x / referenceRes.x;
        float scaleY = currentScreenSize.y / referenceRes.y;

        // 取较大的缩放值，确保填满屏幕
        float scale = Mathf.Max(scaleX, scaleY);

        return new Vector2(scale, scale);
    }

    /// <summary>
    /// 手动设置目标Canvas
    /// </summary>
    public void SetTargetCanvas(Canvas canvas)
    {
        targetCanvas = canvas;
        if (targetCanvas != null)
        {
            canvasScaler = targetCanvas.GetComponent<CanvasScaler>();
        }
        ApplyScaling();
    }

    /// <summary>
    /// 设置新的参考尺寸
    /// </summary>
    public void SetReferenceResolution(Vector2 newReferenceResolution)
    {
        referenceResolution = newReferenceResolution;
        ApplyScaling();
    }

    /// <summary>
    /// 设置缩放模式
    /// </summary>
    public void SetScaleMode(ScaleMode newScaleMode)
    {
        scaleMode = newScaleMode;
        ApplyScaling();
    }

    /// <summary>
    /// 设置宽高屏自适应参数
    /// </summary>
    public void SetAspectRatioAdaptation(bool enabled, ScaleMode wideMode, ScaleMode tallMode, float threshold = 1.0f)
    {
        enableAspectRatioAdaptation = enabled;
        wideScreenMode = wideMode;
        tallScreenMode = tallMode;
        aspectRatioThreshold = threshold;
        ApplyScaling();
    }

    /// <summary>
    /// 获取当前屏幕宽高比
    /// </summary>
    public float GetCurrentAspectRatio()
    {
        Vector2 scaledScreenSize = GetScaledScreenSize();
        return scaledScreenSize.x / scaledScreenSize.y;
    }

    /// <summary>
    /// 获取当前使用的缩放模式
    /// </summary>
    public ScaleMode GetCurrentScaleMode()
    {
        Vector2 scaledScreenSize = GetScaledScreenSize();
        return GetAdaptiveScaleMode(scaledScreenSize);
    }

    /// <summary>
    /// 强制刷新缩放
    /// </summary>
    [ContextMenu("强制刷新缩放")]
    public void ForceRefresh()
    {
        ApplyScaling();
    }

    /// <summary>
    /// 重置为原始尺寸
    /// </summary>
    [ContextMenu("重置尺寸")]
    public void ResetToOriginal()
    {
        rectTransform.sizeDelta = originalSize;
        rectTransform.localScale = originalScale;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 在Inspector中修改值时立即应用
        if (Application.isPlaying && rectTransform != null)
        {
            ApplyScaling();
        }
    }

    [UnityEditor.MenuItem("GameObject/UI/Responsive Scaler", false, 10)]
    static void CreateResponsiveScaler(UnityEditor.MenuCommand menuCommand)
    {
        GameObject go = new GameObject("Responsive Scaler");
        go.AddComponent<RectTransform>();
        go.AddComponent<ResponsiveScaler>();

        UnityEditor.GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        UnityEditor.Selection.activeObject = go;
    }


#endif
}