using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User_BuyPropsParam : UIData
{
    public GameAssetType gameAssetType;
}
[UISetting(UICanvasLayer.Popup_Camera,
    backgroundMask: EBackgroundMask.Black_80F | EBackgroundMask.CloseUIOnBackGroundClick | EBackgroundMask.CloseUIOnCancelClick,
    UIGroupTag: EUIGroupTag.GamePop)]
public partial class User_BuyProps : UIBase<User_BuyPropsParam>
{
    public override void OnInit()
    {
        base.OnInit();

        if (AppExcuteFlagSettings.ToAFlag)
        {
            if (GlobalSingleton.Coin >= 100)
            {
                btnClick.gameObject.SetActive(true);
                btnADClick.gameObject.SetActive(false);
            }
            else
            {
                btnClick.gameObject.SetActive(false);
                btnADClick.gameObject.SetActive(true);
            }
        }
        else
        {
            btnClick.gameObject.SetActive(true);
            btnADClick.gameObject.SetActive(false);
        }

        RefreshAssetType();
    }

    protected override void OnShowed()
    {
        base.OnShowed();
        btnClose.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            Close();
        });

        btnClick.RegistBtnCallback(async () =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            if (GlobalSingleton.Coin >= 100)
            {
                await GlobalSingleton.CostAsset(GameAssetType.Coin, 100);
                GlobalSingleton.GetReward(Data.gameAssetType, 1);
                Close();
            }
            else
            {
                UIManager.ShowToast(1001.ToMultiLanguageText());
            }
        });

        btnADClick.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            ShowADManager.PlayVideoAD("User_BuyProps", (code, msg) =>
            {
                if (code > 0)
                {
                    UIManager.ShowToast(msg);
                    return;
                }
                GlobalSingleton.GetReward(Data.gameAssetType, 1);
                Close();
            });
        });
    }

    private void RefreshAssetType()
    {
        imaTool1.gameObject.SetActive(false);
        imaTool2.gameObject.SetActive(false);
        imaTool3.gameObject.SetActive(false);
        switch (Data.gameAssetType)
        {
            case GameAssetType.RemoveTool:
                imaTool1.gameObject.SetActive(true);
                txtDec.text = 1006.ToMultiLanguageText();
                break;
            case GameAssetType.BackTool:
                imaTool2.gameObject.SetActive(true);
                txtDec.text = 1007.ToMultiLanguageText();
                break;
            case GameAssetType.RefreshTool:
                imaTool3.gameObject.SetActive(true);
                txtDec.text = 1008.ToMultiLanguageText();
                break;
        }
    }
}
