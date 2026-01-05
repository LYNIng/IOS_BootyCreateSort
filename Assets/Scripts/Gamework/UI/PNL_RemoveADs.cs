using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PNL_RemoveADs : UIBase
{


    public override void OnInit()
    {
        base.OnInit();



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
    }

    private void RefreshShow()
    {
        txtHaveADs.text = $"You have ADs:{ShowADManager.ShowADCount}";
        txtNeedADs.text = $"Need ADs:{15 - ShowADManager.ShowADCount}";
    }
}
