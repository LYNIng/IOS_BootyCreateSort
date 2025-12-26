using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.MaterialProperty;

[UISetting(UICanvasLayer.Popup_Camera, backgroundMask: EBackgroundMask.Black_80F | EBackgroundMask.CloseUIOnCancelClick | EBackgroundMask.CloseUIOnBackGroundClick, UIGroupTag: EUIGroupTag.GamePop)]
public partial class User_Thanks : UIBase
{
    private bool IsFirstThank
    {
        get
        {
            return DataManager.GetDataByBool("IsFirstThank", true);
        }
        set
        {
            DataManager.SetDataByBool("IsFirstThank", value);
        }
    }

    public override void OnInit()
    {
        base.OnInit();
        var isFirst = IsFirstThank;
        imaCA.gameObject.SetActive(isFirst);
        imaFinger.gameObject.SetActive(!isFirst);
        txtValue.gameObject.SetActive(isFirst);

        if (isFirst)
        {
            txtDec.text = 1012.ToMultiLanguageText();
        }
        else
        {
            txtDec.text = 1013.ToMultiLanguageText();
        }


    }

    protected override void OnShowed()
    {
        base.OnShowed();
        btnClick.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);

            if (IsFirstThank)
                _ = GlobalSingleton.AsyncGetReward(GameAssetType.SuperCoin, 1000, startPos: imaCA.transform.position);
            IsFirstThank = false;
            Close();
        });

        btnClose.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            Close();
        });
    }
}
