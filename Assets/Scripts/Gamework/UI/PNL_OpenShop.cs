using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[UISetting(UICanvasLayer.Popup_Camera,
    backgroundMask: EBackgroundMask.Black_80F | EBackgroundMask.CloseUIOnBackGroundClick | EBackgroundMask.CloseUIOnCancelClick,
    UIGroupTag: EUIGroupTag.GamePop)]
public partial class PNL_OpenShop : UIBase
{
    public Transform[] itemArray;


    private int[] costValueArr = { 100, 150, 200 };

    public int CurCostCnt
    {
        get
        {
            return DataManager.GetDataByInt("ShopCostCnt", 0);
        }
        set
        {
            DataManager.SetDataByInt("ShopCostCnt", value);
        }
    }

    public override void OnInit()
    {
        RefreshShow();
    }

    protected override void OnShowed()
    {

        btnClose.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            Close();
        });

        btnBuyClick.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);

        });
    }

    private void RefreshShow()
    {
        var idx = CurCostCnt;
        if (idx >= CurCostCnt) return;
        for (int i = 0; i < itemArray.Length; ++i)
        {
            var item = itemArray[i];
            var txt = item.Find("Items/Price/txtPrice").GetComponent<TextMeshProUGUI>();
            if (idx > i)
                txt.text = "Sell Out";
            else txt.text = costValueArr[i].ToString();

            item.Find("Shelf_1").gameObject.SetActive(idx != i);
            item.Find("Shelf_2").gameObject.SetActive(idx == i);

        }

        var rootTrans = itemArray[idx];
        txtValue.text = costValueArr[idx].ToString();

    }
}
