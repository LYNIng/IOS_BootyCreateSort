using System;
using UnityEngine;
using UnityEngine.Events;


public static class ShowADManager
{
    public static bool HasLastTime
    {
        get
        {
            return DataManager.HasKey("LastRemoveADs");
        }
    }

    public static DateTime LastDateTime
    {
        get
        {
            var tmp = DataManager.GetDataByString("LastRemoveADs");
            return DateTime.Parse(tmp);
        }
        set
        {
            var tmp = value.ToString();
            DataManager.SetDataByString("LastRemoveADs", tmp);
        }
    }

    public static int ShowADCount
    {
        get
        {
            return DataManager.GetDataByInt("ShowADCount", 0);
        }
        set
        {
            DataManager.SetDataByInt("ShowADCount", value);
        }
    }

    public static bool IsSkipAD
    {
        get
        {
            if (!HasLastTime)
            {
                return false;
            }

            RefreshSkipADTime();

            if (ShowADCount >= 15)
            {
                return true;
            }

            return false;
        }
    }

    private static void RefreshSkipADTime()
    {
        var lastTime = LastDateTime;

        var span = DateTime.UtcNow - lastTime;
        if (span.Hours >= 24)
        {
            ShowADCount = 0;
            LastDateTime = DateTime.UtcNow;
        }

    }

    public static bool PlusPlayADCount()
    {
        ShowADCount++;
        if (!HasLastTime)
        {
            LastDateTime = DateTime.UtcNow;
        }

        if (ShowADCount >= 15)
        {
            MessageDispatch.CallMessageCommand((ushort)GameEvent.RefreshADFREE);
            return true;
        }
        return false;
    }

    // private static DateTime? lastPlayInterstADTime;
    // private const int InterstADSpan = 8;
    public static void PlayInterstAD(string adTag, Action<int, string> callback)
    {
        if (IsSkipAD)
        {
            callback?.Invoke(0, "SkipAD");
            return;
        }

        //MaxManager.Instance.ShowInterstitial(callback);

        UIManager.OpenUI<UIADTest>(new UIADTestParam { mADTag = adTag, mADT = "Interstitial Ad", onResult = callback });
        // #if UNITY_EDITOR
        // #else
        // #endif
        //BidManager.I.ShowInterstitial(adScene, callback);
    }

    public static void PlayVideoAD(string adTag, Action<int, string> callback)
    {
        if (IsSkipAD)
        {
            callback?.Invoke(0, "SkipAD");
            return;
        }
        //MaxManager.Instance.ShowRewardedAd(callback);
        //         if (!AppExcuteFlagSettings.ToAFlag)
        //         {
        //             callback?.Invoke(0, null);
        //             return;
        //         }
        UIManager.OpenUI<UIADTest>(new UIADTestParam { mADTag = adTag, mADT = "Rewarded Ad", onResult = callback });
        // #if UNITY_EDITOR
        // #else
        // #endif
        //BidManager.I.ShowRewardedAd(adScene, callback);
    }
}
