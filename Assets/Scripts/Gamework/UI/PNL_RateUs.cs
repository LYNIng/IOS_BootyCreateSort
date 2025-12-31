using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[UISetting(UICanvasLayer.Main_Camera,
    backgroundMask: EBackgroundMask.Black_80F | EBackgroundMask.CloseUIOnCancelClick | EBackgroundMask.CloseUIOnCancelClick)]
public partial class PNL_RateUs : UIBase
{
    private int curIndex = 2;

    public override void OnInit()
    {
        base.OnInit();

        for (int i = 0; i < rtStars.childCount; i++)
        {
            var idx = i;
            var rt = rtStars.GetChild(idx);
            rt.GetChild(0).gameObject.SetActive(i <= curIndex);
            var btn = rt.GetOrAddComponent<Button>();
            btn.RegistBtnCallback(() =>
            {
                AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
                ClickStar(idx);
            });
        }
    }

    private void ClickStar(int idx)
    {
        AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
        curIndex = idx;

        for (int i = 0; i < rtStars.childCount; i++)
        {
            var rt = rtStars.GetChild(i);
            rt.GetChild(0).gameObject.SetActive(i <= curIndex);
        }
    }

    protected override void OnShowed()
    {
        base.OnShowed();
        btnClick.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            if (curIndex >= 3)
            {
                Application.OpenURL(AppExcuteFlagSettings.RateURL);
            }
            Close();
        });

        btnClose.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            Close();
        });
    }
}
