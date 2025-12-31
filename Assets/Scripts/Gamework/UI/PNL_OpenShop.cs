using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UISetting(UICanvasLayer.Popup_Camera,
    backgroundMask: EBackgroundMask.Black_80F | EBackgroundMask.CloseUIOnBackGroundClick | EBackgroundMask.CloseUIOnCancelClick,
    UIGroupTag: EUIGroupTag.GamePop)]
public partial class PNL_OpenShop : UIBase
{
    protected override void OnShowed()
    {
        btnBuyClick1.RegistBtnCallback(async () =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            if (GlobalSingleton.Coin >= 200)
            {
                await GlobalSingleton.CostAsset(GameAssetType.Coin, 200);

                GlobalSingleton.GetReward(GameAssetType.RemoveTool, 2);
                GlobalSingleton.GetReward(GameAssetType.BackTool, 2);
                GlobalSingleton.GetReward(GameAssetType.RefreshTool, 2);
            }
            else
            {
                UIManager.ShowToast(1001.ToMultiLanguageText());
            }
        });

        btnBuyClick2.RegistBtnCallback(async () =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            if (GlobalSingleton.Coin >= 300)
            {
                await GlobalSingleton.CostAsset(GameAssetType.Coin, 300);

                GlobalSingleton.GetReward(GameAssetType.RemoveTool, 3);
                GlobalSingleton.GetReward(GameAssetType.BackTool, 3);
                GlobalSingleton.GetReward(GameAssetType.RefreshTool, 3);
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
}
