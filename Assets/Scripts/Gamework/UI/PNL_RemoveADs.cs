using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[UISetting(UICanvasLayer.Popup_Camera,
    backgroundMask: EBackgroundMask.Black_80F | EBackgroundMask.CloseUIOnCancelClick | EBackgroundMask.CloseUIOnBackGroundClick)]
public partial class PNL_RemoveADs : UIBase
{


    public override void OnInit()
    {
        base.OnInit();

        RefreshShow();
    }

    protected override void OnShowed()
    {
        base.OnShowed();
        btnClick.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            ShowADManager.PlayVideoAD("PNL_RemoveADs", (code, msg) =>
            {
                if (code > 0)
                {
                    UIManager.ShowToast(msg);
                    return;
                }
                if (ShowADManager.PlusPlayADCount())
                {
                    Close();
                }
                else
                    RefreshShow();
            });
        });

        btnClose.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            Close();
        });
    }

    private void RefreshShow()
    {
        txtHaveADs.text = $"You have ADs:{ShowADManager.ShowADCount}";
        txtNeedADs.text = $"Need ADs:{15 - ShowADManager.ShowADCount}";
    }
}
