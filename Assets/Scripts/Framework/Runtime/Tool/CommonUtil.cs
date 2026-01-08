using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.VFX;


public static class CommonUtil
{
    public static bool Intersection(int leftMin, int leftMax, int rightMin, int rightMax)
    {
        return Mathf.Max(leftMin, rightMin) < Mathf.Min(leftMax, rightMax);
    }

    public static int ToIndex(int width, int height, int maxWidth)
    {
        return height * maxWidth + width;
    }

    public static bool IsValidNumFunc(string tex)
    {
        try
        {
            const string StrictEmailPattern = @"^\+?\d{10,15}$";

            return Regex.IsMatch(tex,
                StrictEmailPattern,
                RegexOptions.IgnoreCase,
                TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    public static bool IsValidEmailAddressFunc(string tex)
    {
        try
        {
            const string StrictEmailPattern =
@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

            return Regex.IsMatch(tex,
                StrictEmailPattern,
                RegexOptions.IgnoreCase,
                TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    /// <summary>
    /// 安卓自定义时长震动
    /// </summary>
    /// <param name="duration">时长ms</param>
    /// <param name="amplitude">震动幅度 0 - 255</param>
    public static void Vibrate(long duration = 500, int amplitude = 255)
    {
        if (GlobalSingleton.Instance.VibrateIsEnable)
        {
            return;
        }
        try
        {
            //if (SystemInfo.supportsVibration)
            //{
            //    Handheld.Vibrate();
            //}
            if (Application.platform == RuntimePlatform.Android)
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        AndroidJavaClass vibrationPlugin = new AndroidJavaClass("com.cdt.CDTAndroidPlugin");
                        vibrationPlugin.CallStatic("Vibrate", currentActivity, duration, amplitude);
                    }
                }
            }

        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public static string BuildStringFormCollection(this ICollection vals, char splitChar = '|')
    {
        string results = "";
        int i = 0;
        foreach (var value in vals)
        {
            results += value;
            if (i != vals.Count - 1)
            {
                results += splitChar;
            }
            i++;
        }
        return results;
    }

    public static List<T> BuildListFormString<T>(this string str, char splitChar = '|')
    {
        List<T> list = new List<T>();
        if (string.IsNullOrEmpty(str))
            return list;
        string[] arr = str.Split('|', StringSplitOptions.RemoveEmptyEntries);
        foreach (string v in arr)
        {
            if (string.IsNullOrEmpty(v)) continue;
            T val = (T)Convert.ChangeType(v, typeof(T));
            list.Add(val);
        }
        return list;
    }

    public static Vector3 GetMiddlePoint(Vector3 begin, Vector3 end, float delta = 0)
    {
        Vector3 center = Vector3.Lerp(begin, end, 0.5f);
        Vector3 beginEnd = end - begin;
        Vector3 perpendicular = new Vector3(-beginEnd.y, beginEnd.x, 0).normalized;
        Vector3 middle = center + perpendicular * delta;
        return middle;
    }
}

public static class ArrayUtil
{
    /// <summary>
    /// 将数组内所有元素后移一位
    /// 返回 有多出来的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>
    public static T ShiftArrayRight<T>(this T[] array)
    {
        if (array == null || array.Length == 0) return default(T);
        var result = array[array.Length - 1];
        for (int i = array.Length - 2; i >= 0; --i)
        {
            array[i + 1] = array[i];
        }
        array[0] = default(T);
        return result;
    }

    /// <summary>
    /// 将数组内所有元素前移一位
    /// 返回 有多出来的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>
    public static T ShiftArrayLeft<T>(this T[] array)
    {
        if (array == null || array.Length == 0) return default(T);
        var result = array[0];
        for (int i = 1; i < array.Length; ++i)
        {
            array[i - 1] = array[i];
        }
        array[array.Length - 1] = default(T);
        return result;
    }

    public static T GetByXY<T>(this T[] array, int x, int y, int max)
    {
        if (array == null || array.Length == 0)
            return default(T);

        return array[XYToIndex(x, y, max)];
    }

    public static int XYToIndex(int x, int y, int max)
    {
        return y * max + x;
    }

    public static (int x, int y) IndexToXY(int index, int max)
    {
        int x = index % max;
        int y = index / max;
        return (x, y);
    }

    public static bool TryGet<T>(this T[] array, int index, out T result)
    {
        if (index < array.Length)
        {
            result = array[index];
            return true;
        }
        result = default(T);
        return false;
    }
}

public static class ObjectUtil
{
    public static bool TryTo<T>(this object[] objs, int idx, out T result)
    {
        if (objs == null || idx >= objs.Length)
        {
            result = default(T);
            return false;
        }
        else if (objs[idx] is T tmp)
        {
            result = tmp;
            return true;
        }
        result = default(T);
        return false;
    }

    public static T To<T>(this object[] objs, int idx = 0)
    {
        if (TryTo<T>(objs, idx, out var result))
        {
            return result;
        }
        return default(T);
    }
}

public static class GameObjectUtil
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        var comp = go.GetComponent<T>();
        if (comp == null)
        {
            comp = go.AddComponent<T>();
        }
        return comp;
    }

}

public static class CanvasUtil
{

}

public static class RectTransformUtil
{
    public static Vector2 WorldToCanvasPosition(Canvas canvas, Vector3 worldPos, Camera camera = null)
    {
        if (camera == null)
            camera = Camera.main;

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // 对于ScreenSpaceOverlay，直接使用屏幕坐标
            return RectTransformUtility.WorldToScreenPoint(camera, worldPos);
        }
        else
        {
            // 对于其他渲染模式，考虑Canvas缩放
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldPos);

            var rt = canvas.GetComponent<RectTransform>();
            var y = screenPoint.y * rt.sizeDelta.y / Screen.height;
            var x = screenPoint.x * rt.sizeDelta.x / Screen.width;

            return new Vector2(x, y);
        }
    }

    public static Vector3 WorldPosToWorldPosByCanvas(Canvas canvas, Vector3 worldPos)
    {
        var screenPoint = WorldToCanvasPosition(canvas, worldPos);//屏幕坐标

        var rt = canvas.GetComponent<RectTransform>();
        var camera = canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : Camera.main;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, screenPoint, camera, out var worldPoint);


        return worldPoint;
    }


