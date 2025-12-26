using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[UISetting(UICanvasLayer.Popup_Camera,
    backgroundMask: EBackgroundMask.Black_80F | EBackgroundMask.CloseUIOnBackGroundClick | EBackgroundMask.CloseUIOnCancelClick,
    UIGroupTag: EUIGroupTag.GamePop)]
public partial class User_NewSpace : UIBase
{
    private bool result;

    public override void OnInit()
    {
        base.OnInit();

        if (AppExcuteFlagSettings.ToAFlag)
        {
            if (GlobalSingleton.Coin >= 100)
            {
                btnClick.gameObject.SetActive(true);
                btnADClick.gameObject.SetActive(false);
            }
            else
            {
                btnClick.gameObject.SetActive(false);
                btnADClick.gameObject.SetActive(true);
            }
        }
        else
        {
            btnClick.gameObject.SetActive(true);
            btnADClick.gameObject.SetActive(false);
        }
    }

    public override async Task<object> WaitClose()
    {
        await base.WaitClose();
        return result;
    }

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
            if (GlobalSingleton.Coin >= 100)
            {
                _ = GlobalSingleton.CostAsset(GameAssetType.Coin, 100);
                this.SendCommand((ushort)GameEvent.UnlockNewSpace);
                Close();
            }
            else
            {
                UIManager.ShowToast(1001.ToMultiLanguageText());
            }
        });
    }
}
