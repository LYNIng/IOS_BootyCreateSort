using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UISetting(UICanvasLayer.Overlay_Camera,
    backgroundMask: EBackgroundMask.Black_80F | EBackgroundMask.CloseUIOnBackGroundClick | EBackgroundMask.CloseUIOnCancelClick,
    UIGroupTag: EUIGroupTag.GamePop)]
public partial class PNL_Settings : UIBase
{
    public override void OnInit()
    {
        base.OnInit();

        btnBack.gameObject.SetActive(GlobalSingleton.GameRunning);

        togSound.SetIsOnWithoutNotify(!AudioManager.Instance.IsSFXMuted());
        togMusic.SetIsOnWithoutNotify(!AudioManager.Instance.IsMusicMuted());
    }

    protected override void OnShowed()
    {
        base.OnShowed();

        btnPP.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            UIManager.OpenUI<PNL_GamePolicy>();
        });

        btnCU.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            UIManager.OpenUI<PNL_Contact>();
        });

        btnBack.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            this.SendCommand((ushort)GameEvent.GamePlay_BackHome);
        });

        togMusic.RegistToggleCallback((tog, flag) =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            AudioManager.AudioPlayer.SetMusicMuted(!flag);
        });

        togSound.RegistToggleCallback((tog, flag) =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            AudioManager.AudioPlayer.SetSFXMuted(!flag);
        });

        btnClose.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            Close();
        });
    }
}
