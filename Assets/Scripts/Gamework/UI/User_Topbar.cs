using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[PreloadAssets(
    new object[] {
        "Prefabs/UIComp/SuperCoinElement.prefab" },
    "Prefabs/UIComp/CoinElement.prefab",
    "Prefabs/UIComp/LevelElement.prefab"
    )]
[UISetting(UICanvasLayer.Overlay_Camera)]
public partial class User_Topbar : UIBase
{
    protected override async Task Show_Internal()
    {
        await PlayShow_OffsetMoveBySizeY();
    }

    protected override async Task Hide_Internal()
    {
        await PlayHide_OffsetMoveBySizeY();
    }

    private Action onClickBackCallback;

    public override void OnInit()
    {
        base.OnInit();

        if (AppExcuteFlagSettings.ToBFlag)
        {
            if (TryGetPreloadAsset("SuperCoinElement", out GameObject result))
            {
                result.SpawnNewOne(rtElements).SetActive(true);
            }
            if (TryGetPreloadAsset("CoinElement", out result))
            {
                result.SpawnNewOne(rtElements).SetActive(true);
            }
        }
        else
        {
            if (TryGetPreloadAsset("CoinElement", out GameObject result))
            {
                result.SpawnNewOne(rtElements).SetActive(true);
            }
            if (TryGetPreloadAsset("LevelElement", out result))
            {
                result.SpawnNewOne(rtElements).SetActive(true);
            }
        }
    }

    protected override void OnShowed()
    {
        base.OnShowed();

        btnSetting.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            UIManager.OpenUI<PNL_Settings>();
        });

        btnBack.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            onClickBackCallback?.Invoke();
        });
    }


    public async void ShowBackTopBar(Action onClickBack)
    {
        await PlayHide_OffsetMoveBySizeY();
        btnSetting.gameObject.SetActive(false);
        btnBack.gameObject.SetActive(true);
        onClickBackCallback = onClickBack;
        await Play_MoveYTo(0, 0.2f);
    }

    public async void ShowNormalTopBar()
    {
        await PlayHide_OffsetMoveBySizeY();
        btnSetting.gameObject.SetActive(true);
        btnBack.gameObject.SetActive(false);
        onClickBackCallback = null;
        await Play_MoveYTo(0, 0.2f);
    }
}
