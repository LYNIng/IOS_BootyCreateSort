using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UISetting(UICanvasLayer.Overlay_Camera, backgroundMask: EBackgroundMask.Black_80F)]
public partial class PNL_Gameover : UIBase
{
    public override void OnInit()
    {
        base.OnInit();

        txtLv.text = GlobalSingleton.Level.ToString();
    }

    protected override void OnShowed()
    {
        base.OnShowed();

        btnRestart.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);

            Close();
        });
    }
}