    private static readonly Vector3[] corners = new Vector3[4];
    /// <summary>
    /// 用于 Screen Space - Overlay 模式的 Canvas：
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <returns></returns>
    public static bool IsRectTransformVisibleInOverlay(this RectTransform rectTransform)
    {
        rectTransform.GetWorldCorners(corners);

        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        bool isVisible = false;

        foreach (Vector3 corner in corners)
        {
            if (screenRect.Contains(corner))
            {
                isVisible = true;
                break;
            }
        }

        return isVisible;
    }

    public static void SetAnchoredPositionX(this RectTransform target, float x)
    {
        target.anchoredPosition = new Vector2(x, target.anchoredPosition.y);
    }
    public static void SetAnchoredPositionY(this RectTransform target, float y)
    {
        target.anchoredPosition = new Vector2(target.anchoredPosition.x, y);
    }
    public static Tween DoFadeAndAnchorsMoveXFrom(this RectTransform target, float offsetAnchorsPosX, float duration, float beginFade = 0f, float endFade = 1f)
    {
        return target.DoFadeAndAnchorsMoveFrom(new Vector2(offsetAnchorsPosX, 0), duration, beginFade, endFade);
    }
    public static Tween DoFadeAndAnchorsMoveYFrom(this RectTransform target, float offsetAnchorsPosY, float duration, float beginFade = 0f, float endFade = 1f)
    {
        return target.DoFadeAndAnchorsMoveFrom(new Vector2(0, offsetAnchorsPosY), duration, beginFade, endFade);
    }
    public static Tween DoFadeAndAnchorsMoveFrom(this RectTransform target, Vector2 offsetAnchorsPos, float duration, float beginFade = 0f, float endFade = 1f)
    {
        var cg = target.gameObject.GetOrAddComponent<CanvasGroup>();
        cg.alpha = beginFade;
        var pos = target.anchoredPosition;
        target.anchoredPosition = target.anchoredPosition + offsetAnchorsPos;
        Sequence seq = DOTween.Sequence();
        seq.Append(target.DOAnchorPos(pos, duration));
        seq.Join(cg.DOFade(endFade, duration));

        return seq;
    }
    public static Tween DoFadeAndAnchorsMoveYTo(this RectTransform target, float offsetAnchorsPosY, float duration, float beginFade = 1f, float endFade = 0f)
    {
        return target.DoFadeAndAnchorsMoveTo(new Vector2(0, offsetAnchorsPosY), duration, beginFade, endFade);
    }
    public static Tween DoFadeAndAnchorsMoveXTo(this RectTransform target, float offsetAnchorsPosX, float duration, float beginFade = 1f, float endFade = 0f)
    {
        return target.DoFadeAndAnchorsMoveTo(new Vector2(offsetAnchorsPosX, 0), duration, beginFade, endFade);
    }
    public static Tween DoFadeAndAnchorsMoveTo(this RectTransform target, Vector2 offsetAnchorsPos, float duration, float beginFade = 1f, float endFade = 0f)
    {
        var cg = target.gameObject.GetOrAddComponent<CanvasGroup>();
        cg.alpha = beginFade;
        var pos = target.anchoredPosition + offsetAnchorsPos;
        Sequence seq = DOTween.Sequence();
        seq.Append(target.DOAnchorPos(pos, duration));
        seq.Join(cg.DOFade(endFade, duration));

        return seq;
    }

    public static Tween DoFadeAndAnchorsOverMoveFrom(this RectTransform target, Vector2 offsetAnchorsPos, float duration, Vector2 overMove, float beginFade = 0f, float endFade = 1f)
    {
        var cg = target.gameObject.GetOrAddComponent<CanvasGroup>();
        cg.alpha = beginFade;
        var pos = target.anchoredPosition;
        target.anchoredPosition = target.anchoredPosition + offsetAnchorsPos;
        Sequence seq = DOTween.Sequence();
        seq.Append(target.DOAnchorPos(pos + overMove, duration));
        seq.Join(cg.DOFade(endFade, duration));
        seq.Append(target.DOAnchorPos(pos, 0.1f).SetEase(Ease.OutCubic));

        return seq;
    }
    public static Tween DoFadeAndAnchorsOverMoveXFrom(this RectTransform target, float offsetAnchorsPosX, float duration, float overMoveX, float beginFade = 0f, float endFade = 1f)
    {
        return target.DoFadeAndAnchorsOverMoveFrom(new Vector2(offsetAnchorsPosX, 0), duration, new Vector2(overMoveX, 0), beginFade, endFade);
    }

