using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[UISetting(UICanvasLayer.Overlay_Camera, backgroundMask: EBackgroundMask.Black_80F | EBackgroundMask.CloseUIOnCancelClick | EBackgroundMask.CloseUIOnBackGroundClick, UIGroupTag: EUIGroupTag.GamePop)]
public partial class User_Policy : UIBase
{
    public override void OnInit()
    {
        base.OnInit();

        switch (LanguageTextManager.CurrentLanguage)
        {
            case LanguageTextManager.E_LanguageType.EN:
                LanguageContent.Find("en").gameObject.SetActive(true);
                break;
            case LanguageTextManager.E_LanguageType.JA:
                LanguageContent.Find("ja").gameObject.SetActive(true);
                break;
            case LanguageTextManager.E_LanguageType.KO:
                LanguageContent.Find("ko").gameObject.SetActive(true);
                break;
            case LanguageTextManager.E_LanguageType.ES:
                LanguageContent.Find("es").gameObject.SetActive(true);
                break;
            case LanguageTextManager.E_LanguageType.PT:
                LanguageContent.Find("pt").gameObject.SetActive(true);
                break;
            case LanguageTextManager.E_LanguageType.DE:
                LanguageContent.Find("de").gameObject.SetActive(true);
                break;
            case LanguageTextManager.E_LanguageType.FR:
                LanguageContent.Find("fr").gameObject.SetActive(true);
                break;
            case LanguageTextManager.E_LanguageType.RU:
                LanguageContent.Find("ru").gameObject.SetActive(true);
                break;
            default:
                LanguageContent.Find("en").gameObject.SetActive(true);
                break;
        }
    }

    protected override void OnShowed()
    {
        base.OnShowed();

        btnClose.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.Play(SoundName.UIClick);
            Close();
        });
    }
}
