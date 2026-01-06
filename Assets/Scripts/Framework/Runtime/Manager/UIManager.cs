using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public partial class UIGlobal
{
    public static bool AutoSetAdaptation
    {
        get => true;
    }

    public static int UIResolution_Width
    {
        get
        {
            return DataManager.GetDataByInt("UIResolution_Width", 1080);
        }
        set
        {
            DataManager.SetDataByInt("UIResolution_Width", value);
        }
    }

    public static int UIResolution_Height
    {
        get
        {
            return DataManager.GetDataByInt("UIResolution_Height", 1920);
        }
        set
        {
            DataManager.SetDataByInt("UIResolution_Height", value);
        }
    }
}

public enum EBackgroundMask
{
    /// <summary>
    /// 没有遮挡
    /// </summary>
    None = 0x00,
    /// <summary>
    /// 有遮挡 黑色遮罩 透明度0.75
    /// </summary>
    Black_75F = 0x01,
    /// <summary>
    /// 有遮挡 黑色遮罩 透明度0.8
    /// </summary>
    Black_80F = 0x01 << 1,
    /// <summary>
    /// 有遮挡,透明的
    /// </summary>
    Transparency = 0x01 << 2,
    /// <summary>
    /// 点击背景会关闭UI
    /// </summary>
    CloseUIOnBackGroundClick = 0x01 << 3,
    /// <summary>
    /// 在ClickCancel的时候关闭UI
    /// </summary>
    CloseUIOnCancelClick = 0x01 << 4,
    /// <summary>
    /// 拦截ClickCancel的操作
    /// </summary>
    BlockCloseUIOnCancelClick = 0x01 << 5,
}

public enum EUIGroupTag
{
    None,

    GamePop = 0x1,

    HomePage = 0x1 << 1
}

[AttributeUsage(AttributeTargets.Class)]
public class UISettingAttribute : Attribute
{
    public UICanvasLayer DefaultLayer { get; private set; }
    public bool HideOnClose { get; private set; }

    public EBackgroundMask BackgroundMask { get; private set; }

    public EUIGroupTag UIGroupTag { get; private set; }
    public UISettingAttribute(UICanvasLayer defaultLayer,
        bool hideOnClose = false,
        EBackgroundMask backgroundMask = EBackgroundMask.None,
        EUIGroupTag UIGroupTag = EUIGroupTag.None)
    {
        DefaultLayer = defaultLayer;
        HideOnClose = hideOnClose;
        BackgroundMask = backgroundMask;
        this.UIGroupTag = UIGroupTag;
    }
}

/// <summary>
/// 弹窗用POP
/// 全屏遮挡可以用Overlay 或者 System
/// </summary>
public enum UICanvasLayer
{
    Default_Camera = 50 * 0,
    Background_Camera = 50 * 1,
    Main_Camera = 50 * 2,
    Popup_Camera = 50 * 3,
    Overlay_Camera = 50 * 4,
    System_Camera = 50 * 5,
    Top_Camera = 50 * 6,


    Default_Overlay = 50 * 7,      // 默认层
    Default_Global = 50 * 8, // 全局默认层

    Background_Overlay = 50 * 9,   // 背景层
    Background_Global = 50 * 10, // 全局背景层   

    Main_Overlay = 50 * 11,         // 主界面层
    Main_Global = 50 * 12, // 全局主界面层

    Popup_Overlay = 50 * 13,        // 弹窗层
    Popup_Global = 50 * 14, // 全局弹窗层

    Overlay_Overlay = 50 * 15,      // 顶部覆盖层（如提示、引导等）
    Overlay_Global = 50 * 16, // 全局顶部覆盖层（如提示、引导等）

    System_Overlay = 50 * 17,       // 系统层（如加载、全局遮罩等）
    System_Global = 50 * 18, // 全局系统层（如加载、全局遮罩等）

    Top_Overlay = 50 * 19,           // 最高层（如GM面板、调试等）
    Top_Global = 50 * 20, // 全局最高层（如GM面板、调试等）   
}

public abstract class UIData
{

}

[DisallowMultipleComponent]
public abstract class UIBase : MonoComposite
{
    public class IDs
    {
        public const string OPEN_ANIMATION = "Open";
        public const string CLOSE_ANIMATION = "Close";
        public const string IDLE_ANIMATION = "Idle";
    }

    public IPreloadAssetLoader PreloadAssetsLoader { get; protected set; }

    public enum UIState
    {
        None,
        Open,
        LockState,
        Hide,
        Close
    }

    public UICanvasLayer DefaultCanvasLayer { get; protected set; }

    public UICanvasLayer CanvasLayer { get; protected set; }

    protected UIState state { get; set; } = UIState.None;

    public bool IsOpen { get => state == UIState.Open; }

    public bool IsLockState { get; private set; } = false;

    public bool IsHide { get => state == UIState.Hide; }
    public bool HideOnClose { get; protected set; } = false;

    public EBackgroundMask BackgroundMask { get; protected set; }

    public EUIGroupTag UIGroupTag { get; protected set; }

    public event Action onClosed;