    public static Tween DoFadeAndAnchorsOverMoveYFrom(this RectTransform target, float offsetAnchorsPosY, float duration, float overMoveY, float beginFade = 0f, float endFade = 1f)
    {
        return target.DoFadeAndAnchorsOverMoveFrom(new Vector2(0, offsetAnchorsPosY), duration, new Vector2(0, overMoveY), beginFade, endFade);
    }

}
public static class TransformUtil
{
    /// <summary>
    /// 两点之间带曲线路径的插值
    /// </summary>w
    /// <param name="start">起始点</param>
    /// <param name="end">结束点</param>
    /// <param name="t">插值系数(0-1)</param>
    /// <param name="curveHeight">曲线高度(控制弧高)</param>
    /// <param name="frequency">正弦波动频率</param>
    /// <returns>曲线路径上的点</returns>
    public static Tween DoSinLerpMove(this Transform target, Vector3 end, float duration, float curveHeight = 1f, int frequency = 1)
    {
        var v = 0f;
        var srcPos = target.position;
        return DOTween.To(() =>
        {
            return v;
        },
        (resultT) =>
        {
            // 基础线性插值
            Vector3 linearPos = Vector3.Lerp(srcPos, end, resultT);

            // 计算垂直于AB连线的方向(用于创建弧线)
            Vector3 direction = (end - srcPos).normalized;
            Vector3 up = Vector3.Cross(direction, Vector3.Cross(Vector3.up, direction)).normalized;

            // 使用正弦函数计算曲线高度
            float curveFactor = Mathf.Sin(resultT * Mathf.PI * frequency);
            float currentHeight = curveFactor * curveHeight;

            // 应用曲线偏移
            target.position = linearPos + up * currentHeight;

            v = resultT;
        }, 1f, duration);

    }

    public static Tween DoLerpMove(this Transform target, Vector3 pos, float duration)
    {
        var v = 0f;
        var srcPos = target.position;
        return DOTween.To(() =>
        {
            return v;
        },
        (result) =>
        {
            target.position = Vector3.Lerp(srcPos, pos, result);
            v = result;
        }, 1f, duration);
    }

    public static Tween DoSLerpMove(this Transform target, Vector3 pos, float duration)
    {
        var v = 0f;
        var srcPos = target.position;
        return DOTween.To(() =>
        {
            return v;
        },
        (result) =>
        {
            target.position = Vector3.Slerp(srcPos, pos, result);
            v = result;
        }, 1f, duration);
    }
    public static Tween DoRandomLerpMove(this Transform target, Vector3 pos, float duration)
    {
        var rand = RandomHelp.RandomRange(0, 2);

        switch (rand)
        {
            case 1:
                return target.DoSLerpMove(pos, duration);
            case 2:
                return target.DoSinLerpMove(pos, duration, RandomHelp.RandomRange(-500, 200));
            default:
                return target.DoLerpMove(pos, duration);
        }
    }

