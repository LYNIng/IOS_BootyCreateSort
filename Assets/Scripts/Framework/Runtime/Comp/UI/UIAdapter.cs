using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class UIAdapter : MonoBehaviour, IUIBaseInit
{
    public enum RunMode
    {
        OnUIBaseInit,
        OnStart,
    }

    public enum SafeAreaType
    {
        Top,
        Bottom,
    }

    public enum AdapterType
    {
        Size,
        Pos,
        OffsetTop,
    }

    public SafeAreaType safeAreaType = SafeAreaType.Top;
    public AdapterType adapterType = AdapterType.Size;

    public RunMode runMode = RunMode.OnUIBaseInit;

    private async void Start()
    {
        if (runMode == RunMode.OnStart)
        {
            await Excute();
        }
    }

    public async Task Excute()
    {
        var mainCanvas = GetComponentInParent<Canvas>();
        //Debug.Log("here ==> 1");
        while (mainCanvas == null)
        {
            await Task.Yield();
            mainCanvas = GetComponentInParent<Canvas>();

        }
        //Debug.Log("here ==> 2");

        var rectTrans = GetComponent<RectTransform>();
        Rect safeArea = Screen.safeArea;

        var rt = mainCanvas.GetComponent<RectTransform>();

        if (safeAreaType == SafeAreaType.Top)
        {

            float topInset = (Screen.height - (safeArea.y + safeArea.height)) * (rt.sizeDelta.y / Screen.height);
            if (topInset > 0)
            {
                if (adapterType == AdapterType.Size)
                {
                    rectTrans.sizeDelta = new Vector2(
                        rectTrans.sizeDelta.x,
                        rectTrans.sizeDelta.y + topInset);
                }
                else if (adapterType == AdapterType.Pos)
                {
                    rectTrans.anchoredPosition = new Vector2(rectTrans.anchoredPosition.x,
                                                    rectTrans.anchoredPosition.y - topInset);
                }
                else if (adapterType == AdapterType.OffsetTop)
                {
                    rectTrans.offsetMax = new Vector2(rectTrans.offsetMax.x, rectTrans.offsetMax.y - topInset);
                }
            }
        }
        else
        {
            float bottomInset = safeArea.y * (rt.sizeDelta.y / Screen.height);
            if (bottomInset > 0)
            {
                if (adapterType == AdapterType.Size)
                {
                    rectTrans.sizeDelta = new Vector2(
                        rectTrans.sizeDelta.x,
                        rectTrans.sizeDelta.y + bottomInset);
                }
                else if (adapterType == AdapterType.Pos)
                {
                    rectTrans.anchoredPosition = new Vector2(rectTrans.anchoredPosition.x,
                                                    rectTrans.anchoredPosition.y + bottomInset);
                }
                else if (adapterType == AdapterType.OffsetTop)
                {
                    rectTrans.offsetMin = new Vector2(rectTrans.offsetMax.x, rectTrans.offsetMax.y - bottomInset);
                }
            }
        }

    }



    public static float GetTopOffset(RectTransform target)
    {
        var mainCanvas = target.GetComponentInParent<Canvas>();
        if (mainCanvas == null)
        {
            return 0f;
        }
        var rt = mainCanvas.GetComponent<RectTransform>();
        Rect safeArea = Screen.safeArea;
        float topInset = (Screen.height - (safeArea.y + safeArea.height)) * (rt.sizeDelta.y / Screen.height);
        return topInset;
    }

    public void OnUIBaseInitBefore()
    {
        if (runMode == RunMode.OnUIBaseInit)
        {
            _ = Excute();
        }
    }
}