    private RectTransform _rectTransform;
    public RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
    }

    private object InteractionResult = null;
    private bool isInteraction = false;

    public virtual async Task OnAsyncPreload()
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// Data赋值之后的初始化时机
    /// </summary>
    public virtual void OnInit() { }
    public Transform GetCanvasLayerTransfrom()
    {
        return UIManager.GetCanvasLayerTransform(CanvasLayer);
    }
    public void SetCanvasLayer(UICanvasLayer layer)
    {
        if (CanvasLayer == layer) return;
        CanvasLayer = layer;
        var canvasTrans = UIManager.GetCanvasLayerTransform(layer);
        transform.SetParent(canvasTrans, false);

    }

    public void SetCanvasLayerByDefault()
    {
        SetCanvasLayer(DefaultCanvasLayer);
    }

    public async void Close()
    {
        await UIManager.CloseUIAsync(this);
    }
    public async Task AsyncClose()
    {
        await UIManager.CloseUIAsync(this);
    }

    public async Task OnShowAsync()
    {
        if (IsOpen) return;
        state = UIState.Open;
        gameObject.SetActive(false);
        await ShowBackgroundMask();
        gameObject.SetActive(true);
        await Show_Internal();
        PlayIdleState();
        OnShowed();
    }

    /// <summary>
    /// 这个方法是提供UIManager调用,
    /// 其他地方不要调用
    /// 如果要关闭请使用AsyncClose来关闭
    /// </summary>
    /// <returns></returns>
    public async Task<bool> OnCloseAsync()
    {
        if (!IsOpen) return false;
        if (IsLockState) return false;
        IsLockState = true;
        OnHideBefore();
        if (HideOnClose)
        {
            await Hide_Internal();
            await HideBackgroundMask();
            state = UIState.Hide;
            if (imaMask != null)
            {
                Destroy(imaMask.gameObject);
                imaMask = null;
            }
            gameObject.SetActive(false);
            onClosed?.Invoke();
            IsLockState = false;

            OnHideed();
            return false;
        }
        else
        {
            await Hide_Internal();
            await HideBackgroundMask();
            state = UIState.Close;
            Destroy(gameObject);
            onClosed?.Invoke();
            IsLockState = false;

            OnHideed();
            return true;
        }
    }

    private Image imaMask;
    private void DestroyMask()
    {
        if (imaMask != null)
        {
            Destroy(imaMask.gameObject);
            imaMask = null;
        }
    }
    private Image GetImaMask()
    {
        if (imaMask != null) return imaMask;
        var maskGO = new GameObject("MaskGO", typeof(RectTransform));
        maskGO.transform.SetParent(transform.parent);
        maskGO.transform.SetSiblingIndex(transform.GetSiblingIndex());
        maskGO.transform.localPosition = Vector3.zero;
        var maskRT = maskGO.gameObject.GetOrAddComponent<RectTransform>();
        maskRT.anchorMin = Vector2.zero;
        maskRT.anchorMax = Vector2.one;
        maskRT.offsetMax = Vector2.zero;
        maskRT.offsetMin = Vector2.zero;
        imaMask = maskGO.GetOrAddComponent<Image>();
        return imaMask;
    }

    private async Task ShowBackgroundMask()
    {
        if (BackgroundMask == EBackgroundMask.None)
        {
            await Task.CompletedTask;
        }
        else if ((BackgroundMask & EBackgroundMask.Black_75F) != 0)
        {
            imaMask = GetImaMask();
            imaMask.color = Color.white * 0.1f;
            await imaMask.SetFade(0f).DOFade(0.75f, 0.2f).AsyncWaitForCompletion();
        }
        else if ((BackgroundMask & EBackgroundMask.Black_80F) != 0)
        {
            imaMask = GetImaMask();
            imaMask.color = Color.white * 0.1f;
            await imaMask.SetFade(0f).DOFade(0.8f, 0.2f).AsyncWaitForCompletion();
        }
        else if ((BackgroundMask & EBackgroundMask.Transparency) != 0)
        {
            imaMask = GetImaMask();
            imaMask.color = Color.white * 0f;
        }

        if ((BackgroundMask & EBackgroundMask.CloseUIOnBackGroundClick) != 0
            && imaMask != null)
        {
            var btnMask = imaMask.GetOrAddComponent<Button>();
            var colorBlock = btnMask.colors;
            colorBlock.pressedColor = colorBlock.normalColor;
            btnMask.colors = colorBlock;
            btnMask.RegistBtnCallback(() =>
            {
                AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
                Close();
            });
        }


    }

    private async Task HideBackgroundMask()
    {
        if (BackgroundMask == EBackgroundMask.None)
        {
            await Task.CompletedTask;
        }
        else if ((BackgroundMask & EBackgroundMask.Black_75F) != 0)
        {
            await imaMask.DOFade(0f, 0.2f).AsyncWaitForCompletion();
        }
        else if ((BackgroundMask & EBackgroundMask.Black_80F) != 0)
        {
            await imaMask.DOFade(0f, 0.2f).AsyncWaitForCompletion();
        }
        else if ((BackgroundMask & EBackgroundMask.Transparency) != 0)
        {
            await Task.CompletedTask;
        }
    }

    protected virtual void PlayIdleState()
    {
        _ = PlayAnimationByNameSuffix($"idle");
    }

    protected virtual async Task Show_Internal()
    {
        await PlayAnimationByNameSuffix($"open", onFail: PlayShow_ScalePop);
    }

    protected virtual async Task Hide_Internal()
    {
        await PlayAnimationByNameSuffix($"close", onFail: PlayHide_ScalePop);
    }
    /// <summary>
    /// 注册相关的操作可以放在这里
    /// </summary>
    protected virtual void OnShowed() { }
    /// <summary>
    /// 有需要反注册的操作可以放在这里
    /// </summary>
    protected virtual void OnHideed() { }

    protected virtual void OnHideBefore() { }

    private void OnDestroy()
    {
        if (imaMask != null)
        {
            Destroy(imaMask.gameObject);
            imaMask = null;
        }
        UIManager.UIDestroy(this);

    }

    public bool TryGetPreloadAsset<T>(string assetName, out T resultAsset) where T : UnityEngine.Object
    {
        if (PreloadAssetsLoader != null && PreloadAssetsLoader.TryGetPreloadAsset(assetName, out resultAsset))
        {

            return true;
        }
        resultAsset = null;
        return false;
    }

    /// <summary>
    /// 执行触发交互
    /// </summary>
    /// <param name="Interaction"></param>
    public void ExcuteInteraction(object Interaction = null)
    {
        InteractionResult = Interaction;
        isInteraction = true;
    }

    /// <summary>
    /// 等待发生交互
    /// </summary>
    /// <returns></returns>
    public virtual async Task<object> WaitInteraction()
    {
        while (!isInteraction)
        {
            await Task.Yield();
        }
        isInteraction = false;
        return InteractionResult;
    }

    public virtual async Task<T> WaitInteraction<T>()
    {
        var tmp = await WaitClose();
        if (tmp is T result) return result;
        else return default;
    }

    public virtual async Task<object> WaitClose()
    {
        while (IsOpen)
            await Task.Yield();
        return null;
    }

    public async Task<T> WaitClose<T>()
    {
        var tmp = await WaitClose();
        if (tmp is T result) return result;
        else return default;
    }

    public Vector2 WorldToAnchoredPosition(Vector3 worldPosition)
    {
        Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
        if (canvas == null) return Vector2.zero;
        Camera camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main;
        Debug.Log(RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition));
        // 将世界坐标转换为 Viewport 坐标
        Vector2 viewportPosition = camera != null
            ? camera.WorldToViewportPoint(worldPosition)
            : RectTransformUtility.WorldToScreenPoint(null, worldPosition);

        // 将 Viewport 坐标转换为 anchoredPosition
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            viewportPosition,
            camera,
            out anchoredPosition
        );
        return anchoredPosition;
    }

    protected async Task PlayShow_Fade()
    {
        await transform.SetFade(0f).DOFade(1f, 0.2f).AsyncWaitForCompletion();
    }
    protected async Task PlayHide_Fade()
    {
        await gameObject.GetOrAddComponent<CanvasGroup>().DOFade(0, 0.2f).AsyncWaitForCompletion();
    }

    protected async Task PlayShow_ScalePop()
    {
        var ox = transform.localScale.x;
        transform.SetLocalScale(0);
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(ox * 1.1f, 0.15f).SetEase(Ease.Linear));
        seq.Append(transform.DOScale(ox, 0.1f).SetEase(Ease.Linear));
        await seq.AsyncWaitForCompletion();
    }

    protected async Task PlayHide_ScalePop()
    {
        var ox = transform.localScale.x;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(ox * 1.1f, 0.1f).SetEase(Ease.Linear));
        seq.Append(transform.DOScale(0, 0.15f).SetEase(Ease.Linear));
        await seq.AsyncWaitForCompletion();
    }

    protected async Task PlayShow_ScaleLessenAndFadeIn()
    {
        var ox = transform.localScale.x;
        transform.SetLocalScale(ox * 1.1f);
        var cg = transform.SetFade(0);
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(ox, 0.2f).SetEase(Ease.Linear));
        seq.Join(cg.DOFade(1f, 0.2f).SetEase(Ease.Linear));
        await seq.AsyncWaitForCompletion();
    }

    protected async Task PlayHide_ScaleMagnifyFadeOut()
    {
        var ox = transform.localScale.x;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(ox * 1.1f, 0.2f).SetEase(Ease.Linear));
        seq.Join(gameObject.GetOrAddComponent<CanvasGroup>().DOFade(0, 0.2f).SetEase(Ease.Linear));
        await seq.AsyncWaitForCompletion();
    }

    protected async Task PlayShow_OverMoveFromTop()
    {
        var rt = transform.GetComponent<RectTransform>();
        var canvas = GetComponentInParent<Canvas>();
        var canvasScale = canvas.GetComponent<CanvasScaler>();
        var rtCanvas = canvas.GetComponent<RectTransform>();
        var oy = rt.anchoredPosition.y;
        rt.SetAnchoredPositionY(rt.anchoredPosition.y + rtCanvas.sizeDelta.y);
        var dt = (rtCanvas.sizeDelta.y / canvasScale.referenceResolution.y) * 0.2f;
        Sequence seq = DOTween.Sequence();
        seq.Append(rt.DOAnchorPosY(oy - 100, dt).SetEase(Ease.Linear));
        seq.Append(rt.DOAnchorPosY(oy, 0.15f).SetEase(Ease.Linear));

        await seq.AsyncWaitForCompletion();
    }

    protected async Task PlayHide_OverMoveToTop()
    {
        var rt = transform.GetComponent<RectTransform>();
        var canvas = GetComponentInParent<Canvas>();
        var canvasScale = canvas.GetComponent<CanvasScaler>();
        var rtCanvas = canvas.GetComponent<RectTransform>();
        var toY = rt.anchoredPosition.y + rtCanvas.sizeDelta.y;
        var dt = (rtCanvas.sizeDelta.y / canvasScale.referenceResolution.y) * 0.2f;

        Sequence seq = DOTween.Sequence();
        seq.Append(rt.DOAnchorPosY(rt.anchoredPosition.y - 100, 0.15f).SetEase(Ease.Linear));
        seq.Append(rt.DOAnchorPosY(toY, dt).SetEase(Ease.Linear));

        await seq.AsyncWaitForCompletion();
    }

    protected async Task PlayShow_OverMoveBySizeY()
    {
        var rt = transform.GetComponent<RectTransform>();
        await PlayShow_OverMoveY(rt.sizeDelta.y, 0.2f);
    }

    protected async Task PlayHide_OverMoveBySizeY()
    {
        var rt = transform.GetComponent<RectTransform>();
        await PlayHide_OverMoveY(rt.sizeDelta.y, 0.2f);
    }

    protected async Task PlayShow_OverMoveY(float offsetY, float dt)
    {
        var rt = transform.GetComponent<RectTransform>();
        var canvas = GetComponentInParent<Canvas>();
        var canvasScale = canvas.GetComponent<CanvasScaler>();
        var rtCanvas = canvas.GetComponent<RectTransform>();
        var oy = rt.anchoredPosition.y;
        //Debug.Log(rt.anchoredPosition.y + offsetY);
        rt.SetAnchoredPositionY(rt.anchoredPosition.y + offsetY);

        Sequence seq = DOTween.Sequence();
        seq.Append(rt.DOAnchorPosY(oy - 50, dt).SetEase(Ease.Linear));
        seq.Append(rt.DOAnchorPosY(oy, 0.15f).SetEase(Ease.Linear));

        await seq.AsyncWaitForCompletion();
    }

    protected async Task PlayHide_OverMoveY(float offsetY, float dt)
    {
        var rt = transform.GetComponent<RectTransform>();
        var canvas = GetComponentInParent<Canvas>();
        var canvasScale = canvas.GetComponent<CanvasScaler>();
        var rtCanvas = canvas.GetComponent<RectTransform>();

        var oy = rt.anchoredPosition.y;
        //rt.SetAnchoredPositionY(rt.anchoredPosition.y + offsetY);
        Sequence seq = DOTween.Sequence();
        seq.Append(rt.DOAnchorPosY(oy - 50, 0.15f).SetEase(Ease.Linear));
        seq.Append(rt.DOAnchorPosY(oy + offsetY, dt).SetEase(Ease.Linear));

        await seq.AsyncWaitForCompletion();
    }
    protected async Task PlayShow_OffsetMoveBySizeY()
    {
        var rt = transform.GetComponent<RectTransform>();
        await PlayShow_OffsetMoveY(rt.sizeDelta.y, 0.2f);
    }

    protected async Task PlayHide_OffsetMoveBySizeY()
    {
        var rt = transform.GetComponent<RectTransform>();
        await PlayHide_OffsetMoveY(rt.sizeDelta.y, 0.2f);
    }

    protected async Task PlayShow_OffsetMoveY(float offsetY, float dt)
    {
        var rt = transform.GetComponent<RectTransform>();
        var canvas = GetComponentInParent<Canvas>();
        var canvasScale = canvas.GetComponent<CanvasScaler>();
        var rtCanvas = canvas.GetComponent<RectTransform>();
        var oy = rt.anchoredPosition.y;
        //Debug.Log(rt.anchoredPosition.y + offsetY);
        rt.SetAnchoredPositionY(rt.anchoredPosition.y + offsetY);

        Sequence seq = DOTween.Sequence();
        //seq.Append(rt.DOAnchorPosY(oy - 50, dt).SetEase(Ease.Linear));
        seq.Append(rt.DOAnchorPosY(oy, 0.15f).SetEase(Ease.Linear));

        await seq.AsyncWaitForCompletion();
    }

    protected async Task PlayHide_OffsetMoveY(float offsetY, float dt)
    {
        var rt = transform.GetComponent<RectTransform>();
        var canvas = GetComponentInParent<Canvas>();
        var canvasScale = canvas.GetComponent<CanvasScaler>();
        var rtCanvas = canvas.GetComponent<RectTransform>();

        var oy = rt.anchoredPosition.y;
        //rt.SetAnchoredPositionY(rt.anchoredPosition.y + offsetY);
        Sequence seq = DOTween.Sequence();
        //seq.Append(rt.DOAnchorPosY(oy - 50, 0.15f).SetEase(Ease.Linear));
        seq.Append(rt.DOAnchorPosY(oy + offsetY, dt).SetEase(Ease.Linear));

        await seq.AsyncWaitForCompletion();
    }

    protected async Task Play_MoveYTo(float posY, float dt)
    {
        var rt = transform.GetComponent<RectTransform>();

        Sequence seq = DOTween.Sequence();

        seq.Append(rt.DOAnchorPosY(posY, dt).SetEase(Ease.Linear));

        await seq.AsyncWaitForCompletion();
    }


    /// <summary>
    /// 需要Clip和State的名字相同
    /// </summary>
    /// <param name="stateName"></param>
    /// <param name="layerIndex"></param>
    /// <returns></returns>
    protected async Task PlayAnimatorState(string stateName, int layerIndex = 0, Func<Task> onFail = null)
    {
        if (gameObject.TryGetComponent<Animator>(out var animator)
            && animator.HasState(layerIndex, Animator.StringToHash(stateName))
            && animator.TryAnimationClip(stateName, out var resultClip))
        {
            var clipLength = resultClip.length;
            animator.Play(stateName);

            await Task.Delay((int)(clipLength * 1000));
        }
        else
        {
            if (onFail != null)
                await onFail.Invoke();
        }
    }

    protected async Task PlayAnimationByNameSuffix(string nameSuffix, Func<Task> onFail = null)
    {
        if (gameObject.TryGetComponent<Animation>(out var animation)
            && animation.PlayAnimationByNameSuffix(nameSuffix, out var clipLength))
        {
            await Task.Delay(Mathf.RoundToInt(clipLength * 1000));
        }
        else if (onFail != null)
        {
            await onFail.Invoke();
        }
    }

    protected async Task PlayAnimation(string clipName, Func<Task> onFail = null)
    {
        if (gameObject.TryGetComponent<Animation>(out var animation))
        {
            var clip = animation.GetClip(clipName);
            if (clip != null)
            {
                animation.Play(clipName);
                await Task.Delay(Mathf.FloorToInt(clip.length * 1000));
                return;
            }
            Debug.Log($"{clipName} 没找到,请检查是否命名错误?");
        }
        if (onFail != null)
            await onFail.Invoke();
    }


}

