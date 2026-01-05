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
            return DataManager.GetDataByInt("GuideState", 3);
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

    //private static AssetLoader<Sprite> homeCoinIcon;

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

    //private static GameObjectPool layerGoPool;
    //private static GameObjectPool cabinetGoPool;
    //private static GameObjectPool goodsGoPool;

    //public static ParticlePool trailParticlePool { get; private set; }
    //public static ParticlePool eliminateParticlePool { get; private set; }
    //public static ParticlePool hitBarParticlePool { get; private set; }


    //public static async Task<GameObject> SpawnLayer()
    //{
    //    if (layerGoPool == null)
    //    {
    //        layerGoPool = new GameObjectPool("Prefabs/Game/OneFloor.prefab");
    //        await layerGoPool.AsyncInit();
    //    }
    //    if (!layerGoPool.IsLoaded)
    //        await layerGoPool.AsyncWaitLoaded();


    //    return layerGoPool.Spawn();
    //}

    //public static async Task<GameObject> SpawnCabinet()
    //{
    //    if (cabinetGoPool == null)
    //    {
    //        cabinetGoPool = new GameObjectPool("Prefabs/Game/CabinetUnit.prefab");
    //        await cabinetGoPool.AsyncInit();
    //    }

    //    if (!cabinetGoPool.IsLoaded)
    //        await cabinetGoPool.AsyncWaitLoaded();

    //    return cabinetGoPool.Spawn();

    //}

    //public static async Task<GameObject> SpawnGoods()
    //{
    //    if (goodsGoPool == null)
    //    {
    //        goodsGoPool = new GameObjectPool("Prefabs/Game/StockItem.prefab");
    //        await goodsGoPool.AsyncInit();
    //    }

    //    if (!goodsGoPool.IsLoaded)
    //        await goodsGoPool.AsyncWaitLoaded();

    //    return goodsGoPool.Spawn();

    //}

    public async Task<bool> AsyncInit()
    {
        //layerGoPool = new GameObjectPool("Prefabs/Game/Layer.prefab");
        //cabinetGoPool = new GameObjectPool("Prefabs/Game/Cabinet.prefab");
        //goodsGoPool = new GameObjectPool("Prefabs/Game/Goods.prefab");

        //trailParticlePool = new ParticlePool("Prefabs/Particles/FX_ParticleTrail_bubble.prefab");
        //eliminateParticlePool = new ParticlePool("Prefabs/Particles/Icon_Boom.prefab");
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
        //itemPaths.Add("Sprites/GameContents/tb-21.png");
        if (AppExcuteFlagSettings.ToBFlag)
            itemPaths.Add("Sprites/GameContents/tb-21.png");

        AssetsManager.LoadAsset<Sprite>("Sprites/GameContents/tb-red.png", (resultSp) =>
        {
            spritesDict.Add(resultSp.name, resultSp);
        });


        itemAssetsLoader = new AssetsLoader<Sprite>(itemPaths.ToArray());

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

    //public async Task<GameObject> SpawnLayerGo()
    //{
    //    await Task.CompletedTask;
    //    return layerGoPool.Spawn();
    //}

    //public async Task<GameObject> SpawnCabinetGo()
    //{
    //    await Task.CompletedTask;
    //    return cabinetGoPool.Spawn();
    //}

    //public async Task<GameObject> SpawnGoodsGo()
    //{
    //    await Task.CompletedTask;
    //    return goodsGoPool.Spawn();
    //}

    //public static bool TryGetSuperCoinClaim(out ParticlePool resultPool)
    //{
    //    resultPool = superCoinClaimEffect;
    //    return resultPool != null;

    //}

    //public static GameObject GetBoxExpParticle(int i, Action<GameObject> onSet)
    //{
    //    if (boxExpDict.TryGetValue(i, out var particlePool))
    //    {
    //        return particlePool.Spawn(onSet: onSet);
    //    }
    //    return null;
    //}

    //private static async Task PreloadGameObjects(params string[] paths)
    //{
    //    List<Task> tasks = new List<Task>();
    //    for (int i = 0; i < paths.Length; ++i)
    //    {
    //        var path = paths[i];
    //        if (!_dict.TryGetValue(path, out GameObjectPool resultPool))
    //        {
    //            resultPool = new GameObjectPool(path, false);
    //            resultPool.PoolParent = GlobalPartent;
    //            _dict.Add(path, resultPool);

    //            tasks.Add(resultPool.AsyncInit());

    //        }
    //    }
    //    await Task.WhenAll(tasks);
    //}

    //private static Task PreloadGameObject

    //public static async Task<GameObject> GetGameObject(string path)
    //{
    //    if (_dict.TryGetValue(path, out GameObjectPool resultPool))
    //    {
    //        if (!resultPool.IsInited)
    //        {
    //            await resultPool.AsyncInit();
    //        }
    //        else if (resultPool.IsInited && !resultPool.IsLoaded)
    //        {
    //            await resultPool.WaitLoaded();
    //        }

    //        return resultPool.Spawn();
    //    }
    //    else
    //    {
    //        resultPool = new GameObjectPool(path, false);
    //        resultPool.PoolParent = GlobalPartent;
    //        _dict.Add(path, resultPool);

    //        await resultPool.AsyncInit();
    //        return resultPool.Spawn();
    //    }
    //}

    //public static void BackGameObject(string path, GameObject backGo)
    //{
    //    if (_dict.TryGetValue(path, out GameObjectPool resultPool))
    //    {
    //        resultPool.Back(backGo);
    //    }
    //    else
    //    {
    //        Destroy(backGo);
    //    }
    //}

    //public static async Task<GameObject> SpawnBox(int boxLenght)
    //{
    //    var result = await GetGameObject($"Prefabs/Box/Box_{boxLenght}.prefab");
    //    return result;
    //}
    //public static void DespawnBox(int boxLenght, GameObject backGo)
    //{
    //    BackGameObject($"Prefabs/Box/Box_{boxLenght}.prefab", backGo);
    //}
    //public static async Task<GameObject> SpawnFloor()
    //{
    //    var result = await GetGameObject($"Prefabs/Floor/Floor.prefab");
    //    return result;
    //}
    //public static void DespawnFloor(GameObject backGo)
    //{
    //    BackGameObject($"Prefabs/Floor/Floor.prefab", backGo);
    //}
    //public static async Task<GameObject> SpawnItem(int itemIDX)
    //{
    //    var result = await GetGameObject($"Prefabs/Items/Item_{itemIDX}.prefab");
    //    return result;
    //}
    //public static void DespawnItem(int itemIDX, GameObject backGo)
    //{
    //    BackGameObject($"Prefabs/Items/Item_{itemIDX}.prefab", backGo);
    //}

    //public static async Task<GameObject> SpawnGoods()
    //{
    //    var result = await GetGameObject("Prefabs/Goods/Goods.prefab");
    //    return result;
    //}

    //public static void DespawnGoods(GameObject backGo)
    //{
    //    BackGameObject("Prefabs/Goods/Goods.prefab", backGo);
    //}


}
