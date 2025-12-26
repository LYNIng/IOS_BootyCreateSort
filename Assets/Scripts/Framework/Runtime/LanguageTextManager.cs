using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// PayType
/// </summary>
public enum __ppType
{
    /// <summary>
    /// CashApp
    /// </summary>
    CA_ = 0,
    /// <summary>
    /// PayPal
    /// </summary>
    PP_,
    /// <summary>
    /// PIX
    /// </summary>
    P_,
    /// <summary>
    /// Nubank
    /// </summary>
    NB_,
    /// <summary>
    /// PayPay
    /// </summary>
    PP__,
    /// <summary>
    /// LinePay
    /// </summary>
    LP_,
    /// <summary>
    /// KakaoPay
    /// </summary>
    KP_,
    /// <summary>
    /// SumsungPay
    /// </summary>
    SP_,
    /// <summary>
    /// GoogleWallet
    /// </summary>
    GW_,
    /// <summary>
    /// QIWIWallet
    /// </summary>
    QW_,
    /// <summary>
    /// YandexMoney
    /// </summary>
    YM_,
    //MercadoPago,
    //Ualá,
    //TrueMoney,
    //RabbitLINEPay,
    //MOMO,
    //ZaloPay,
    //Ininal,
    //FastPay,
    //DANA,
    //OVO,
    //GCash,
    //GooglePay,
}


public interface IMultiText
{
    void UpdateText();
}

public class LanguageTextManager : Singleton<LanguageTextManager>, IManager, IManagerInit
{

    public enum E_LanguageType
    {
        CN = 0,
        TC = 1,
        EN = 2,
        JA = 3,
        KO = 4,
        ES = 5,
        PT = 6,
        DE = 7,
        FR = 8,
        RU = 9,
        Max,

        //测试  阿拉伯语 
        AR = 100,
        //测试 
    }
    public static bool Inited { get; private set; }
    private LangCfg m_mlConfig;



    private E_LanguageType m_CurrentLanguage;

    private Dictionary<E_LanguageType, Func<int, string>> m_Dic;

    private List<IMultiText> m_AllTextLanguageList;


    public List<E_LanguageType> AllLanguages { get; private set; }

    //private Dictionary<LanguageEnum, Font> m_fontDict;



    //private readonly Dictionary<E_LanguageType, List<__ppType>> PTDict = new Dictionary<E_LanguageType, List<__ppType>>()
    //    {
    //        {E_LanguageType.EN, new List<__ppType>(){ __ppType.CA_, __ppType.PP_ }},
    //        {E_LanguageType.FR, new List<__ppType>(){ __ppType.PP_, __ppType.GW_ }},
    //        {E_LanguageType.JA, new List<__ppType>(){ __ppType.PP__, __ppType.LP_ }},
    //        {E_LanguageType.KO, new List<__ppType>(){ __ppType.KP_, __ppType.SP_ }},
    //        {E_LanguageType.PT, new List<__ppType>(){ __ppType.P_, __ppType.NB_ }},
    //        {E_LanguageType.RU, new List<__ppType>(){ __ppType.QW_, __ppType.YM_ }},
    //        {E_LanguageType.ES, new List<__ppType>(){ __ppType.PP_, __ppType.GW_ }},
    //        {E_LanguageType.DE, new List<__ppType>(){ __ppType.PP_, __ppType.GW_ }},

    //    };

    //private static Dictionary<string, string> cToPhCode = new Dictionary<string, string>()
    //    {
    //        {"US", "+1"},
    //        {"CA", "+1"},
    //        {"CN", "+86"},
    //        {"JP", "+81"},
    //        {"KR", "+82"},
    //        {"UK", "+44"},
    //        {"FR", "+33"},
    //        {"DE", "+49"},
    //        {"IT", "+39"},
    //        {"RU", "+7"},
    //        {"BR", "+55"},
    //        {"IN", "+91"},
    //    // 添加更多...
    //    };

    //public static string GetCurrentCountry()
    //{
    //    RegionInfo region = new RegionInfo(CultureInfo.CurrentCulture.LCID);
    //    return region.Name; // 返回国家代码，如"US"、"CN"等
    //}

    //public static string GetCurrentCountryToPhoneCode()
    //{
    //    var tmp = GetCurrentCountry();
    //    if (cToPhCode.TryGetValue(tmp, out var result))
    //    {
    //        return result;
    //    }
    //    return "+1";
    //}

    //public List<__ppType> GetPayTypesList()
    //{
    //    if (PTDict.TryGetValue(CurrentLanguage, out var list))
    //    {
    //        return list;
    //    }
    //    else
    //    {
    //        return PTDict[MultiLanguageEnum.EN];
    //    }
    //}

