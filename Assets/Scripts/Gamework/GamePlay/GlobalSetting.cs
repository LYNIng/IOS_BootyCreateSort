
using System.Collections.Generic;
using System;

public partial class GlobalSingleton
{
    public enum SuperCoinPopState
    {
        None,
        Pop,
        PopDouble
    }

    private readonly static Dictionary<int, Func<(SuperCoinPopState canDouble, int value, bool playInsertAD)>> GetSuperCoinFunc = new Dictionary<int, Func<(SuperCoinPopState canDouble, int value, bool playInsertAD)>>
    {
        {0, GetSuperCoinValue_Scheme1},
        {1, GetSuperCoinValue_Scheme2}
    };

    public static int InsertAdSpan
    {
        get => DataManager.GetDataByInt("DoubleAdSpan", 0);
        set => DataManager.SetDataByInt("DoubleAdSpan", value);
    }

    /// <summary>
    /// 设定Double间隔 
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public static int GetDoubleADSpan()
    {
        return AppExcuteFlagSettings.CurSTag switch
        {
            0 => 5,//间隔4次给一次Double
            1 => 5,
            _ => 5
        };
    }

    public static bool PlayedAD
    {
        get
        {
            return DataManager.GetDataByBool("PlayedAD", false);
        }
        set
        {
            DataManager.SetDataByBool("PlayedAD", value);
        }
    }

    public static int PlayedADByGetSuperCoinCnt
    {
        get
        {
            return DataManager.GetDataByInt("PlayedADByGetSuperCoinCnt", 0);
        }
        set
        {
            DataManager.SetDataByInt("PlayedADByGetSuperCoinCnt", value);
        }
    }

    public static (SuperCoinPopState popState, int value, bool playInsertAD) PeekSuperCoinValue()
    {
        if (GetSuperCoinFunc.TryGetValue(AppExcuteFlagSettings.CurSTag, out var func))
        {
            return func.Invoke();
        }
        else
        {
            return GetSuperCoinValue_Scheme1();
        }
    }
    /// <summary>
    /// 第一套方案
    /// </summary>
    /// <returns></returns>
    private static (SuperCoinPopState, int, bool) GetSuperCoinValue_Scheme1()
    {
        int resultValue = 0;
        SuperCoinPopState popState = SuperCoinPopState.None;
        bool playInsertAD = false;
        switch (EliminateSuperCoinCnt)
        {
            case 1: //首次弹窗赠送30$
                popState = SuperCoinPopState.Pop;
                resultValue = 3000;
                break;
            case 2://普通钞票直接加奖励不弹窗
                popState = SuperCoinPopState.None;
                resultValue = RandomHelp.RandomRange(100, 201);
                break;
            case 3://第三次为double领取界面 双倍弹窗10-15$翻倍
                popState = SuperCoinPopState.PopDouble;
                resultValue = RandomHelp.RandomRange(1000, 1501);
                break;
            default:

                var temp = EliminateSuperCoinCnt - 3;
                var cnt = temp % GetDoubleADSpan();

                if (cnt == 0)
                {
                    popState = SuperCoinPopState.PopDouble;
                    resultValue = RandomHelp.RandomRange(1000, 1501);
                }
                else
                {
                    popState = SuperCoinPopState.None;
                    resultValue = RandomHelp.RandomRange(100, 201);
                }

                playInsertAD = GlobalSingleton.InsertAdSpan > 0 && PlayedADByGetSuperCoinCnt <= 0;



                break;
        }
        return (popState, resultValue, playInsertAD);
    }

    /// <summary>
    /// 第二套方案
    /// </summary>
    /// <returns></returns>
    private static (SuperCoinPopState, int, bool) GetSuperCoinValue_Scheme2()
    {
        int resultValue = 0;
        SuperCoinPopState popState = SuperCoinPopState.None;
        bool playInsertAD = false;
        var eliminateSuperCoinCnt = EliminateSuperCoinCnt;

        switch (eliminateSuperCoinCnt)
        {
            case 1://首次免费金额60-70$（弹窗）
                popState = SuperCoinPopState.Pop;
                resultValue = RandomHelp.RandomRange(6000, 7001);
                break;
            case 2://第二次免费20-30$
                popState = SuperCoinPopState.None;
                resultValue = RandomHelp.RandomRange(2000, 3001);
                break;
            case 3://（第三次为double领取界面）
                popState = SuperCoinPopState.PopDouble;
                resultValue = RandomHelp.RandomRange(2000, 3001);
                break;
            default:
                var tmpValue = eliminateSuperCoinCnt - 3;

                int doubleCnt = tmpValue % GetDoubleADSpan();//间隔4次一次Double
                int valueCnt = (eliminateSuperCoinCnt - 3) % 6;//间隔6次一次大额
                if (doubleCnt == 0)
                {
                    popState = SuperCoinPopState.PopDouble;
                    resultValue = RandomHelp.RandomRange(800, 2001);
                }
                else if (valueCnt == 0)
                {
                    //大额
                    resultValue = RandomHelp.RandomRange(1000, 2501);
                }
                else
                {
                    //小额
                    resultValue = RandomHelp.RandomRange(500, 1501);
                }


                playInsertAD = GlobalSingleton.InsertAdSpan > 0 && PlayedADByGetSuperCoinCnt <= 0;


                break;
        }

        return (popState, resultValue, playInsertAD);
    }

}