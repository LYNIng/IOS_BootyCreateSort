using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UISetting(UICanvasLayer.Overlay_Camera, backgroundMask: EBackgroundMask.Black_80F)]
public partial class User_FailPop : UIBase
{
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