    //public List<__ppType> GetPPTypeList()
    //{
    //    return PTDict[CurrentLanguage];
    //}

    public LanguageTextManager()
    {


    }

    private void SetLanIt(E_LanguageType language, Font font, Func<int, string> getStringCallback)
    {
        if (m_Dic == null)
            m_Dic = new Dictionary<E_LanguageType, Func<int, string>>();

        m_Dic.Add(language, (int id) =>
        {
            if (id > 0)
            {
                return getStringCallback?.Invoke(id);
            }
            else
                return $"no id {id}";
        });
    }

    //    public void InitLoad()
    //    {
    //        if (Inited) return;
    //        Inited = true;

    //#if UNITY_EDITOR
    //        if (LaunchScene.S_SimLanguge)
    //        {
    //            CurrentLanguage = LaunchScene.S_LanguageType;
    //            return;
    //        }

    //#endif

    //        var lang = PlayerPrefs.GetInt("LastLang", -1);
    //        bool RecordLastLanguageStatus = false;
    //        //SetCurrentLanguage(lastInt);
    //        if (lang == -1)
    //        {
    //            MultiLanguageEnum tmp = MultiLanguageEnum.EN;
    //            switch (Application.systemLanguage)
    //            {
    //                case SystemLanguage.English:
    //                    tmp = MultiLanguageEnum.EN;
    //                    break;
    //                case SystemLanguage.Japanese:
    //                    tmp = MultiLanguageEnum.JA;
    //                    break;
    //                case SystemLanguage.Korean:
    //                    tmp = MultiLanguageEnum.KO;
    //                    break;
    //                case SystemLanguage.Estonian:
    //                    tmp = MultiLanguageEnum.ES;
    //                    break;
    //                case SystemLanguage.Portuguese:
    //                    tmp = MultiLanguageEnum.PT;
    //                    break;
    //                case SystemLanguage.German:
    //                    tmp = MultiLanguageEnum.DE;
    //                    break;
    //                case SystemLanguage.French:
    //                    tmp = MultiLanguageEnum.FR;
    //                    break;
    //                case SystemLanguage.Russian:
    //                    tmp = MultiLanguageEnum.RU;
    //                    break;
    //                default:
    //                    tmp = MultiLanguageEnum.EN;
    //                    break;
    //            }
    //            m_CurrentLanguage = tmp;
    //        }
    //        else
    //        {
    //            Instance.m_CurrentLanguage = (MultiLanguageEnum)lang;
    //            if (RecordLastLanguageStatus)
    //                PlayerPrefs.SetInt("LastLang", (int)lang);
    //        }

    //    }

    //public bool TryGetPaySprite_T1(ppType payType, out Sprite result)
    //{
    //    result = null;
    //    if (AssetMgr.TryGetPayImageConfig(AssetMgr.PayImageConfig_T1, out var tmp))
    //    {
    //        return tmp.TryGetImage(payType, out result);
    //    }
    //    return false;
    //}

    //public bool TryGetPaySprite_T2(ppType payType, out Sprite result)
    //{
    //    result = null;
    //    if (AssetMgr.TryGetPayImageConfig(AssetMgr.PayImageConfig_T2, out var tmp))
    //    {
    //        return tmp.TryGetImage(payType, out result);
    //    }
    //    return false;
    //}

    public static E_LanguageType CurrentLanguage
    {
        get
        {

            return Instance.m_CurrentLanguage;
        }
        set
        {
            int language = (int)value;
            if (language == -1)
            {
                switch (Application.systemLanguage)
                {
                    case SystemLanguage.English:
                        language = (int)E_LanguageType.EN;
                        break;
                    case SystemLanguage.Japanese:
                        language = (int)E_LanguageType.JA;
                        break;
                    case SystemLanguage.Korean:
                        language = (int)E_LanguageType.KO;
                        break;
                    case SystemLanguage.Estonian:
                        language = (int)E_LanguageType.ES;
                        break;
                    case SystemLanguage.Portuguese:
                        language = (int)E_LanguageType.PT;
                        break;
                    case SystemLanguage.German:
                        language = (int)E_LanguageType.DE;
                        break;
                    case SystemLanguage.French:
                        language = (int)E_LanguageType.FR;
                        break;
                    case SystemLanguage.Russian:
                        language = (int)E_LanguageType.RU;
                        break;
                    default:
                        language = (int)E_LanguageType.EN;
                        break;
                }
            }
            Instance.m_CurrentLanguage = (E_LanguageType)language;
            int count = Instance.m_AllTextLanguageList.Count;
            for (int i = 0; i < count; i++)
            {
                Instance.m_AllTextLanguageList[i].UpdateText();
            }
            PlayerPrefs.SetInt("LastLang", (int)Instance.m_CurrentLanguage);

            Debug.Log($"设置 {Instance.m_CurrentLanguage}");
        }
    }