public abstract class UIBase<T> : UIBase where T : UIData
{
    public T Data { get; protected set; }

    public void ReplaceData(T data)
    {
        this.Data = data;
        _ = OnReplaceData();
    }

    public async Task AsyncReplaceData(T data)
    {
        this.Data = data;
        await OnReplaceData();
    }

    protected virtual async Task OnReplaceData()
    {
        await Task.CompletedTask;
    }


}

/// <summary>
/// 给其他组件提供的UIBase Init的时机
/// </summary>
public interface IUIBaseInit
{
    void OnUIBaseInitBefore();
}

public class UIManager : MonoSingleton<UIManager>, IManager, IManagerInit, IMsgObj
{
    private static Camera _uiCamera;
    /// <summary>
    /// 切换场景的时候需要考虑是否要更新
    /// </summary>
    public static Camera UICamera
    {
        get
        {
            if (_uiCamera == null)
            {
                var uicGo = GameObject.FindGameObjectWithTag("UICamera");
                if (uicGo != null)
                {
                    _uiCamera = uicGo.GetComponent<Camera>();
                }
            }
            return _uiCamera;
        }
    }

    private const string UIPrefabsPath = "Prefabs/UI/{0}.prefab";

    public const string DefaultToastPath = "Prefabs/UI/UIToast.prefab";

