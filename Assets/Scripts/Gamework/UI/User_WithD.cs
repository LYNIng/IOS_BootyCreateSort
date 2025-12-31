using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UISetting(UICanvasLayer.Popup_Camera, backgroundMask: EBackgroundMask.Black_80F | EBackgroundMask.CloseUIOnCancelClick | EBackgroundMask.CloseUIOnBackGroundClick, UIGroupTag: EUIGroupTag.GamePop)]
public partial class User_WithD : UIBase
{
    protected override void OnShowed()
    {
        base.OnShowed();

        btnClose.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            Close();
        });
        btnClick.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            UIManager.ShowToast(1010.ToMultiLanguageText());
        });
        btnQuest.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            UIManager.OpenUI<PNL_GamePolicy>();
        });
    }
}
