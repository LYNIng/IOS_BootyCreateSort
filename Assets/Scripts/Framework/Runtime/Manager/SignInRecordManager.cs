using System;

public partial class GlobalSingleton
{
    public int CurrentSignInState
    {
        get
        {
            return DataManager.GetDataByInt("Asset_Signin", 0);
        }
        set
        {
            DataManager.SetDataByInt("Asset_Signin", value);
        }
    }

    public int SignInLastDayState
    {
        get
        {
            return DataManager.GetDataByInt("Asset_Signin_Last_Day", 0);
        }
        set
        {
            DataManager.SetDataByInt("Asset_Signin_Last_Day", value);
        }
    }

    public static (GameAssetType assetType, int rewardCnt)[] GetSigninRewards()
    {
        return AppExcuteFlagSettings.ToBFlag ? SigninRewardsExSetting : SigninRewardsSetting;
    }

    public static (GameAssetType assetType, int rewardCnt)[] SigninRewardsSetting = new (GameAssetType assetType, int rewardCnt)[]
    {
        (GameAssetType.Coin,50),//1
        (GameAssetType.Coin,100),//2
        (GameAssetType.Coin,150),//3
        (GameAssetType.Coin,300),//4
        (GameAssetType.Coin,500),//5
        (GameAssetType.Coin,1000),//6

        (GameAssetType.Coin,3),//7-1
        (GameAssetType.BackTool,3),//7-2
        (GameAssetType.RefreshTool,3),//7-3
        (GameAssetType.RefreshTool,3),//7-3

    };

    public static (GameAssetType assetType, int rewardCnt)[] SigninRewardsExSetting = new (GameAssetType assetType, int rewardCnt)[]
{
        (GameAssetType.Coin,50),//1
        (GameAssetType.Coin,100),//2
        (GameAssetType.Coin,150),//3
        (GameAssetType.Coin,300),//4
        (GameAssetType.Coin,500),//5
        (GameAssetType.Coin,1000),//6

        (GameAssetType.Coin,3),//7-1
        (GameAssetType.BackTool, 3),//7-2
        (GameAssetType.RefreshTool,3),//7-3
        (GameAssetType.RefreshTool,3),//7-3

};

}

public class SignInRecordManager : Singleton<SignInRecordManager>
{
    public int GetSignInState(int inputDay)
    {
        int year = DateTime.Now.Year * 10000;
        int month = DateTime.Now.Month * 100;
        int day = DateTime.Now.Day;
        int signInDay = year + month + day;

        var _curSignIn = GlobalSingleton.Instance.CurrentSignInState;
        var _signInLastDay = GlobalSingleton.Instance.SignInLastDayState;

        if (_curSignIn > inputDay)
        {
            //拿过的
            return 2;
        }
        else if (_curSignIn == inputDay && signInDay > _signInLastDay)
        {
            //能拿的
            return 1;
        }
        else
        {
            //不能拿的
            return 0;
        }
    }

    public bool SetSignInState(int signDay)
    {
        int year = DateTime.Now.Year * 10000;
        int month = DateTime.Now.Month * 100;
        int day = DateTime.Now.Day;
        int signInDay = year + month + day;

        var _curSignIn = GlobalSingleton.Instance.CurrentSignInState;
        var _signInLastDay = GlobalSingleton.Instance.SignInLastDayState;


        if (signInDay > _signInLastDay && _curSignIn == signDay)
        {
            _signInLastDay = signInDay;
            _curSignIn = signDay;

            GlobalSingleton.Instance.CurrentSignInState = _curSignIn + 1;
            GlobalSingleton.Instance.SignInLastDayState = _signInLastDay;

            MessageDispatch.CallMessageCommand((ushort)FrameworksMsg.SigninRefresh);
            RedPointManager.Instance.RefreshRedPoint(RedPointManager.IDS.RP_Task);
            RedPointManager.Instance.RefreshRedPoint(RedPointManager.IDS.RP_Signin);
            return true;
        }
        return false;
    }

    public bool CanShowSignInRedPoint()
    {
        int year = DateTime.Now.Year * 10000;
        int month = DateTime.Now.Month * 100;
        int day = DateTime.Now.Day;
        int signInDay = year + month + day;

        //var _curSignIn = DataCtrlMgr.GetDataByInt(GameConst.DATA_ASSET_SIGNIN, 0);
        var _signInLastDay = GlobalSingleton.Instance.SignInLastDayState;

        if (signInDay > _signInLastDay)
        {
            return true;
        }

        return false;
    }
}