    private static Dictionary<Type, UIBase> UIDict = new Dictionary<Type, UIBase>();

    private static Dictionary<UICanvasLayer, Transform> canvasLayerDict = new Dictionary<UICanvasLayer, Transform>();

    private static AssetLoader<GameObject> canvasLoader;

    private class UIQueueItem
    {
        public Type UIType;
        public UIData UIData;

    }

    private static List<UIQueueItem> UIQueue = new List<UIQueueItem>();

    private static List<UIBase> CloseUIOnClickCancelQueue = new List<UIBase>();

    public override bool DontDestory => true;

    public bool inited { get; private set; } = false;

    public static bool IsUIOpen<T>()
    {
        return IsUIOpen(typeof(T));
    }

    public static bool IsUIOpen(Type uiType)
    {
        return UIDict.TryGetValue(uiType, out var ui) && ui.IsOpen;
    }

    public static T GetUI<T>() where T : UIBase
    {
        TryGetUI(out T result);
        return result;
    }

    public static bool TryGetUI<T>(out T result) where T : UIBase
    {
        if (TryGetUI(typeof(T), out UIBase ui) && ui is T tmp)
        {
            result = tmp;
            return true;
        }
        result = null;
        return false;
    }

    public static bool TryGetUI(Type uiType, out UIBase result)
    {
        if (UIDict.TryGetValue(uiType, out var ui))
        {
            result = ui;
            return true;
        }
        result = null;
        return false;
    }

