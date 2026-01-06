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

    private int lastSelectIDX
    {
        get
        {
            return DataManager.GetDataByInt("lastSelectIDX", 0);
        }
        set
        {
            DataManager.SetDataByInt("lastSelectIDX", value);
        }
    }

    public override void OnInit()
    {
        ShowIDX(lastSelectIDX);
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
            CallBuy();
        });

        btnSelect1.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            SelectItem(0);
        });
        btnSelect2.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            SelectItem(1);
        });
        btnSelect3.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            SelectItem(2);
        });
    }

    private void SelectItem(int idx)
    {
        HideIDX(lastSelectIDX);
        ShowIDX(idx);
    }

    private void CallBuy()
    {
        var cost = costValueArr[lastSelectIDX];
        if (GlobalSingleton.Coin >= cost)
        {
            _ = GlobalSingleton.CostAsset(GameAssetType.Coin, cost);
        }
        else
        {
            UIManager.ShowToast(1001.ToMultiLanguageText());
        }
    }

    private void HideIDX(int idx)
    {
        var item = itemArray[idx];

        item.Find("Shelf_1").gameObject.SetActive(true);
        item.Find("Shelf_2").gameObject.SetActive(false);

    }

    private void ShowIDX(int idx)
    {
        lastSelectIDX = idx;
        var item = itemArray[idx];

        item.Find("Shelf_1").SetActive(false);
        item.Find("Shelf_2").SetActive(true);
        item.ClickScaleAni(null, intensity: 0.5f);
        txtValue.text = costValueArr[idx].ToString();

    }
}
