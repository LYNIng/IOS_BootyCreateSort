using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum GameAssetType : byte
{
    Coin,
    SuperCoin,

    RemoveTool,
    BackTool,
    RefreshTool,

}


public partial class GlobalSingleton : Singleton<GlobalSingleton>
{
    public static int PlayTimeBySeconds
    {
        get
        {
            return DataManager.GetDataByInt("PlayTimeBySeconds", 0);
        }
        set
        {
            DataManager.SetDataByInt("PlayTimeBySeconds", value);
        }
    }
    public static bool GameRunning { get; set; }
    public static int CurCount { get; set; }
    public static int MaxCount { get; set; }
    public static int Level
    {
        get
        {
            return DataManager.GetDataByInt("Level", 0);
        }
        set
        {
            DataManager.SetDataByInt("Level", value);
        }

    }

    public static string LevelString
    {
        get
        {
            return (Level + 1).ToString();
        }
    }

    public static int RemoveTool
    {
        get
        {
            return DataManager.GetDataByInt("RemoveTool", 0);
        }
        set
        {
            DataManager.SetDataByInt("RemoveTool", value);
            RedPointManager.Instance.RefreshRedPoint(RedPointManager.IDS.RP_Remove);
        }
    }

    public static int BackTool
    {
        get
        {
            return DataManager.GetDataByInt("BackTool");
        }
        set
        {
            DataManager.SetDataByInt("BackTool", value);
            RedPointManager.Instance.RefreshRedPoint(RedPointManager.IDS.RP_Back);
        }
    }

    public static int RefreshTool
    {
        get => DataManager.GetDataByInt("RefreshTool");
        set
        {
            DataManager.SetDataByInt("RefreshTool", value);
            RedPointManager.Instance.RefreshRedPoint(RedPointManager.IDS.RP_Refresh);
        }
    }

    #region Coin


    public static int Coin
    {
        get
        {
            return DataManager.GetDataByInt("Coin", 0);
        }
        private set
        {
            DataManager.SetDataByInt("Coin", value);

        }
    }


    #endregion

    public static int SuperCoin
    {
        get
        {
            return DataManager.GetDataByInt("SuperCoin", 0);
        }
        private set
        {
            DataManager.SetDataByInt("SuperCoin", value);
        }
    }

    /// <summary>
    /// 消除次数
    /// </summary>
    public static int EliminateCnt
    {
        get
        {
            return DataManager.GetDataByInt("EliminateCnt", 0);
        }
        set
        {
            DataManager.SetDataByInt("EliminateCnt", value);

            RedPointManager.Instance.RefreshRedPoint(RedPointManager.IDS.RP_Eli);
            RedPointManager.Instance.RefreshRedPoint(RedPointManager.IDS.RP_Task);
        }
    }

    /// <summary>
    /// 获取SC次数
    /// </summary>
    public static int GetSuperCoinCount
    {
        get
        {
            return DataManager.GetDataByInt("GetSuperCount", 0);
        }
        set
        {
            DataManager.SetDataByInt("GetSuperCount", value);

        }
    }

    public static int GuideState
    {
        get
        {
            return DataManager.GetDataByInt("GuideState", 0);
        }
        set
        {
            DataManager.SetDataByInt("GuideState", value);
        }
    }

    protected override void OnAwake()
    {
        base.OnAwake();

    }
    #region 关卡配置

    public const int SuperCoinType = 14;

    /// <summary>
    /// 镂空概率
    /// basic 基础概率
    /// plus 叠加概率
    /// </summary>
    /// <param name="floorIdx"></param>
    /// <returns></returns>
    public static (float basic, float plus) GetHollowOutByFloor(int floorIdx)
    {
        return floorIdx switch
        {
            1 => (10f, 30f),
            2 => (10f, 40f),
            3 => (10f, 50f),
            4 => (10f, 30f),
            5 => (10f, 20f),
            _ => (0.0f, 0f)
        };
    }

