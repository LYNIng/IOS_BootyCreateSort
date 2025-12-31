using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[UISetting(UICanvasLayer.Popup_Camera, backgroundMask: EBackgroundMask.Black_80F | EBackgroundMask.CloseUIOnCancelClick | EBackgroundMask.CloseUIOnBackGroundClick, UIGroupTag: EUIGroupTag.GamePop)]
public partial class PNL_Thanks : UIBase
{
    private bool THKSwitch
    {
        get
        {
            return DataManager.GetDataByBool("THKSwitch", true);
        }
        set
        {
            DataManager.SetDataByBool("THKSwitch", value);
        }
    }

    public override void OnInit()
    {
        base.OnInit();
        var isFirst = THKSwitch;
        imaCA.gameObject.SetActive(isFirst);
        imaFinger.gameObject.SetActive(!isFirst);
        txtValue.gameObject.SetActive(isFirst);

        if (isFirst)
        {
            txtDec.text = 1012.ToMultiLanguageText();
        }
        else
        {
            txtDec.text = 1013.ToMultiLanguageText();
        }


    }

    protected override void OnShowed()
    {
        base.OnShowed();
        btnClick.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);

            if (THKSwitch)
                _ = GlobalSingleton.AsyncGetReward(GameAssetType.SuperCoin, 1000, startPos: imaCA.transform.position);
            THKSwitch = false;
            Close();
        });

        btnClose.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            Close();
        });
    }
}
