using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

public class UIADTestParam : UIData
{
    /// <summary>
    // AdScene
    /// </summary>
    public string mADTag;
    /// <summary>
    /// AdType
    /// </summary>
    public string mADT;
    /// <summary>
    /// callback
    /// </summary>
    public Action<int, string> onResult;
}

[UISetting(UICanvasLayer.System_Camera)]
public class UIADTest : UIBase<UIADTestParam>
{
    public TextMeshProUGUI txtTagInfo;
    public Button btnSuc;
    public Button btnFail;

    public override void OnInit()
    {
        txtTagInfo.text = $"AdScene: {Data.mADTag}\n\nAdType: {Data.mADT}";
        btnSuc.onClick.AddListener(() =>
        {
            UIManager.CloseUI(this);
            Data.onResult(0, "");
        });
        btnFail.onClick.AddListener(() =>
        {
            UIManager.CloseUI(this);
            Data.onResult(1000, "Mock AD Failed");
        });
    }

    protected override Task Show_Internal()
    {
        return Task.CompletedTask;
    }

    protected override Task Hide_Internal()
    {
        return Task.CompletedTask;
    }
}
