using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HomeLogic : MonoSingleton<HomeLogic>
{
    public Canvas gameCanvas;
    public GameObject GamePage;
    public GameObject HomePage;

    public override bool DontDestory => false;

    private async void Start()
    {
        try
        {
            if (gameCanvas != null) UIManager.SetCanvasScaler(gameCanvas);
            await Frameworks.AsyncInit();

            await GamePlay.Instance.PlayLaunch();
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            GlobalSingleton.GetReward(GameAssetType.Coin, 500);
        }
    }

#endif
}


public class GamePlay : Singleton<GamePlay>, IMsgObj
{
    public static bool IsGamePlayStart
    {
        get;
        private set;
    }

    public async Task PlayLaunch()
    {
        AudioManager.AudioPlayer.PlayMusic(SoundName.Bgm);
        IsGamePlayStart = false;
        ShowHomePage();
        await UIManager.OpenMultiUIAsync(
            typeof(PNL_MainPage_Top),
            typeof(PNL_MainPage));


        if (GlobalSingleton.GuideState == 0)
        {
            if (AppExcuteFlagSettings.ToBFlag)
            {
                var takeUI = await UIManager.OpenUIAsync<User_TakeGuide>();
                await takeUI.WaitClose();
            }

            GlobalSingleton.GuideState = 1;

            var ui = await UIManager.OpenUIAsync<User_Guide>(new User_GuideParam { guideIDX = 0 });
            await ui.WaitInteraction();
            ui.Close();
        }
    }

    public void ShowHomePage()
    {
        HomeLogic.Instance.HomePage.SetActive(true);
        HomeLogic.Instance.GamePage.SetActive(false);
    }

    public void ShowGamePage()
    {
        HomeLogic.Instance.HomePage.SetActive(false);
        HomeLogic.Instance.GamePage.SetActive(true);
    }
    [CmdCallback((ushort)GameEvent.GamePlay_Begin)]
    private async Task OnGamePlay_Begin()
    {
        if (IsGamePlayStart) return;
        IsGamePlayStart = true;

        var loadingUI = await UIManager.OpenUIAsync<PNL_Loading>();

        ShowGamePage();
        await UIManager.CloseMultiUIAsyncByUIGroupTag(EUIGroupTag.HomePage);

        await MiniGame.Instance.InitData(GlobalSingleton.Level);

        await loadingUI.AsyncClose();

        await UIManager.OpenMultiUIAsync(typeof(HUB_Topbar), typeof(HUB_MainGamePlay));

        this.SendCommand((ushort)GameEvent.RefreshProgressBar);
        if (GlobalSingleton.GuideState == 1)
        {
            var guideUI = await UIManager.OpenUIAsync<User_Guide>(new User_GuideParam
            {
                guideIDX = 1,
                goods = StorageUnit.Instance.guideTempGoodsList[0]
            });
            await guideUI.WaitInteraction();

            guideUI.ReplaceData(new User_GuideParam
            {
                guideIDX = 1,
                goods = StorageUnit.Instance.guideTempGoodsList[1]
            });


            await guideUI.WaitInteraction();
            guideUI.ReplaceData(new User_GuideParam
            {
                guideIDX = 1,
                goods = StorageUnit.Instance.guideTempGoodsList[2]
            });
            await guideUI.WaitInteraction();
            await guideUI.AsyncClose();

            using (var handle = new AsyncWaitMessage((ushort)GameEvent.EliminateEnd))
            {
                await handle.AsyncWait();
            }

            guideUI = await UIManager.OpenUIAsync<User_Guide>(new User_GuideParam
            {
                guideIDX = 2,
            });
            await guideUI.WaitInteraction();

            //var newSpaUI = await UIManager.OpenUIAsync<User_NewSpace>();
            //await newSpaUI.WaitClose();


            if (AppExcuteFlagSettings.ToBFlag)
            {
                guideUI.ReplaceData(new User_GuideParam { guideIDX = 3 });
                await guideUI.WaitInteraction();
            }
            await guideUI.AsyncClose();
            GlobalSingleton.GuideState = 2;
        }
        //await GamePlay_ShelfGame.Instance.StartGameLevel();
    }

    [CmdCallback((ushort)GameEvent.GamePlay_BackHome)]
    private async Task OnGamePlay_BackHome()
    {
        if (!IsGamePlayStart) return;
        IsGamePlayStart = false;

        GlobalSingleton.GameRunning = false;
        var loadingUI = await UIManager.OpenUIAsync<PNL_Loading>();
        await UIManager.CloseMultiUIAsyncByUIGroupTag(EUIGroupTag.GamePop);

        ShowHomePage();

        await loadingUI.AsyncClose();

        await UIManager.OpenMultiUIAsync(
            typeof(PNL_MainPage_Top),
            typeof(PNL_MainPage));


    }

    //[CmdCallback((ushort)GameEvent.GamePlay_LevelComplete)]
    //private async Task OnGamePlay_LevelComplete()
    //{
    //    GlobalSingleton.Level++;
    //    var ui = await UIManager.OpenUIAsync<PNL_Loading>();
    //    //await GamePlay_ShelfGame.Instance.BuildGameLevel();
    //    await Task.Delay(600);
    //    await ui.AsyncClose();
    //    //await GamePlay_ShelfGame.Instance.StartGameLevel();
    //}
}