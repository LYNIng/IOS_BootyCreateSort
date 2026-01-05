using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[PreloadAssets(
    "Sprites/Signin/tb-ch.png", "Sprites/Signin/tb-coin.png", "Sprites/Signin/tb-wash.png", "Sprites/Signin/tb-xxyz.png")]
[UISetting(UICanvasLayer.Popup_Camera, backgroundMask: EBackgroundMask.Black_80F | EBackgroundMask.CloseUIOnCancelClick | EBackgroundMask.CloseUIOnBackGroundClick, UIGroupTag: EUIGroupTag.GamePop)]
public partial class PNL_Signin : UIBase
{
    public CMP_SigninItem[] signinItems;
    public override void OnInit()
    {
        base.OnInit();

        RefreshShow();
    }

    protected override void OnShowed()
    {
        base.OnShowed();

        btnClose.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            Close();
        });
    }

    private void RefreshShow()
    {
        for (int i = 0; i < signinItems.Length; i++)
        {
            var item = signinItems[i];

            item.Set(i, OnClick, this);
        }
    }

    private void OnClick(int idx)
    {
        Debug.Log($"Click {idx}");
        SignInRecordManager.Instance.SetSignInState(idx);
        var rewards = GlobalSingleton.GetSigninRewards();
        if (idx < 6)
        {
            var item = rewards[idx];
            GlobalSingleton.GetReward(item.assetType, item.rewardCnt);
        }
        else
        {
            var item1 = rewards[6];
            var item2 = rewards[7];
            var item3 = rewards[8];
            var item4 = rewards[9];

            GlobalSingleton.GetReward(item1.assetType, item1.rewardCnt);
            GlobalSingleton.GetReward(item2.assetType, item2.rewardCnt);
            GlobalSingleton.GetReward(item3.assetType, item3.rewardCnt);
            GlobalSingleton.GetReward(item4.assetType, item4.rewardCnt);
        }
        RefreshShow();
    }

    public bool TryGetSprite(GameAssetType gameAssetType, out Sprite resultSprite)
    {
        if (gameAssetType == GameAssetType.Coin)
        {
            return TryGetPreloadAsset("tb-coin", out resultSprite);
        }
        else if (gameAssetType == GameAssetType.RemoveTool)
        {
            return TryGetPreloadAsset("tb-xxyz", out resultSprite);
        }
        else if (gameAssetType == GameAssetType.RefreshTool)
        {
            return TryGetPreloadAsset("tb-wash", out resultSprite);
        }
        else if (gameAssetType == GameAssetType.BackTool)
        {
            return TryGetPreloadAsset("tb-ch", out resultSprite);
        }
        resultSprite = null;
        return false;
    }
}
