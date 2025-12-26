using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UISetting(UICanvasLayer.Overlay_Camera, backgroundMask: EBackgroundMask.Black_80F)]
public partial class User_TakeGuide : UIBase
{
    protected override void OnShowed()
    {
        base.OnShowed();

        btnClick.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            Close();
        });
    }
}