    /// <summary>
    /// 这个值会乘输入的min 和max 达到改变这两个值的效果
    /// </summary>
    /// <param name="floorIdx"></param>
    /// <returns></returns>
    public static (float min, float max) GetSparsityByFloor(int floorIdx)
    {
        return floorIdx switch
        {
            1 => (0.7f, 0.8f),
            2 => (0.7f, 0.8f),
            3 => (0.7f, 0.8f),
            4 => (0.7f, 0.8f),
            5 => (0.7f, 1f),
            6 => (0.9f, 1f),
            _ => (0.9f, 1f)
        };
    }

    /// <summary>
    /// 多少层分割一次稀疏度
    /// </summary>
    /// <returns></returns>
    public static int GetSparsityByFloorCnt()
    {
        return 7;
    }

    /// <summary>
    /// 获取每关最大层数配置
    /// </summary>
    /// <returns></returns>
    public static int GetLayerCountMax()
    {
        return Level switch
        {
            0 => 10,
            1 => 24,//24
            2 => 38,//
            3 => 55,
            _ => 20
        };
    }
    public static int GetCanbinetMaxLength()
    {
        return Level switch
        {
            0 => 3,
            1 => 4,
            2 => 4,
            3 => 4,
            _ => 5
        };
    }

    public static int GetCanbinetMinLength()
    {
        return Level switch
        {
            0 => 1,
            1 => 2,
            2 => 3,
            3 => 3,
            _ => 3
        };
    }

    public static int GetGoodsTypeCount()
    {
        return Level switch
        {
            0 => 5,
            1 => 7,
            2 => 9,
            3 => 11,
            _ => 21
        };
    }

    /// <summary>
    /// 钞票概率
    /// </summary>
    /// <returns></returns>
    public static int GetGoldRate()
    {
        // 钞票

        return 25;
    }

    /// <summary>
    /// 无限关钞票概率
    /// </summary>
    /// <returns></returns>
    public static int GetInfiniteRate()
    {
        return 20;
    }

    public static int GetOverlayRate()
    {
        return 10;
    }
    public static int GetOverlayInterval()
    {
        return 10;
    }

    public static int CaItemType()
    {
        return 21;
    }
    /// <summary>
    /// item组的数量
    /// </summary>
    /// <returns></returns>
    public static (int min, int max) ItemGroupCountRange()
    {
        return (3, 6);// 3-5个
    }

    #endregion

    #region GameSetting

    public bool VibrateIsEnable
    {
        get
        {
            return DataManager.GetDataByBool("VibrateIsEnable", true);
        }
        set
        {
            DataManager.SetDataByBool("VibrateIsEnable", value);
        }
    }



    #endregion

    #region SuperCoin

    /// <summary>
    /// 消除钞票次数
    /// </summary>
    public static int EliminateSuperCoinCnt
    {
        get
        {
            return DataManager.GetDataByInt("EliminateSuperCoinCnt", 0);
        }
        set
        {
            var cnt = EliminateSuperCoinCnt;
            DataManager.SetDataByInt("EliminateSuperCoinCnt", value);
            if (InsertAdSpan > 0)
            {
                PlayedADByGetSuperCoinCnt -= value - cnt;
            }

            Debug.Log($"EliminateSuperCoinCnt -> {EliminateSuperCoinCnt} | PlayedADByGetSuperCoinCnt -> {PlayedADByGetSuperCoinCnt}");
        }
    }

    #endregion

    #region Reward

    public static void GetReward(GameAssetType gameAsset, int value, Vector3? startPos = null)
    {
        Debug.Log($"onGetReward {gameAsset} - {value}");
        if (gameAsset == GameAssetType.Coin)
        {
            Coin += value;


            MessageDispatch.CallMessageCommand((ushort)GameEvent.RefreshCoin);

        }
        else if (gameAsset == GameAssetType.SuperCoin)
        {
            SuperCoin += value;
            GetSuperCoinCount++;
            MessageDispatch.CallMessageCommand((ushort)GameEvent.RefreshSuperCoin);

        }
        else if (gameAsset == GameAssetType.RemoveTool)
        {
            RemoveTool += value;
        }
        else if (gameAsset == GameAssetType.BackTool)
        {
            BackTool += value;
        }
        else if (gameAsset == GameAssetType.RefreshTool)
        {
            RefreshTool += value;
        }
    }

