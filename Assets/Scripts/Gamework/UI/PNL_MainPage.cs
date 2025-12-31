using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[UISetting(UICanvasLayer.Main_Camera)]
public partial class PNL_MainPage : UIBase
{
    protected override async Task Show_Internal()
    {
        await PlayShow_ScaleLessenAndFadeIn();
    }

    protected override async Task Hide_Internal()
    {
        await PlayHide_ScaleMagnifyFadeOut();
    }

    protected override void OnShowed()
    {
        btnPlay.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            this.SendCommand((ushort)GameEvent.GamePlay_Begin);
        });

        btnTask.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            UIManager.OpenUI<User_Tasks>();
        });

        btnShop.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            UIManager.OpenUI<PNL_OpenShop>();
        });

        btnSettings.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            UIManager.OpenUI<PNL_Settings>();
        });
    }
}
