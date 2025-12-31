using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[UISetting(UICanvasLayer.Popup_Camera, backgroundMask: EBackgroundMask.Black_80F, UIGroupTag: EUIGroupTag.GamePop)]
public partial class PNL_Restar : UIBase
{

    bool result = false;
    public override void OnInit()
    {
        base.OnInit();
        btnADClick.gameObject.SetActive(AppExcuteFlagSettings.ToAFlag);
        btnClick.gameObject.SetActive(!AppExcuteFlagSettings.ToAFlag);
    }

    protected override void OnShowed()
    {
        base.OnShowed();

        btnADClick.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            ShowADManager.PlayVideoAD("PNL_Restar", (code, msg) =>
            {
                if (code > 0)
                {
                    UIManager.ShowToast(msg);
                    return;
                }
                result = true;
                Close();
            });
        });

        btnClick.RegistBtnCallback(async () =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            if (GlobalSingleton.Coin >= 100)
            {
                await GlobalSingleton.CostAsset(GameAssetType.Coin, 100);
                result = true;
                Close();
            }
            else
            {
                UIManager.ShowToast(1001.ToMultiLanguageText());
            }
        });

        btnClose.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            Close();
        });
    }

    public override async Task<object> WaitClose()
    {
        await base.WaitClose();
        return result;
    }
}
