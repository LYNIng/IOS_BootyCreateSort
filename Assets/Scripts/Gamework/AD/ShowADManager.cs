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
            var tmp = LastDateTime.ToString();
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
            if (HasLastTime)
            {
                return false;
            }
            var lastTime = LastDateTime;
            DateTime dateTime = DateTime.UtcNow;

            return false;
        }
    }

    public static bool PlusPlayADCount()
    {
        ShowADCount++;
        if (ShowADCount >= 15)
        {
            return true;
        }
        return false;
    }

    // private static DateTime? lastPlayInterstADTime;
    // private const int InterstADSpan = 8;
    public static void PlayInterstAD(string adTag, Action<int, string> callback)
    {
        //MaxManager.Instance.ShowInterstitial(callback);
        //         if (lastPlayInterstADTime.HasValue)
        //         {
        //             var sec = (DateTime.UtcNow - lastPlayInterstADTime.Value).TotalSeconds;
        //
        //             if (sec < InterstADSpan)
        //             {
        //                 callback?.Invoke(999, $"not ready ");
        //                 return;
        //             }
        //
        //         }
        //         lastPlayInterstADTime = DateTime.UtcNow;
        //
        //         if (!AppExcuteFlagSettings.ToAFlag)
        //         {
        //             callback?.Invoke(0, null);
        //             return;
        //         }
        //
        UIManager.OpenUI<UIADTest>(new UIADTestParam { mADTag = adTag, mADT = "Interstitial Ad", onResult = callback });
        // #if UNITY_EDITOR
        // #else
        // #endif
        //BidManager.I.ShowInterstitial(adScene, callback);
    }

    public static void PlayVideoAD(string adTag, Action<int, string> callback)
    {
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
