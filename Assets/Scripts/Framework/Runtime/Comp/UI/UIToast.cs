using DG.Tweening;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIToastParam : UIData
{
    public UIToast.ToastType toastType = UIToast.ToastType.Default;
    public string msg;
    public Vector3? toastPos = null;
}

[UISetting(UICanvasLayer.System_Camera)]
public class UIToast : UIBase<UIToastParam>
{
    public enum ToastType
    {
        Default,
        Ca,
        Coin
    }

    private GameObject txtToast;

    public override void OnInit()
    {
        base.OnInit();

        txtToast = transform.GetChild((int)Data.toastType).gameObject;
        var toast = txtToast.GetComponentInChildren<TextMeshProUGUI>();

        toast.text = Data.msg;
        txtToast.transform.localScale = Vector3.one;
    }

    protected override async Task Show_Internal()
    {
        txtToast.SetActive(true);
        if (Data.toastPos.HasValue)
            txtToast.transform.position = Data.toastPos.Value;
        LayoutRebuilder.ForceRebuildLayoutImmediate(txtToast.GetComponent<RectTransform>());
        var seq = DOTween.Sequence();
        seq.Append(txtToast.transform.DOScale(Vector3.one, 0.2f));
        seq.Join(transform.SetFade(0f).DOFade(1f, 0.2f));
        seq.AppendInterval(0.5f);
        seq.Append(txtToast.GetComponent<RectTransform>().DoFadeAndAnchorsMoveYTo(300f, 0.5f).SetEase(Ease.OutQuad));
        seq.AppendCallback(() => Close());

        await seq.AsyncWaitForCompletion();
    }

    protected override async Task Hide_Internal()
    {
        await transform.GetOrAddComponent<CanvasGroup>().DOFade(0, 0.2f).AsyncWaitForCompletion();
    }

    protected override void OnShowed()
    {
        Close();
    }


}