    public static Tween DoRotateShake(this Transform target, float intensity = 1f, int shakeCnt = 1)
    {
        Sequence seq = DOTween.Sequence();
        var orgRotate = target.localRotation;
        for (int i = 0; i < shakeCnt; ++i)
        {
            seq.Append(target.DOLocalRotate((orgRotate * Quaternion.Euler(0, 0, -2f * intensity)).eulerAngles, 0.05f).SetEase(Ease.InCubic).SetUpdate(true));
            seq.Append(target.DOLocalRotate((orgRotate * Quaternion.Euler(0, 0, 2f * intensity)).eulerAngles, 0.1f).SetEase(Ease.InCubic).SetUpdate(true));
            seq.Append(target.DOLocalRotate((orgRotate * Quaternion.Euler(0, 0, -2f * intensity)).eulerAngles, 0.1f).SetEase(Ease.InCubic).SetUpdate(true));
        }
        seq.Append(target.DOLocalRotate((orgRotate).eulerAngles, 0.05f).SetEase(Ease.InCubic).SetUpdate(true));
        return seq;
    }
    public static Transform SetLocalScale(this Transform target, float scale)
    {
        target.localScale = new Vector3(scale, scale, scale);
        return target;
    }
    public static void ScaleAction(this Transform target, Vector3 startScale, Action<Transform> action)
    {
        Sequence seq = DOTween.Sequence();
        Vector3 orgScale = startScale;
        seq.Append(target.DOScale(Vector3.Scale(orgScale, new Vector3(1.2f, 0.8f, 1f)), 0.05f).SetEase(Ease.InCubic).SetUpdate(true));
        seq.Append(target.DOScale(Vector3.Scale(orgScale, new Vector3(0.8f, 1.2f, 1f)), 0.1f).SetEase(Ease.InCubic).SetUpdate(true));
        seq.Append(target.DOScale(orgScale, 0.05f).SetEase(Ease.OutCubic).SetUpdate(true));

        seq.OnComplete(() =>
        {
            action?.Invoke(target);
        });
    }
    public static void ClickScaleAni(this Transform target, Action<Transform> action, float durationScale = 1f, float intensity = 1f)
    {
        var comp = target.gameObject.GetOrAddComponent<ButtonEx>();
        target.ClickScaleAni((setV) => comp.IsClicked = setV, () => comp.IsClicked, (target) => { action?.Invoke(target); }, durationScale, intensity);
    }
    public static void ClickScaleAni(this Transform target, Action<bool> switchSet, Func<bool> switchGet, Action<Transform> action, float durationScale = 1f, float intensity = 1f)
    {
        var m_ClickAni = switchGet != null ? switchGet.Invoke() : false;
        if (m_ClickAni) return;
        switchSet.Invoke(true);

        Sequence seq = DOTween.Sequence();
        Vector3 orgScale = target.localScale;
        float offset = 0.2f * intensity;
        seq.Append(target.DOScale(Vector3.Scale(orgScale, new Vector3(1f + offset, 1f - offset, 1f)), 0.05f * durationScale).SetEase(Ease.InCubic).SetUpdate(true));
        seq.Append(target.DOScale(Vector3.Scale(orgScale, new Vector3(1f - offset, 1f + offset, 1f)), 0.1f * durationScale).SetEase(Ease.InCubic).SetUpdate(true));
        seq.Append(target.DOScale(orgScale, 0.05f * durationScale).SetEase(Ease.OutCubic).SetUpdate(true));

        seq.OnComplete(() =>
        {
            switchSet?.Invoke(false);
            action?.Invoke(target);
        });
    }
    public static async Task AsyncClickScaleAni(this Transform target, Action<Transform> action, float durationScale = 1f, float intensity = 1f)
    {
        var comp = target.gameObject.GetOrAddComponent<ButtonEx>();
        await target.AsyncClickScaleAni((setV) => comp.IsClicked = setV, () => comp.IsClicked, (target) => { action?.Invoke(target); }, durationScale, intensity);
    }
    public static async Task AsyncClickScaleAni(this Transform target, Action<bool> switchSet, Func<bool> switchGet, Action<Transform> action, float durationScale = 1f, float intensity = 1f)
    {
        var m_ClickAni = switchGet != null ? switchGet.Invoke() : false;
        if (m_ClickAni) return;
        switchSet.Invoke(true);

        Sequence seq = DOTween.Sequence();
        Vector3 orgScale = target.localScale;
        float offset = 0.2f * intensity;
        seq.Append(target.DOScale(Vector3.Scale(orgScale, new Vector3(1f + offset, 1f - offset, 1f)), 0.05f * durationScale).SetEase(Ease.InCubic).SetUpdate(true));
        seq.Append(target.DOScale(Vector3.Scale(orgScale, new Vector3(1f - offset, 1f + offset, 1f)), 0.1f * durationScale).SetEase(Ease.InCubic).SetUpdate(true));
        seq.Append(target.DOScale(orgScale, 0.05f * durationScale).SetEase(Ease.OutCubic).SetUpdate(true));

        await seq.AsyncWaitForCompletion();
        switchSet?.Invoke(false);
        action?.Invoke(target);
    }

    public static void ClickRotateAni(this Transform target, Action<bool> switchSet, Func<bool> switchGet, Action<Transform> action, float durationScale = 1f, float strength = 1f)
    {
        var m_ClickAni = switchGet != null ? switchGet.Invoke() : false;
        if (m_ClickAni) return;
        switchSet.Invoke(true);

        Sequence seq = DOTween.Sequence();
        var orgRotate = target.localRotation;
        seq.Append(target.DOLocalRotate((orgRotate * Quaternion.Euler(0, 0, -2f * strength)).eulerAngles, 0.05f * durationScale).SetEase(Ease.InCubic).SetUpdate(true));
        seq.Append(target.DOLocalRotate((orgRotate * Quaternion.Euler(0, 0, 2f * strength)).eulerAngles, 0.1f * durationScale).SetEase(Ease.InCubic).SetUpdate(true));
        seq.Append(target.DOLocalRotate((orgRotate * Quaternion.Euler(0, 0, -2f * strength)).eulerAngles, 0.1f * durationScale).SetEase(Ease.InCubic).SetUpdate(true));
        seq.Append(target.DOLocalRotate((orgRotate).eulerAngles, 0.05f * durationScale).SetEase(Ease.InCubic).SetUpdate(true));

        seq.OnComplete(() =>
        {
            switchSet?.Invoke(false);
            action?.Invoke(target);
        });

    }