    public static async void OpenUI<T>(UIData uiData = null, Action<T> onResult = null) where T : UIBase
    {
        var ui = await OpenUIAsync<T>(uiData);
        onResult?.Invoke(ui);
    }
    public static async void OpenUI(Type uiType, UIData uiData = null, Action<UIBase> onResult = null)
    {
        var ui = await OpenUIAsync(uiType, uiData);
        onResult?.Invoke(ui);
    }

    public static async Task OpenMultiUIAsync(params Type[] uiType)
    {
        Task[] tasks = new Task[uiType.Length];

        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = OpenUIAsync(uiType[i]);
        }

        await Task.WhenAll(tasks);
    }

    public static async Task<T> OpenUIAsync<T>(UIData uiData = null) where T : UIBase
    {
        return await OpenUIAsync(typeof(T), uiData) as T;
    }

    /// <summary>
    /// 打开UI
    /// </summary>
    /// <param name="uiType"></param>
    /// <param name="uiData"></param>
    /// <returns></returns>
    public static async Task<UIBase> OpenUIAsync(Type uiType, UIData uiData = null)
    {
        try
        {
            if (UIDict.TryGetValue(uiType, out var ui))
            {
                MessageDispatch.BindMessage(ui);
                _OnShowBefore(ui);
                await ui.OnShowAsync();
                return ui;
            }

            var uiName = uiType.Name;
            var uiPath = string.Format(UIPrefabsPath, uiName);

            UICanvasLayer defaultCanvasLayer = UICanvasLayer.Default_Overlay;
            EBackgroundMask eBackgroundMask = EBackgroundMask.None;
            EUIGroupTag uigroupTag = EUIGroupTag.None;
            if (uiType.TryGetCustomAttribute(out UISettingAttribute result))
            {
                defaultCanvasLayer = result.DefaultLayer;
                eBackgroundMask = result.BackgroundMask;
                uigroupTag = result.UIGroupTag;
            }
            Transform canvasRoot = GetCanvasLayerTransform(defaultCanvasLayer);
            var preloader = await AssetsManager.AsyncCreatePreloadAssetsLoader(uiType);
            var resultGO = await AssetsManager.AsyncInstantiate(uiPath);
            resultGO.transform.SetParent(canvasRoot, false);
            resultGO.transform.SetAsLastSibling();
            var uiBase = resultGO.GetComponent<UIBase>();
            if (uiBase == null)
            {
                preloader.Unload();
                throw new Exception($"OpenUI Error: {uiName} does not have a UIBase component.");
            }

            var defCLPro = uiType.GetProperty("DefaultCanvasLayer", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (defCLPro != null) defCLPro.SetValue(uiBase, defaultCanvasLayer);
            var CLPro = uiType.GetProperty("CanvasLayer", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (CLPro != null) CLPro.SetValue(uiBase, defaultCanvasLayer);
            var bgMaskPro = uiType.GetProperty("BackgroundMask", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (bgMaskPro != null) bgMaskPro.SetValue(uiBase, eBackgroundMask);
            var UIGroupTagPro = uiType.GetProperty("UIGroupTag", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (UIGroupTagPro != null) UIGroupTagPro.SetValue(uiBase, uigroupTag);

            AssetsManager.SetPreloadAssetLoaderToObj(uiBase, preloader);

            BindData(uiBase, uiData);
            MessageDispatch.BindMessage(uiBase);

            UIDict.Add(uiType, uiBase);

            OnUIBaseInitBefore(uiBase);

            uiBase.OnInit();
            _OnShowBefore(uiBase);
            await uiBase.OnShowAsync();
            return uiBase;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return null;
        }
    }

    public static void OpenUIByQueue<T>(UIData uiData = null)
    {
        UIQueue.Enqueue(new UIQueueItem { UIType = typeof(T), UIData = uiData });
        if (UIQueue.Count == 1)
        {
            OpenUI(typeof(T), uiData);
        }
    }
    public static async void CloseUI(UIBase ui)
    {
        await CloseUIAsync(ui.GetType());
    }
    public static async void CloseUI(Type type)
    {
        await CloseUIAsync(type);
    }
    public static async void CloseUI<T>()
    {
        await CloseUIAsync<T>();
    }

    public static async Task CloseMultiUIAsync(Type[] uiType)
    {
        Task[] tasks = new Task[uiType.Length];
        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = CloseUIAsync(uiType[i]);
        }

        await Task.WhenAll(tasks);
    }
    public static async Task CloseMultiUIAsyncByUIGroupTag(EUIGroupTag tag)
    {
        List<Task> tasks = null; ;
        foreach (var kvp in UIDict)
        {
            var uibase = kvp.Value;
            if ((uibase.UIGroupTag & tag) != 0)
            {
                if (tasks == null)
                    tasks = new List<Task>();
                tasks.Add(uibase.AsyncClose());
            }
        }
        if (tasks != null)
            await Task.WhenAll(tasks);
    }
    public static async Task CloseUIAsync(UIBase uiBase)
    {
        if (toastUIList != null)
        {
            if (toastUIList.Remove(uiBase))
            {
                var removeFlag = await uiBase.OnCloseAsync();
                if (removeFlag)
                {
                    UIDestroy(uiBase);
                }
                return;
            }
        }


        await CloseUIAsync(uiBase.GetType());
    }

    public static async Task CloseUIAsync<T>()
    {
        await CloseUIAsync(typeof(T));
    }
    /// <summary>
    /// 关闭UI
    /// </summary>
    /// <param name="uiType"></param>
    /// <returns></returns>
    public static async Task CloseUIAsync(Type uiType)
    {
        if (UIDict.TryGetValue(uiType, out var uiBase))
        {
            MessageDispatch.UnBindMessage(uiBase);

            _OnCloseBefore(uiBase);
            var removeFlag = await uiBase.OnCloseAsync();
            if (removeFlag)
            {
                UIDestroy(uiBase);
            }

            if (UIQueue.TryPeekForward(out UIQueueItem item))
            {
                if (uiType == item.UIType)
                {
                    UIQueue.Dequeue();

                    if (UIQueue.TryPeekForward(out UIQueueItem item2))
                    {
                        await OpenUIAsync(item2.UIType, item2.UIData);
                    }
                }
            }
        }
    }

    public static Transform GetCanvasLayerTransform(UICanvasLayer layer)
    {
        if (canvasLayerDict.TryGetValue(layer, out var layerTransform))
        {
            return layerTransform;
        }
        else
        {
            var canvasGo = canvasLoader.Asset.SpawnNewOne();
            var canvas = canvasGo.GetComponent<Canvas>();
            canvasGo.name = layer.ToString();
            if (layer.IsCamera())
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = UICamera == null ? Camera.main : UICamera;
                canvas.planeDistance = 10;
            }
            else
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }

            SetCanvasScaler(canvas);

            if (layer.IsGlobal())
            {
                DontDestroyOnLoad(canvasGo);
            }

            canvas.sortingOrder = (int)layer;
            if (canvasGo.TryGetComponent<GraphicRaycaster>(out var GRComp))
            {
                GRComp.enabled = UIInteractionState;
            }

            layerTransform = canvasGo.transform;
            canvasLayerDict.Add(layer, layerTransform);
        }

        return layerTransform;
    }

    public static void SetCanvasScaler(Canvas canvas)
    {
        var canvasScaler = canvas.gameObject.GetOrAddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(UIGlobal.UIResolution_Width, UIGlobal.UIResolution_Height);
        if (UIGlobal.AutoSetAdaptation)
        {
            var t = (float)Screen.width / (float)Screen.height;
            if (t > 0.6f)
            {
                canvasScaler.matchWidthOrHeight = 1f;
            }
            else
            {
                canvasScaler.matchWidthOrHeight = 0f;
            }
            // 强制重新构建布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(canvas.GetComponent<RectTransform>());
            // 或者对整个画布进行刷新
            Canvas.ForceUpdateCanvases();
        }
    }

    private static void BindData(UIBase uiBase, UIData uiData)
    {
        if (uiData == null) return;
        var type = uiBase.GetType();
        var property = type.GetProperty("Data", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property == null) return;
        property.SetValue(uiBase, uiData);

    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <returns></returns>
    public async Task<bool> AsyncInit()
    {
        if (inited) return true;

        try
        {
            canvasLoader = new AssetLoader<GameObject>("Prefabs/UI/Canvas.prefab");
            await canvasLoader.AsyncLoad();


            inited = true;

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"UIManager AsyncInit Error: {e}");
            return false;
        }
    }
    /// <summary>
    /// 不要主动调用
    /// </summary>
    /// <param name="uiBase"></param>
    public static void UIDestroy(UIBase uiBase)
    {

        if (UIDict.Remove(uiBase.GetType()))
        {

        }


    }

    private static void _OnShowBefore(UIBase uibase)
    {
        if ((uibase.BackgroundMask & EBackgroundMask.CloseUIOnCancelClick) != 0)
        {
            RegistClickCancelUIBase(uibase);
        }

        if ((uibase.BackgroundMask & EBackgroundMask.BlockCloseUIOnCancelClick) != 0)
        {
            CloseUIOnClickCancelIsBlock = true;
        }
    }

    private static void _OnCloseBefore(UIBase uibase)
    {
        if ((uibase.BackgroundMask & EBackgroundMask.CloseUIOnCancelClick) != 0)
        {
            if (CloseUIOnClickCancelQueue.Remove(uibase))
            {

            }
        }

        if ((uibase.BackgroundMask & EBackgroundMask.BlockCloseUIOnCancelClick) != 0)
        {
            CloseUIOnClickCancelIsBlock = false;
        }


    }

    private static List<UIBase> toastUIList;

    private static Dictionary<string, AssetLoader<GameObject>> toastLoaderDict;

    public static void ShowCoinToast(string toast, Vector3? toastPos)
    {
        _ = AsyncShowToast(toast, UIToast.ToastType.Coin, toastPos);
    }

    public static void ShowCAToast(string toast, Vector3? toastPos)
    {
        _ = AsyncShowToast(toast, UIToast.ToastType.Ca, toastPos);
    }

    public static void ShowToast(string toast, string toastUIPath = DefaultToastPath)
    {
        _ = AsyncShowToast(toast, UIToast.ToastType.Default, null, toastUIPath);
    }

    public static async Task AsyncShowToast(string toast,
        UIToast.ToastType toastType = UIToast.ToastType.Default,
        Vector3? toastPos = null,
        string toastUIPath = DefaultToastPath)
    {
        if (string.IsNullOrEmpty(toastUIPath))
        {
            return;
        }

        if (toastLoaderDict == null)
        {
            toastLoaderDict = new Dictionary<string, AssetLoader<GameObject>>();
        }

        if (!toastLoaderDict.TryGetValue(toastUIPath, out var loader))
        {
            loader = new AssetLoader<GameObject>(toastUIPath);
            toastLoaderDict.Add(toastUIPath, loader);
            await loader.AsyncLoad();
        }
        var prefabCompBase = loader.Asset.GetComponent<UIBase>();
        var uiType = prefabCompBase.GetType();
        var uiName = uiType.Name;
        UICanvasLayer defaultCanvasLayer = UICanvasLayer.Default_Overlay;
        EBackgroundMask eBackgroundMask = EBackgroundMask.None;
        EUIGroupTag uigroupTag = EUIGroupTag.None;
        if (uiType.TryGetCustomAttribute(out UISettingAttribute result))
        {
            defaultCanvasLayer = result.DefaultLayer;
            eBackgroundMask = result.BackgroundMask;
            uigroupTag = result.UIGroupTag;
        }

        Transform canvasRoot = GetCanvasLayerTransform(defaultCanvasLayer);
        var preloader = await AssetsManager.AsyncCreatePreloadAssetsLoader(uiType);
        var resultGO = loader.Asset.SpawnNewOne(canvasRoot);
        resultGO.transform.SetAsLastSibling();
        var uiBase = resultGO.GetComponent<UIBase>();
        if (uiBase == null)
        {
            preloader.Unload();
            throw new Exception($"OpenUI Error: {uiName} does not have a UIBase component.");
        }

        try
        {
            var defCLPro = uiType.GetProperty("DefaultCanvasLayer", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (defCLPro != null) defCLPro.SetValue(uiBase, defaultCanvasLayer);
            var CLPro = uiType.GetProperty("CanvasLayer", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (CLPro != null) CLPro.SetValue(uiBase, defaultCanvasLayer);
            var bgMaskPro = uiType.GetProperty("BackgroundMask", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (bgMaskPro != null) bgMaskPro.SetValue(uiBase, eBackgroundMask);
            var UIGroupTagPro = uiType.GetProperty("UIGroupTag", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (UIGroupTagPro != null) UIGroupTagPro.SetValue(uiBase, uigroupTag);

            AssetsManager.SetPreloadAssetLoaderToObj(uiBase, preloader);

            BindData(uiBase, new UIToastParam
            {
                msg = toast,
                toastType = toastType,
                toastPos = toastPos,
            });
            MessageDispatch.BindMessage(uiBase);

            if (toastUIList == null)
                toastUIList = new List<UIBase>();
            toastUIList.Add(uiBase);
            //await uiBase.OnAsyncPreload();
            uiBase.OnInit();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }


        await uiBase.OnShowAsync();

    }

    private static void OnUIBaseInitBefore(UIBase uiBase)
    {
        var resultArr = uiBase.GetComponentsInChildren<IUIBaseInit>();
        for (int i = 0; i < resultArr.Length; ++i)
        {
            resultArr[i].OnUIBaseInitBefore();
        }

    }

    public static bool CloseUIOnClickCancelIsBlock { get; set; }
    private void Update()
    {
        if (!CloseUIOnClickCancelIsBlock && Input.GetButtonDown("Cancel"))
        {
            try
            {
                _ = _CloseUIOnClickCancel();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
    private bool _isClickCancelUIClosing = false;
    private async Task _CloseUIOnClickCancel()
    {
        if (_isClickCancelUIClosing) return;
        _isClickCancelUIClosing = true;

        if (CloseUIOnClickCancelQueue.TryDequeue(out var uibase))
        {
            await uibase.AsyncClose();
        }
        _isClickCancelUIClosing = false;
    }

    public static void RegistClickCancelUIBase(UIBase uibase)
    {
        CloseUIOnClickCancelQueue.Push(uibase);
    }

    /// <summary>
    /// ui交互状态
    /// </summary>
    public static bool UIInteractionState { get; private set; } = true;

    public static void SetUIInteractionState(bool isEnable)
    {
        if (UIInteractionState == isEnable) return;
        UIInteractionState = isEnable;
        foreach (var kvp in canvasLayerDict)
        {
            if (kvp.Value.TryGetComponent<GraphicRaycaster>(out var comp))
            {
                comp.enabled = UIInteractionState;
            }
        }
    }
}

public static class UIManagerUtil
{
    public static bool IsGlobal(this UICanvasLayer layer)
    {
        return layer == UICanvasLayer.Default_Global ||
            layer == UICanvasLayer.Background_Global ||
            layer == UICanvasLayer.Main_Global ||
            layer == UICanvasLayer.Overlay_Global ||
            layer == UICanvasLayer.Popup_Global ||
            layer == UICanvasLayer.Top_Global ||
            layer == UICanvasLayer.System_Global;
    }

    public static bool IsOverlay(this UICanvasLayer layer)
    {
        return layer == UICanvasLayer.Default_Overlay ||
            layer == UICanvasLayer.Background_Overlay ||
            layer == UICanvasLayer.Main_Overlay ||
            layer == UICanvasLayer.Overlay_Overlay ||
            layer == UICanvasLayer.Popup_Overlay ||
            layer == UICanvasLayer.Top_Overlay ||
            layer == UICanvasLayer.System_Overlay;
    }

    public static bool IsCamera(this UICanvasLayer layer)
    {
        return layer == UICanvasLayer.Default_Camera ||
            layer == UICanvasLayer.Background_Camera ||
            layer == UICanvasLayer.Main_Camera ||
            layer == UICanvasLayer.Overlay_Camera ||
            layer == UICanvasLayer.Popup_Camera ||
            layer == UICanvasLayer.Top_Camera ||
            layer == UICanvasLayer.System_Camera;
    }
}