    public static async Task AsyncGetReward(GameAssetType gameAsset, int value, Vector3? startPos = null, int? flyCnt = null)
    {
        if (gameAsset == GameAssetType.Coin)
        {
            Coin += value;
            if (startPos.HasValue)
            {
                AudioManager.AudioPlayer.PlayOneShot(SoundName.RewardPop);
                var resultSprite = await AssetsManager.AsyncLoadAsset<Sprite>("Sprites/MainGamePlay/tb-coin.png");
                await ExplodeFlyToEffect.PlayEffectByWorldPos(resultSprite,
                     flyCnt.HasValue ? flyCnt.Value : RandomHelp.RandomRange(3, 5),
                        startPos.Value, TopCoinPart.Instance.imaIcon.transform.position,
                        UIManager.GetCanvasLayerTransform(UICanvasLayer.Overlay_Camera), () =>
                        {
                            AudioManager.AudioPlayer.PlayOneShot(SoundName.CA);
                            MessageDispatch.CallMessageCommand((ushort)GameEvent.RefreshCoin);
                        }
                     );

                MessageDispatch.CallMessageCommand((ushort)GameEvent.RefreshCoin);
            }
            else
            {
                await MessageDispatch.AsyncCallMessageCommand((ushort)GameEvent.RefreshCoin);
            }
        }
        else if (gameAsset == GameAssetType.SuperCoin)
        {
            SuperCoin += value;
            GetSuperCoinCount++;
            if (startPos.HasValue)
            {
                AudioManager.AudioPlayer.PlayOneShot(SoundName.CA);
                var resultSprite = await AssetsManager.AsyncLoadAsset<Sprite>("Sprites/MainGamePlay/tb-mj.png");
                await ExplodeFlyToEffect.PlayEffectByWorldPos(resultSprite,
                    flyCnt.HasValue ? flyCnt.Value : RandomHelp.RandomRange(3, 5),
                    startPos.Value, SuperCoinElement.Instance.imaIcon.transform.position,
                    UIManager.GetCanvasLayerTransform(UICanvasLayer.Overlay_Camera), () =>
                    {
                        AudioManager.AudioPlayer.PlayOneShot(SoundName.CA);
                        MessageDispatch.CallMessageCommand((ushort)GameEvent.RefreshSuperCoin);

                    });
            }
            else
            {
                await MessageDispatch.AsyncCallMessageCommand((ushort)GameEvent.RefreshSuperCoin);
            }
        }
        //else if (gameAsset == GameAssetType.Cards)
        //{
        //    Cards += value;
        //    await MessageDispatch.AsyncCallMessageCommand((ushort)GameEvent.RefreshCards);
        //}
    }

    #endregion

    #region Items
    public static async Task CostAsset(GameAssetType assetType, int value)
    {
        if (assetType == GameAssetType.Coin)
        {
            Coin -= value;
            await MessageDispatch.AsyncCallMessageCommand((ushort)GameEvent.RefreshCoin);

        }
        else if (assetType == GameAssetType.SuperCoin)
        {
            SuperCoin -= value;
            await MessageDispatch.AsyncCallMessageCommand((ushort)GameEvent.RefreshSuperCoin);

        }
        //else if (assetType == GameAssetType.Cards)
        //{
        //    GameGlobal.Instance.Cards -= value;
        //    await MessageDispatch.AsyncCallMessageCommand((ushort)GameEvent.RefreshCards);
        //}
    }

    #endregion

    #region Tools

    #endregion

    #region Channel
    //public int CAChannel
    //{
    //    get
    //    {
    //        return DataManager.GetDataByInt("CAChannel");
    //    }
    //    set
    //    {
    //        DataManager.SetDataByInt("CAChannel", value);
    //    }
    //}

    #endregion
}