    public static IEnumerator DoUIBackGroundFade(this Transform target)
    {
        var ima = target.GetComponent<Image>();
        if (ima != null)
        {
            var col = ima.color;
            col.a = 0f;
            ima.color = col;
            yield return ima.DOFade(0.83f, 0.2f).WaitForCompletion();
        }
    }
    public static IEnumerator DoUIContentScale(this Transform target)
    {
        Sequence seq = DOTween.Sequence();
        Vector3 orgScale = Vector3.one;
        seq.Append(target.DOScale(new Vector3(1.2f, 0.8f, 1f), 0.1f).SetEase(Ease.InCubic).SetUpdate(true));
        seq.Append(target.DOScale(new Vector3(0.8f, 1.2f, 1f), 0.1f).SetEase(Ease.InCubic).SetUpdate(true));
        seq.Append(target.DOScale(orgScale, 0.05f).SetEase(Ease.OutCubic).SetUpdate(true));

        yield return seq.WaitForCompletion();
    }
    public static IEnumerator DOUIContentFadeShow(this Transform target, Vector3 localPos)
    {
        Sequence seq = DOTween.Sequence();
        var cg = target.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = target.gameObject.AddComponent<CanvasGroup>();
        }

        seq.Append(cg.DOFade(1, 0.2f).SetEase(Ease.Linear));
        seq.Join(target.DOLocalMove(localPos, 0.2f).SetEase(Ease.Linear));
        yield return seq.WaitForCompletion();

    }
    public static CanvasGroup SetFade(this Transform target, float fade)
    {
        var cg = target.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = fade;
        }
        else
        {
            cg = target.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = fade;
        }
        return cg;
    }

    public static Task PopShow(this Transform target)
    {

        return Task.CompletedTask;
    }

    public static Task PopHide(this Transform target)
    {
        return Task.CompletedTask;
    }

    public static T GetOrAddComponent<T>(this Transform go) where T : Component
    {
        return go?.gameObject.GetOrAddComponent<T>();
    }

    public static Transform SetActive(this Transform target, bool flag)
    {
        target.gameObject.SetActive(flag);
        return target;
    }
}

public static class ButtonUtil
{
    public static Button ClearAllBtnCallback(this Button btn)
    {
        btn.onClick.RemoveAllListeners();
        return btn;
    }

    public static Button RegistBtnCallback(this Button btn, UnityAction action)
    {
        btn.onClick.AddListener(action);
        return btn;
    }
    public static void RegistBtnCallback(this Button btn, Action<Button> action)
    {
        btn.onClick.AddListener(() =>
        {
            action?.Invoke(btn);
        });
    }

}

public static class ToggleUtil
{
    public static void RegistToggleCallback(this Toggle tog, UnityAction<Toggle, bool> action)
    {
        tog.onValueChanged.AddListener((result) =>
        {
            action?.Invoke(tog, result);
        });
    }
}

public static class MonoBehaviourUtil
{
    public static void ClickRotateAni(this MonoBehaviour target, Action action, float durationTime = 1f, float strength = 1f)
    {
        var comp = target.GetOrAddComponent<ButtonEx>();
        target.ClickRotateAni((setV) => comp.IsClicked = setV, () => comp.IsClicked, (target) => { action?.Invoke(); }, durationTime, strength);
    }

    public static void ClickRotateAni(this MonoBehaviour target, Action<bool> switchSet, Func<bool> switchGet, Action<MonoBehaviour> action, float durationTime = 1f, float strength = 1f)
    {
        target.transform.ClickRotateAni(switchSet, switchGet,
            (result) => { action?.Invoke(target); },
            durationTime, strength);
    }

    public static void ClickScaleAni(this MonoBehaviour target, Action action)
    {
        var comp = target.GetOrAddComponent<ButtonEx>();
        target.ClickScaleAni((setV) => comp.IsClicked = setV, () => comp.IsClicked, (target) => { action?.Invoke(); });
    }
    public static void ClickScaleAni(this MonoBehaviour target, Action<bool> switchSet, Func<bool> switchGet, Action<MonoBehaviour> action, float durationTime = 1f)
    {
        target.transform.ClickScaleAni(switchSet, switchGet, (result) =>
        {
            action?.Invoke(target);
        }, durationTime);
    }
    public static T GetOrAddComponent<T>(this MonoBehaviour go) where T : Behaviour
    {
        return go?.gameObject.GetOrAddComponent<T>();
    }

}

public static class ImageUtil
{
    public static Image SetFade(this Image ima, float fadeValue)
    {
        var col = ima.color;
        col.a = fadeValue;
        ima.color = col;
        return ima;
    }

    public static Image SpawnNew(string name = "imaNewSpawned")
    {
        var go = new GameObject(name, new Type[] {
            typeof(RectTransform),
            typeof(Image) });

        return go.GetComponent<Image>();
    }

    public static Image SetSprite(this Image ima, Sprite sprite)
    {
        ima.sprite = sprite;
        return ima;
    }

}