    /// <summary>
    /// GetLanguageText
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isReplaceUn"></param>
    /// <returns></returns>
    public static string GetLangStr(int id, bool isReplaceUn = true)
    {
        if (!Inited)
        {
            return id.ToString();
        }
        if (isReplaceUn)
        {
            return Instance.m_Dic[CurrentLanguage](id).Replace("\\n", "\n");
        }
        else
        {
            return Instance.m_Dic[CurrentLanguage](id);
        }

    }
    /// <summary>
    /// GetGoldSymbol
    /// </summary>
    /// <returns></returns>
    //public static string GetInfo_Gold_Symbol()
    //{
    //    return GetLangStr(1000);
    //}

    /// <summary>
    /// RegisterText
    /// </summary>
    /// <param name="textLanguage"></param>
    internal static void RegText(IMultiText textLanguage)
    {
        if (!Instance.m_AllTextLanguageList.Contains(textLanguage))
            Instance.m_AllTextLanguageList.Add(textLanguage);
    }
    /// <summary>
    /// RemoveText
    /// </summary>
    /// <param name="textLanguage"></param>
    internal static void RemText(IMultiText textLanguage)
    {
        if (Instance.m_AllTextLanguageList.Contains(textLanguage))
            Instance.m_AllTextLanguageList.Remove(textLanguage);
    }

    public async Task<bool> AsyncInit()
    {
        if (Inited) return true;


        m_mlConfig = new LangCfg();
        await m_mlConfig.AsyncInitLangCfg();
        Inited = true;
        m_AllTextLanguageList = new List<IMultiText>();
        m_CurrentLanguage = E_LanguageType.EN;

        SetLanIt(E_LanguageType.EN, null, m_mlConfig.GetENByID);
        SetLanIt(E_LanguageType.JA, null, m_mlConfig.GetJAByID);
        SetLanIt(E_LanguageType.KO, null, m_mlConfig.GetKRByID);

        SetLanIt(E_LanguageType.ES, null, m_mlConfig.GetESByID);
        SetLanIt(E_LanguageType.PT, null, m_mlConfig.GetPTByID);
        SetLanIt(E_LanguageType.DE, null, m_mlConfig.GetDEByID);
        SetLanIt(E_LanguageType.FR, null, m_mlConfig.GetFRByID);
        SetLanIt(E_LanguageType.RU, null, m_mlConfig.GetRUByID);

        AllLanguages = new List<E_LanguageType>(m_Dic.Keys);

#if UNITY_EDITOR
        //if (LaunchPage.S_SimLanguge)
        //{
        //    CurrentLanguage = LaunchPage.S_LanguageType;
        //    return true;
        //}

#endif
        var lang = PlayerPrefs.GetInt("LastLang", -1);
        bool RecordLastLanguageStatus = false;

        if (lang == -1)
        {
            E_LanguageType tmp = E_LanguageType.EN;
            switch (Application.systemLanguage)
            {
                case SystemLanguage.English:
                    tmp = E_LanguageType.EN;
                    break;
                case SystemLanguage.Japanese:
                    tmp = E_LanguageType.JA;
                    break;
                case SystemLanguage.Korean:
                    tmp = E_LanguageType.KO;
                    break;
                case SystemLanguage.Estonian:
                    tmp = E_LanguageType.ES;
                    break;
                case SystemLanguage.Portuguese:
                    tmp = E_LanguageType.PT;
                    break;
                case SystemLanguage.German:
                    tmp = E_LanguageType.DE;
                    break;
                case SystemLanguage.French:
                    tmp = E_LanguageType.FR;
                    break;
                case SystemLanguage.Russian:
                    tmp = E_LanguageType.RU;
                    break;
                default:
                    tmp = E_LanguageType.EN;
                    break;
            }
            m_CurrentLanguage = tmp;
        }
        else
        {
            Instance.m_CurrentLanguage = (E_LanguageType)lang;
            if (RecordLastLanguageStatus)
                PlayerPrefs.SetInt("LastLang", (int)lang);
        }


        return true;
    }
}

