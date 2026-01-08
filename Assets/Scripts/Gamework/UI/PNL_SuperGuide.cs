using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[UISetting(UICanvasLayer.Overlay_Camera, backgroundMask: EBackgroundMask.Black_80F)]
public partial class PNL_SuperGuide : UIBase
{
    private string talkString1;
    private string talkString2;
    private string talkString3;

    public override void OnInit()
    {
        base.OnInit();
        talkString1 = TalkText1.text;
        talkString2 = TalkText2.text;
        talkString3 = TalkText3.text;

        TalkText1.text = "";
        TalkText2.text = "";
        TalkText3.text = "";

        TalkText4.transform.localScale = Vector3.zero;
    }

    protected override async Task Show_Internal()
    {
        await PlayAnimation("SuperCoinShowAction_1");
        AudioManager.AudioPlayer.PlayOneShot("Audios/Greet1.mp3");
        await TalkText1.DoText(talkString1, 0.2f).AsyncWaitForCompletion();
        await PlayAnimation("SuperCoinShowAction_2");
        AudioManager.AudioPlayer.PlayOneShot("Audios/Greet2.mp3");
        await TalkText2.DoText(talkString2, 0.2f).AsyncWaitForCompletion();
        await PlayAnimation("SuperCoinShowAction_3");
        AudioManager.AudioPlayer.PlayOneShot("Audios/Greet3.mp3");
        await TalkText3.DoText(talkString3, 0.2f).AsyncWaitForCompletion();
        AudioManager.AudioPlayer.PlayOneShot("Audios/Greet4.mp3");
        await TalkText4.transform.DOScale(1.1f, 0.2f).AsyncWaitForCompletion();
        await TalkText4.transform.DOScale(1f, 0.1f).AsyncWaitForCompletion();
        AudioManager.AudioPlayer.PlayOneShot("Audios/Greet5.mp3");
        await PlayAnimation("SuperCoinShowAction_4");

    }

    protected override async Task Hide_Internal()
    {
        await PlayHide_ScaleMagnifyFadeOut();
    }

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