public static class ListUtil
{
    public static void Shuffle<T>(this IList<T> list, int? seed = null)
    {
        System.Random rng = seed != null ? new System.Random((int)seed) : new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static bool TryFind<T>(this IList<T> list, Predicate<T> match, out T result, out int idx)
    {
        if (match == null)
        {
            result = default(T);
            idx = -1;
            return false;
        }

        for (int i = 0; i < list.Count; ++i)
        {
            if (match(list[i]))
            {
                result = list[i];
                idx = i;
                return true;
            }
        }
        result = default(T);
        idx = -1;
        return false;
    }

    public static bool TryRandomPopOne<T>(this IList<T> list, out T result)
    {
        if (list.Count == 0)
        {
            result = default(T);
            return false;
        }

        int idx = RandomHelp.RandomRange(0, list.Count);
        result = list[idx];
        list.RemoveAt(idx);
        return true;
    }
    public static bool TryRandomPop<T>(this IList<T> list, IList<T> resultValue, int? max = null)
    {
        if (list.Count == 0)
        {
            return false;
        }

        if (resultValue == null) return true;
        var maxCnt = max.HasValue ? max.Value : list.Count;
        maxCnt = Mathf.Max(1, maxCnt);
        int popCnt = RandomHelp.RandomRange(1, maxCnt);
        for (int i = 0; i < popCnt; ++i)
        {
            if ((list.TryRandomPopOne(out var result)))
            {
                resultValue.Add(result);
            }
        }
        return popCnt > 0;
    }

    public static void Enqueue<T>(this IList<T> list, T value)
    {
        list.Add(value);
    }

    public static T Dequeue<T>(this IList<T> list)
    {
        if (list.Count == 0)
            return default;

        var result = list[0];
        list.RemoveAt(0);
        return result;
    }

    public static bool TryDequeue<T>(this IList<T> list, out T value)
    {
        if (list.Count == 0)
        {
            value = default;
            return false;
        }

        value = list[0];
        list.RemoveAt(0);
        return true;
    }

    public static bool TryPeekForward<T>(this IList<T> list, out T Value)
    {
        if (list.Count == 0)
        {
            Value = default(T);
            return false;
        }
        Value = list[0];
        return true;
    }

    public static bool TryPeekBackward<T>(this IList<T> list, out T Value)
    {
        if (list.Count == 0)
        {
            Value = default(T);
            return false;
        }
        Value = list[list.Count - 1];
        return true;
    }

    public static void Push<T>(this IList<T> list, T value)
    {
        list.Insert(0, value);
    }

    public static bool TryPop<T>(this IList<T> list, out T value)
    {
        if (list.TryPeekForward(out value))
        {
            list.RemoveAt(0);
            return true;
        }
        else
        {
            return false;
        }

    }
}

public static class ShadowUtil
{
    public static Tween DoEffectDistance(this Shadow target, Vector2 to, float duration)
    {
        var v = 0f;
        var srcPos = target.effectDistance;
        return DOTween.To(() =>
        {
            return v;
        },
        (result) =>
        {
            target.effectDistance = Vector2.Lerp(srcPos, to, result);
            v = result;
        }, 1f, duration);
    }

    public static Tween DoFade(this Shadow target, float to, float duration)
    {
        var v = 0f;
        var srcPos = target.effectColor;
        return DOTween.To(() =>
        {
            return v;
        },
        (result) =>
        {
            Color tmp = srcPos;
            tmp.a = Mathf.Lerp(tmp.a, to, result);
            target.effectColor = tmp;
            v = result;
        }, 1f, duration);
    }

    public static Shadow SetFade(this Shadow target, float to)
    {
        var tmp = target.effectColor;
        tmp.a = to;
        target.effectColor = tmp;
        return target;
    }
}



public static class TypeUtil
{
    public static bool TryGetPropertyByName<PropertyType>(this Type type, string name, BindingFlags bindingFlags, out PropertyInfo resultInfo)
    {
        var info = type.GetProperty(name, bindingFlags);
        if (info != null && info.PropertyType == typeof(PropertyType))
        {
            resultInfo = info;
            return true;
        }
        resultInfo = null;
        return false;
    }

    public static bool TryGetPropertysByAttribute<PropertyType, AttributeType>(this Type type, BindingFlags bindingFlags, ref List<PropertyInfo> resultList) where AttributeType : Attribute
    {
        var infos = type.GetProperties(bindingFlags);
        if (infos == null || infos.Length == 0)
        {
            return false;
        }
        bool resultFlag = false;
        for (int i = 0; i < infos.Length; i++)
        {
            var info = infos[i];
            if (info.PropertyType == typeof(PropertyType))
            {
                var att = info.GetCustomAttribute(typeof(AttributeType));
                if (att != null)
                {
                    if (resultList == null)
                    {
                        resultList = new List<PropertyInfo>();
                    }
                    resultList.Add(info);
                    resultFlag = true;
                }
            }
        }
        return resultFlag;
    }

    public static bool TryGetCustomAttribute<T>(this Type type, out T result) where T : Attribute
    {
        result = type.GetCustomAttribute(typeof(T), true) as T;
        if (result != null)
        {
            return true;
        }
        return false;
    }


    public static bool TryGetCustomAttributes<T>(this Type type, out T[] result) where T : Attribute
    {
        result = type.GetCustomAttributes(typeof(T), true) as T[];
        if (result != null)
        {
            return true;
        }
        return false;
    }