public static class StringMultiLanguageUtil
{
    public static float ToPriceValue(this int price)
    {
        float result = price / 100f;
        switch (LanguageTextManager.CurrentLanguage)
        {
            case LanguageTextManager.E_LanguageType.EN: //美国
                return (result * 1);
            case LanguageTextManager.E_LanguageType.FR: //法国
                return (result * 1);
            case LanguageTextManager.E_LanguageType.JA: //日本
                return (result * 140);
            case LanguageTextManager.E_LanguageType.KO: //韩国
                return (result * 1350);
            case LanguageTextManager.E_LanguageType.PT: //巴西
                return (result * 5);
            case LanguageTextManager.E_LanguageType.RU: //俄罗斯
                return (result * 60);
            case LanguageTextManager.E_LanguageType.ES: //西班牙
                return (result * 1);
            case LanguageTextManager.E_LanguageType.DE: //德国
                return (result * 1);
            default: //默认英语
                return (result * 1);
        }
    }

    public static string ToPriceStrF0(this int price)
    {
        if (!LanguageTextManager.Inited)
        {
            float t = price / 100f;
            return $"${(t * 1).ToString("f0")}";
        }

        var symbol = LanguageTextManager.GetLangStr(1000);
        string str = "";
        float result = price / 100f;
        switch (LanguageTextManager.CurrentLanguage)
        {
            case LanguageTextManager.E_LanguageType.EN: //美国
                str = $"{symbol}{(result * 1).ToString("f0")}";
                break;
            case LanguageTextManager.E_LanguageType.FR: //法国
                str = $"{(result * 1).ToString("f0")}{symbol}";
                break;
            case LanguageTextManager.E_LanguageType.JA: //日本
                str = $"{symbol}{(result * 140).ToString("f0")}";
                break;
            case LanguageTextManager.E_LanguageType.KO: //韩国
                str = $"{symbol}{(result * 1350).ToString("f0")}";
                break;
            case LanguageTextManager.E_LanguageType.PT: //巴西
                str = $"{symbol}{(result * 5).ToString("f0")}";
                break;
            case LanguageTextManager.E_LanguageType.RU: //俄罗斯
                str = $"{symbol}{(result * 60).ToString("f0")}";
                break;
            case LanguageTextManager.E_LanguageType.ES: //西班牙
                str = $"{(result * 1).ToString("f0")}{symbol}";
                break;
            case LanguageTextManager.E_LanguageType.DE: //德国
                str = $"{(result * 1).ToString("f0")}{symbol}";
                break;
            default: //默认英语
                str = $"{symbol}{(result * 1).ToString("f0")}";
                break;
        }

        return str;
    }

    public static string ToPriceStr(this int price)
    {
        if (!LanguageTextManager.Inited)
        {
            float t = price / 100f;
            return $"${(t * 1).ToString("f2")}";
        }

        var symbol = LanguageTextManager.GetLangStr(1000);
        string str = "";
        float result = price / 100f;
        switch (LanguageTextManager.CurrentLanguage)
        {
            case LanguageTextManager.E_LanguageType.EN: //美国
                str = $"{symbol}{(result * 1).ToString("f2")}";
                break;
            case LanguageTextManager.E_LanguageType.FR: //法国
                str = $"{(result * 1).ToString("f2")}{symbol}";
                break;
            case LanguageTextManager.E_LanguageType.JA: //日本
                str = $"{symbol}{(result * 140).ToString("f0")}";
                break;
            case LanguageTextManager.E_LanguageType.KO: //韩国
                str = $"{symbol}{(result * 1350).ToString("f0")}";
                break;
            case LanguageTextManager.E_LanguageType.PT: //巴西
                str = $"{symbol}{(result * 5).ToString("f2")}";
                break;
            case LanguageTextManager.E_LanguageType.RU: //俄罗斯
                str = $"{symbol}{(result * 60).ToString("f2")}";
                break;
            case LanguageTextManager.E_LanguageType.ES: //西班牙
                str = $"{(result * 1).ToString("f2")}{symbol}";
                break;
            case LanguageTextManager.E_LanguageType.DE: //德国
                str = $"{(result * 1).ToString("f2")}{symbol}";
                break;
            default: //默认英语
                str = $"{symbol}{(result * 1).ToString("f2")}";
                break;
        }

        return str;
    }

    /// <summary>
    /// int 转多语言字符串
    /// </summary>
    /// <param name="multiLangID"></param>
    /// <returns></returns>
    public static string ToMultiLanguageText(this int multiLangID)
    {
        return LanguageTextManager.GetLangStr(multiLangID);
    }

    public static string ToMulStrFormat(this int multiLangID, params object[] format)
    {
        return string.Format(multiLangID.ToMultiLanguageText(), format);
    }

    public static string ToNumText(this int num, GameAssetType assetType)
    {
        if (assetType == GameAssetType.SuperCoin)
        {
            return num.ToPriceStr();
        }
        else
        {
            return $"x{num.FormatNumber()}";
        }
    }
}

