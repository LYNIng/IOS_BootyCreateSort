using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[UISetting(UICanvasLayer.Main_Camera, UIGroupTag: EUIGroupTag.HomePage)]
public partial class PNL_MainPage : UIBase, IMsgObj
{
    protected override async Task Show_Internal()
    {
        await PlayShow_ScaleLessenAndFadeIn();
    }

    protected override async Task Hide_Internal()
    {
        await PlayHide_ScaleMagnifyFadeOut();
    }

    public override void OnInit()
    {
        base.OnInit();
        RefreshADFREE();
    }

    protected override void OnShowed()
    {
        btnPlay.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            this.SendCommand((ushort)GameEvent.GamePlay_Begin);
        });

        btnSignin.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            UIManager.OpenUI<PNL_Signin>();
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

        btnADFREE.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            UIManager.OpenUI<PNL_RemoveADs>();
        });
    }
    [CmdCallback((ushort)GameEvent.RefreshADFREE)]
    private void RefreshADFREE()
    {
        btnADFREE.gameObject.SetActive(!ShowADManager.IsSkipAD);
    }
}