    public static GameObject SpawnNewOne(this GameObject go, Transform parent = null)
    {
        return GameObject.Instantiate(go, parent);
    }
}

public static class DateTimeUtil
{
    /// <summary>
    /// 获取从公元元年开始的总天数
    /// </summary>
    public static int GetDaysSinceEpoch(this DateTime date)
    {
        DateTime epoch = new DateTime(1, 1, 1);
        return (date - epoch).Days;
    }
}

public static class IntUtil
{
    public static int Next(this int value, int min, int max, int span = 1)
    {
        int range = max - min;

        if (span >= 0)
        {
            // 正向移动
            value = min + (value - min + span) % range;
        }
        else
        {
            // 反向移动
            int steps = Mathf.Abs(span);
            value = min + (value - min - steps % range + range) % range;
        }
        return value;
    }

    public static void ToNext(this ref int value, int min, int max, int span = 1)
    {
        value = value.Next(min, max, span);
    }

    public static int Last(this int value, int min, int max, int span = 1)
    {
        return value.Next(min, max, -span);
    }

    public static void ToLast(this ref int value, int min, int max, int span = 1)
    {
        value.ToNext(min, max, -span);
    }
    public static int[] RandomSplitThree(this int total)
    {
        if (total < 3)
            throw new ArgumentException("总数必须至少为3");

        System.Random rand = new System.Random();
        int[] result = new int[3];

        // 初始每份至少为1
        int remaining = total - 3;
        result[0] = result[1] = result[2] = 1;

        // 将剩余的值随机分配
        for (int i = 0; i < remaining; i++)
        {
            result[rand.Next(0, 3)]++;
        }

        return result;
    }

    /// <summary>
    /// 将整数转换为K/M格式的字符串
    /// </summary>
    /// <param name="number">要转换的数字</param>
    /// <returns>格式化后的字符串</returns>
    public static string FormatNumber(this int number)
    {
        if (number < 1000)
            return number.ToString();

        if (number < 1000000) // 千位
        {
            double value = number / 1000.0;
            return FormatWithDecimal(value, "K");
        }
        else // 百万位
        {
            double value = number / 1000000.0;
            return FormatWithDecimal(value, "M");
        }
    }

    private static string FormatWithDecimal(double value, string suffix)
    {
        if (value < 10)
            return $"{value:0.0}{suffix}"; // 一位小数
        else
            return $"{value:0}{suffix}";   // 无小数
    }
}

public static class Vector3Util
{
    public static Vector2 ToVector2IgnoreZ(this Vector3 value)
    {
        return new Vector2(value.x, value.y);
    }

    public static Vector3 ToVector3IgnoreY(this Vector3 value)
    {
        return new Vector3(value.x, 0, value.z);
    }
}

public static class Vector2Util
{
    public static Vector3 ToVector3_XY(this Vector2 value)
    {
        return new Vector3(value.x, value.y, 0);
    }

}

public static class EventTriggerUtil
{
    public static void RegistPointerClickCallback(this EventTrigger trigger, Action<PointerEventData> onAction)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { onAction?.Invoke(data as PointerEventData); });
        trigger.triggers.Add(entry);
    }

    public static void RegistPointerDownCallback(this EventTrigger trigger, Action<PointerEventData> onAction)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) => { onAction?.Invoke(data as PointerEventData); });
        trigger.triggers.Add(entry);
    }
}

public static class ColorUtil
{
    public static Color CreateHDRColor(Color baseColor, float intensity)
    {
        return baseColor * intensity;
    }

    public static string ToHex(this Color color, bool includeAlpha = true)
    {
        return ColorUtility.ToHtmlStringRGBA(color);
    }

    public static string ToHexRGB(this Color color)
    {
        return ColorUtility.ToHtmlStringRGB(color);
    }

    public static Color ToColor(this string hex)
    {
        if (string.IsNullOrEmpty(hex))
            return Color.white;

        if (!hex.StartsWith("#"))
            hex = "#" + hex;

        if (ColorUtility.TryParseHtmlString(hex, out Color color))
            return color;

        Debug.LogWarning($"Failed to parse hex color: {hex}");
        return Color.white;
    }

    public static Color ToColor(this string hex, Color defaultColor)
    {
        if (string.IsNullOrEmpty(hex))
            return defaultColor;

        if (!hex.StartsWith("#"))
            hex = "#" + hex;

        if (ColorUtility.TryParseHtmlString(hex, out Color color))
            return color;

        return defaultColor;
    }

    // 支持的格式：
    // #RGB, #RGBA, #RRGGBB, #RRGGBBAA
    // rgb(255,255,255), rgba(255,255,255,1.0)
    // hsl(360,100%,100%), hsla(360,100%,100%,1.0)

    // 1. 自动检测并转换
    public static Color ParseColor(string input)
    {
        if (string.IsNullOrEmpty(input))
            return Color.white;

        input = input.Trim().ToLower();

        // 十六进制格式
        if (input.StartsWith("#") || IsHexString(input))
        {
            return ParseHexColor(input);
        }
        // RGB/RGBA格式
        else if (input.StartsWith("rgb"))
        {
            return ParseRGBColor(input);
        }
        // HSL/HSLA格式
        else if (input.StartsWith("hsl"))
        {
            return ParseHSLColor(input);
        }
        // 颜色名称（red, blue, etc）
        else if (ColorUtility.TryParseHtmlString(input, out Color namedColor))
        {
            return namedColor;
        }

        Debug.LogWarning($"Unsupported color format: {input}");
        return Color.white;
    }