public partial class GlobalAssetSingleton : MonoSingleton<GlobalAssetSingleton>, IManager, IManagerInit
{
    public static Transform GlobalPartent => Instance.transform;

    public override bool DontDestory => true;

    private static Dictionary<string, Sprite> spritesDict;
    private AssetsLoader<Sprite> itemAssetsLoader;

    public static ParticlePool eliEffectPool;
    public static ParticlePool UIFX_ParticleTrail_Feather;
    public static ParticlePool UIPoof;
    public static ParticlePool UIHitBar;
    public static ParticlePool UIBox_Boom;

    public static Material SuperCoinMate { get; private set; }

    public async Task<bool> AsyncInit()
    {
        eliEffectPool = new ParticlePool("Prefabs/Particles/UISmooch.prefab");
        UIFX_ParticleTrail_Feather = new ParticlePool("Prefabs/Particles/UIFX_ParticleTrail_Feather.prefab");
        UIPoof = new ParticlePool("Prefabs/Particles/UIPoof.prefab");
        UIHitBar = new ParticlePool("Prefabs/Particles/UIFeatherExplosion.prefab");

        UIBox_Boom = new ParticlePool("Prefabs/Particles/UIBox_Boom.prefab");


        //hitBarParticlePool = new ParticlePool("Prefabs/Particles/UIP_StunExplosion.prefab");

        spritesDict = new Dictionary<string, Sprite>();
        List<string> itemPaths = new List<string>();
        itemPaths.Add("Sprites/GameContents/tb-1.png");
        itemPaths.Add("Sprites/GameContents/tb-2.png");
        itemPaths.Add("Sprites/GameContents/tb-3.png");
        itemPaths.Add("Sprites/GameContents/tb-4.png");
        itemPaths.Add("Sprites/GameContents/tb-5.png");
        itemPaths.Add("Sprites/GameContents/tb-6.png");
        itemPaths.Add("Sprites/GameContents/tb-7.png");
        itemPaths.Add("Sprites/GameContents/tb-8.png");
        itemPaths.Add("Sprites/GameContents/tb-9.png");
        itemPaths.Add("Sprites/GameContents/tb-10.png");
        itemPaths.Add("Sprites/GameContents/tb-11.png");
        itemPaths.Add("Sprites/GameContents/tb-12.png");
        itemPaths.Add("Sprites/GameContents/tb-13.png");
        itemPaths.Add("Sprites/GameContents/tb-14.png");
        itemPaths.Add("Sprites/GameContents/tb-15.png");
        itemPaths.Add("Sprites/GameContents/tb-16.png");
        itemPaths.Add("Sprites/GameContents/tb-17.png");
        itemPaths.Add("Sprites/GameContents/tb-18.png");
        itemPaths.Add("Sprites/GameContents/tb-19.png");
        itemPaths.Add("Sprites/GameContents/tb-20.png");
        if (AppExcuteFlagSettings.ToBFlag)
            itemPaths.Add("Sprites/GameContents/tb-21.png");

        AssetsManager.LoadAsset<Sprite>("Sprites/GameContents/tb-red.png", (resultSp) =>
        {
            spritesDict.Add(resultSp.name, resultSp);
        });


        itemAssetsLoader = new AssetsLoader<Sprite>(itemPaths.ToArray());

        AssetsManager.LoadAsset<Material>("Materials/UIItem.mat", (resultMate) =>
        {
            SuperCoinMate = resultMate;
        });

        await itemAssetsLoader.AsyncLoad();

        foreach (var item in itemAssetsLoader.Handle.Result)
        {
            spritesDict.Add(item.name, item);
        }

        return true;
    }

    public bool TryGetSprite(string spriteAssetName, out Sprite result)
    {
        if (spritesDict.TryGetValue(spriteAssetName, out result))
        {
            return true;
        }
        return false;
    }


    public bool TryGetSprite(int index, out Sprite result)
    {
        if (spritesDict.TryGetValue($"tb-{index}", out result))
        {
            return true;
        }
        return false;
    }


}
