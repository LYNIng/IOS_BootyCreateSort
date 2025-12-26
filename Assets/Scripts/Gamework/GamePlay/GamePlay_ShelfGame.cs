using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class GamePlay_ShelfGame : MonoSingleton<GamePlay_ShelfGame>, IMsgObj
{
    public override bool DontDestory => false;
    [SerializeField]
    private GamePlay_Storage storage;

    [SerializeField]
    private GamePlay_Slots slots;

    public static int Level;

    //public List<GamePlay_Goods> guideItems = null;

    public Vector3 BackItemPos { get; set; }

    //private int _curLevelItemAllCount;

    public bool GoodsCanClickSwitch = false;

    //public int waitForSettleTimes;

    //ParticlePool SoulExplosionOrange;
    //ParticlePool particleTrail;
    //ParticlePool liquidExplosionAcidPool;
    //ParticlePool boxParticle;

    private FlyItemLogic flyLogic;

    //private void Awake()
    //{
    //    //Inst = this;
    //    //    SoulExplosionOrange = new ParticlePool("Prefabs/Particles/SoulExplosionOrange.prefab");
    //    //    particleTrail = new ParticlePool("Prefabs/Particles/FX_ParticleTrail_bubble.prefab");
    //    //    liquidExplosionAcidPool = new ParticlePool("Prefabs/Particles/UIStunExplosion.prefab");
    //    //    boxParticle = new ParticlePool("Prefabs/Particles/UIBubbleBlastUnderwater.prefab");
    //}

    public async Task InitData(int lv)
    {
        GoodsCanClickSwitch = false;
        Level = lv;
        slots.InitData();
        //propsLogic.ResetProp();
        ActionLogManager.CallInit();
        storage.CleanFloor();
        storage.InitData();
        GlobalSingleton.CurCount = 0;
        GlobalSingleton.MaxCount = storage.GetVol();
        UIManager.OpenUI<User_Topbar>();

        await Task.Delay(500);
        //await Task.Delay(2000);
        slots.ShowSeats();
        GoodsCanClickSwitch = true;

        GlobalSingleton.GameRunning = true;



        if (AppExcuteFlagSettings.ToBFlag && AppExcuteFlagSettings.ToAFlag)
        {
            flyLogic = new FlyItemLogic("Prefabs/UIComp/FlyItem.prefab");
        }
        isOver = false;
    }



    public bool isOver = false;
    // 游戏结算
    public async void CheckComplete()
    {
        if (isOver) return;


        if (slots.IsFail())
        {
            isOver = true;
            if (flyLogic != null)
            {
                flyLogic.Destroy();
                flyLogic = null;
            }
            var reUI = await UIManager.OpenUIAsync<User_Resurrection>();
            var result = await reUI.WaitClose<bool>();

            if (result)
            {
                isOver = false;
                OnUseBackProp(true);
                return;
            }

            // 失败
            slots.CallStopAllAnim();
            Debug.Log("Call Fail");
            this.SendCommand((ushort)GameEvent.GameFail);
            //if (AppExcuteFlagSettings.ToBFlag)
            //{

            //    if (!UIManager.IsUIOpen<User_FailPop>())
            //    {
            //        UIManager.OpenUIByQueue<User_FailPop>();
            //    }
            //}
            //else
            //{

            //    UIManager.OpenUIByQueue<User_FailPop>();
            //}
        }
        else if (storage.IsEmpty() && slots.IsEmpty())
        {
            isOver = true;
            if (flyLogic != null)
            {
                flyLogic.Destroy();
                flyLogic = null;
            }

            // 胜利
            slots.CallStopAllAnim();

            var ui = await UIManager.OpenUIAsync<User_RateUs>();
            await ui.WaitClose();

            this.SendCommand((ushort)GameEvent.GameSuccess);

            Debug.Log("Level完成");
            //UIManager.OpenViewQueue<WinPage>(new WinPageParam
            //{
            //    onEnd = () =>
            //    {
            //        MainLogic.Instance.GoNextLevel();
            //    }
            //});
        }
    }

    // 游戏进度增加
    public async void AddFillAmount(int type, Vector3 pos)
    {

        if (type == GlobalSingleton.CaItemType())
        {
            var peekReward = GlobalSingleton.PeekSuperCoinValue();
            bool playInsertAD = peekReward.playInsertAD;

            if (peekReward.popState == GlobalSingleton.SuperCoinPopState.None)
            {
                await GlobalSingleton.AsyncGetReward(GameAssetType.SuperCoin, peekReward.value, pos);
            }
            else if (peekReward.popState == GlobalSingleton.SuperCoinPopState.Pop)
            {
                var ui = await UIManager.OpenUIAsync<User_SuperCoinClaim>(new User_SuperCoinClaimParam { reward = peekReward.value });
                var result = await ui.WaitClose<(int, bool)>();


                await GlobalSingleton.AsyncGetReward(GameAssetType.SuperCoin, result.Item1, pos);

                playInsertAD = result.Item2 ? false : playInsertAD;
            }
            else if (peekReward.popState == GlobalSingleton.SuperCoinPopState.PopDouble)
            {
                var ui = await UIManager.OpenUIAsync<User_SuperCoinClaim>(new User_SuperCoinClaimParam
                {
                    isDouble = true,
                    reward = peekReward.value,
                });
                var result = await ui.WaitClose<(int, bool)>();
                await GlobalSingleton.AsyncGetReward(GameAssetType.SuperCoin, result.Item1, pos);
                playInsertAD = result.Item2 ? false : playInsertAD;
            }

            if (playInsertAD)
            {
                using (var handle = new AsyncWaitCallback<int, string>(
                    (code, msg) =>
                    {
                        if (code > 0)
                        {
                            UIManager.ShowToast(msg);
                            return;
                        }
                        if (GlobalSingleton.InsertAdSpan > 0)
                        {
                            GlobalSingleton.PlayedADByGetSuperCoinCnt = GlobalSingleton.InsertAdSpan;
                        }
                    }))
                {
                    ShowADManager.PlayInterstAD("GetSuperCoin", handle.waitCallback);
                    await handle.AsyncWait();
                }
            }
        }

        this.SendCommand((ushort)GameEvent.EliminateEnd);
    }


    // 点击消除商品
    public void OnClickedGoods(GamePlay_Goods goods)
    {
        if (slots.IsFail() || !GoodsCanClickSwitch) return;
        // 记录操作
        ActionLogManager.AddActionLog(new ActionLogParmas(goods));
        // 从仓库移除物体
        var result = storage.CallRemoveGoods(goods);
        var shelf = result.cabinet;
        // 放入暂存区            
        slots.CallAddGoods(result.goods);
        // 如果货柜空了，进行移除操作
        if (shelf != null && shelf.IsNull()) storage.RemoveBox(shelf);
    }

    //public int GetCurLevelItemAllCount()
    //{
    //    return _curLevelItemAllCount;
    //}

    //public int GetCurLevelCleanItemCount()
    //{
    //    return slots.GetCurLevelCleanGoodsCount();
    //}

    #region 道具

    // 点击增加火车
    public async Task OnClickedUnlockTrain()
    {
        var ui = await UIManager.OpenUIAsync<User_NewSpace>();
        var result = await ui.WaitClose<bool>();

    }

    public bool OnUseRemoveProp()
    {
        if (GlobalSingleton.RemoveTool <= 0)
        {
            UIManager.OpenUI<User_BuyProps>(new User_BuyPropsParam
            {
                gameAssetType = GameAssetType.RemoveTool,
            });
            return false;
        }
        if (slots.IsEmpty())
        {
            UIManager.ShowToast("no item");
            return false;
        }
        GlobalSingleton.RemoveTool--;

        var goods = slots.GetGoodsByIDX(0);
        if (goods == null) return false;

        var goods2 = slots.GetGoodsByIDX(1);
        // 从仓库移除物体
        var floorGoods1 = storage.GetGoodsByGoodsType(goods.ItemType);
        var shelf = storage.EliminateGoods(floorGoods1);
        if (shelf.IsNull()) storage.RemoveBox(shelf);

        if (goods2 == null || goods.ItemType != goods2.ItemType)
        {
            var floorGoods2 = storage.GetGoodsByGoodsType(goods.ItemType);
            shelf = storage.EliminateGoods(floorGoods2);
            if (shelf.IsNull()) storage.RemoveBox(shelf);

            slots.EliminateGoods(new List<GamePlay_Goods> { goods });
        }
        else if (goods2 != null && goods.ItemType == goods2.ItemType)
        {

            slots.EliminateGoods(new List<GamePlay_Goods> { goods, goods2 });
        }
        else
        {
            Debug.Log("调用错误, 请检查" + goods + " and " + goods2);
        }


        return true;
    }

    // 点击撤销道具
    public bool OnUseBackProp(bool igroneCost = false)
    {
        if (!igroneCost && GlobalSingleton.BackTool <= 0)
        {
            UIManager.OpenUI<User_BuyProps>(new User_BuyPropsParam
            {
                gameAssetType = GameAssetType.BackTool,
            });
            return false;
        }

        // 没有操作记录
        if (ActionLogManager.IsEmpty())
        {
            UIManager.ShowToast("no record");
            return false;
        }
        if (!igroneCost)
            GlobalSingleton.BackTool--;
        //DataCtrlMgr.AddDataByInt(GameConst.DATA_ASSET_BACK, -1);

        // 提取最后一步操作
        var record = ActionLogManager.DestroyAtLast();
        // 仓库恢复物品
        storage.CallRestoreGoods(record);
        // 暂存区删除物品
        slots.CallRemoveGoods(record);
        return true;
    }

    // 点击打乱道具
    public bool OnUseShuffleProp()
    {
        if (GlobalSingleton.RefreshTool <= 0)
        {
            UIManager.OpenUI<User_BuyProps>(new User_BuyPropsParam
            {
                gameAssetType = GameAssetType.RefreshTool,
            });
            return false;
        }

        GlobalSingleton.RefreshTool--;
        //DataCtrlMgr.AddDataByInt(GameConst.DATA_ASSET_SHUFFLE, -1);

        storage.RandomShuffleGoods();
        //CommandManager.CallCommand(GameConst.EV_PROP_SHUFFLE);
        return true;
    }

    public void PlayEliminateEffect(Vector3 startPos, bool playTrail = false, Action onComplete = null)
    {
        Sequence seq = DOTween.Sequence();
        Transform parent = transform;
        if (UIManager.TryGetUI<User_GamePlay>(out var user_GamePlay))
        {
            parent = user_GamePlay.GetCanvasLayerTransfrom();
        }
        seq.AppendCallback(() =>
    {
        //钞票效果
        var go = GlobalAssetSingleton.eliminateParticlePool.Spawn();
        go.transform.SetParent(parent);
        go.transform.position = startPos;

    });

        seq.AppendInterval(0.2f);

        seq.AppendCallback(() =>
        {
            if (playTrail)
            {

                //飞行粒子效果
                var tmp = GlobalAssetSingleton.trailParticlePool.Spawn(true, (resultGO) =>
            {
                resultGO.SetActive(false);
                resultGO.transform.SetParent(parent);
                resultGO.transform.position = new Vector3(startPos.x, startPos.y, user_GamePlay.transform.position.z);

            });



                tmp.transform.DoSinLerpMove(user_GamePlay.fillAmount.transform.position, 1f, RandomHelp.RandomRange(-4, 4));

                //Debug.Log($"startPos={startPos}, barPos={barLogic.amountPos}");
            }
        });
        seq.AppendInterval(1.0f);
        seq.AppendCallback(() =>
        {
            //飞行粒子击中效果
            AudioManager.AudioPlayer.PlayOneShot(SoundName.HitBar);
            var tmp = GlobalAssetSingleton.hitBarParticlePool.Spawn(true, onSet: (resutlGo) =>
            {
                resutlGo.transform.SetParent(parent);


                resutlGo.transform.position = user_GamePlay.fillAmount.transform.position;
                //tmp.transform.DoRandomLerpMove(comp.fillAmount.transform.position, 1f);


            });

            this.SendCommand((ushort)GameEvent.RefreshProgressBar);
        });

        seq.AppendCallback(() =>
        {
            onComplete?.Invoke();
        });

    }

    public void PlayBoxEffect(Vector3 pos)
    {
        //var go = boxParticle.Get();
        //go.transform.SetParent(transform);
        //go.transform.position = pos;
    }
    float timer = 0;
    private void Update()
    {
        if (GlobalSingleton.GameRunning)
        {
            timer += Time.deltaTime;
            if (timer > 1f)
            {
                timer = 0;
                GlobalSingleton.PlayTimeBySeconds += 1;
            }


            if (flyLogic != null)
                flyLogic.Update();

        }
    }

    private void OnDestroy()
    {
        if (flyLogic != null)
        {
            flyLogic.Destroy();
            flyLogic = null;
        }
    }

    #endregion

    #region 消息
    [CmdCallback((ushort)GameEvent.UnlockNewSpace)]
    private void UnlockTrain()
    {
        slots.UnlockTrain();
    }

    bool isChanged = false;

    [CmdCallback((ushort)GameEvent.GameSuccess)]
    private async Task OnGameWin()
    {
        if (isChanged) return;
        isChanged = true;

        var level = ++GlobalSingleton.Level;
        var ui = await UIManager.OpenUIAsync<User_WinPop>();

        await ui.AsyncClose();

        var loading = await UIManager.OpenUIAsync<User_Loading>();

        _ = InitData(level);
        await Task.Delay(500);
        await loading.AsyncClose();

        isChanged = false;


        this.SendCommand((ushort)GameEvent.RefreshProgressBar);

    }

    [CmdCallback((ushort)GameEvent.GameFail)]
    private async Task OnGameFail()
    {
        if (isChanged) return;
        isChanged = true;

        var ui = await UIManager.OpenUIAsync<User_FailPop>();

        await ui.WaitClose();

        var loading = await UIManager.OpenUIAsync<User_Loading>();
        var level = GlobalSingleton.Level;
        await InitData(level);
        await Task.Delay(500);
        await loading.AsyncClose();
        isChanged = false;

        this.SendCommand((ushort)GameEvent.RefreshProgressBar);
    }

    //private void OnResurrection()
    //{
    //    // 没有操作记录
    //    if (ActionLogManager.IsEmpty())
    //    {
    //        //UIToast.I.Show("no record"); --!
    //        return;
    //    }

    //    // 提取最后一步操作
    //    var record = ActionLogManager.DestroyAtLast();
    //    // 仓库恢复物品
    //    storage.CallRestoreGoods(record);
    //    // 暂存区删除物品
    //    slots.CallRemoveGoods(record);
    //}

    #endregion
}