    private static bool IsHexString(string input)
    {
        // 检查是否是有效的十六进制字符串
        string hex = input.Replace("#", "");
        return Regex.IsMatch(hex, @"^[0-9a-f]{3,8}$", RegexOptions.IgnoreCase);
    }

    private static Color ParseHexColor(string hex)
    {
        if (!hex.StartsWith("#"))
            hex = "#" + hex;

        if (ColorUtility.TryParseHtmlString(hex, out Color color))
            return color;

        return Color.white;
    }

    private static Color ParseRGBColor(string rgb)
    {
        // 解析 rgb(255,255,255) 或 rgba(255,255,255,1.0)
        string pattern = @"rgba?\((\d+),\s*(\d+),\s*(\d+)(?:,\s*([\d.]+))?\)";
        Match match = Regex.Match(rgb, pattern);

        if (match.Success)
        {
            float r = int.Parse(match.Groups[1].Value) / 255f;
            float g = int.Parse(match.Groups[2].Value) / 255f;
            float b = int.Parse(match.Groups[3].Value) / 255f;
            float a = match.Groups[4].Success ? float.Parse(match.Groups[4].Value) : 1f;

            return new Color(r, g, b, a);
        }

        return Color.white;
    }

    private static Color ParseHSLColor(string hsl)
    {
        // HSL 转 RGB 转换（简化版）
        Debug.LogWarning("HSL color conversion not fully implemented");
        return Color.white;
    }
}

public static class AnimationUtil
{
    public static bool PlayAnimationByNameSuffix(this Animation animation, string nameSuffix, out float clipLength)
    {
        clipLength = 0f;
        if (string.IsNullOrEmpty(nameSuffix)) return false;
        var spArr = nameSuffix.Split('_');
        foreach (AnimationState animationState in animation)
        {
            var animSpArr = animationState.clip.name.Split('_');
            if (nameSuffix.Equals(animSpArr[animSpArr.Length - 1]))
            {
                clipLength = animationState.clip.length;
                animation.Play(animationState.name);
                return true;
            }
        }
        return false;
    }
    public static void PlayAnimationByNameSuffix(this List<Animation> animations, string nameSuffix)
    {
        if (animations == null || animations.Count == 0) return;
        foreach (var animation in animations)
        {
            animation.PlayAnimationByNameSuffix(nameSuffix, out var _);
        }
    }
}

public static class AnimatorUtil
{
    // 获取当前动画长度
    public static float GetCurrentAnimationLength(this Animator animator)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length;
    }

    // 获取当前动画进度（0-1）
    public static float GetCurrentAnimationProgress(this Animator animator)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime % 1;
    }

    // 获取特定动画的长度
    public static bool TryGetAnimationLength(this Animator animator, string stateName, out float clipLength)
    {
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;
        foreach (AnimationClip clip in controller.animationClips)
        {
            if (clip.name == stateName)
            {
                clipLength = clip.length;
                return true;
            }
        }
        clipLength = 0f;
        return false;
    }

    public static bool TryAnimationClip(this Animator animator, string stateName, out AnimationClip resultClip)
    {
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;
        foreach (AnimationClip clip in controller.animationClips)
        {
            if (clip.name == stateName)
            {
                resultClip = clip;
                return true;
            }
        }
        resultClip = null;
        return false;
    }

    public static bool HasAnimtionClip(this Animator animator, string stateName)
    {
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;
        foreach (AnimationClip clip in controller.animationClips)
        {
            if (clip.name == stateName)
            {
                return true;
            }
        }
        return false;
    }

}

public static class TextUtil
{
    /// <summary>
    /// 文本逐字显示效果
    /// </summary>
    /// <param name="textComponent">Text或TextMeshProUGUI组件</param>
    /// <param name="targetText">目标文本</param>
    /// <param name="duration">总持续时间</param>
    public static Tweener DoText(this UnityEngine.UI.Text textComponent, string targetText, float duration)
    {
        textComponent.text = "";
        StringBuilder currentText = new StringBuilder();

        return DOTween.To(
            () => 0,
            value =>
            {
                int charCount = Mathf.FloorToInt(value);
                currentText.Clear();

                for (int i = 0; i < charCount && i < targetText.Length; i++)
                {
                    currentText.Append(targetText[i]);
                }

                textComponent.text = currentText.ToString();
            },
            targetText.Length,
            duration
        ).SetEase(Ease.Linear);
    }

    /// <summary>
    /// TextMeshPro版本
    /// </summary>
    public static Tweener DoText(this TMPro.TextMeshProUGUI textComponent, string targetText, float duration)
    {
        textComponent.text = "";
        StringBuilder currentText = new StringBuilder();

        return DOTween.To(
            () => 0,
            value =>
            {
                int charCount = Mathf.FloorToInt(value);
                currentText.Clear();

                for (int i = 0; i < charCount && i < targetText.Length; i++)
                {
                    currentText.Append(targetText[i]);
                }

                textComponent.text = currentText.ToString();
            },
            targetText.Length,
            duration
        ).SetEase(Ease.Linear);
    }
}
public interface IGameObjectPool
{
    Task AsyncInit();
}