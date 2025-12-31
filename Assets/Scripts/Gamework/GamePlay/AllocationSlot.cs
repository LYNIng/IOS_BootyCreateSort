using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AllocationSlot : MonoCompositeSingleton<AllocationSlot>, IMsgObj
{
    public override bool DontDestory => false;

    //public List<SkeletonGraphic> cleanEffectList;
    public RectTransform maskContent;
    public List<Transform> normalTrains;
    public SecuredAllocation unlockSeat;

    public List<Transform> trainList;

    public CanvasGroup seatsGroup;

    private readonly Queue<Sequence> _sequencesQueue = new();
    private readonly List<StockItem> stockItemList = new();

    private int curItemsCount = 0;

    private void Start()
    {
        maskContent.sizeDelta = new Vector2(Screen.width, Screen.height);

    }

    private Vector3 srcPos;
    public void InitData()
    {
        unlockSeat.ResetThis();
        trainList.Clear();
        trainList.AddRange(normalTrains);
        foreach (var item in stockItemList)
        {
            item.transform.DOKill();
            Destroy(item.gameObject);
        }

        stockItemList.Clear();

        ClearContent();
        curItemsCount = 0;

        seatsGroup.alpha = 0;
        srcPos = seatsGroup.transform.localPosition;
        seatsGroup.transform.localPosition = seatsGroup.transform.localPosition - new Vector3(0, 20f, 0);
    }
    public void ShowSeats()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(seatsGroup.DOFade(1f, 0.2f));
        seq.Join(seatsGroup.transform.DOLocalMove(srcPos, 0.2f));
        seq.Play();
    }

    // 激活广告火车
    public void UnlockTrain()
    {
        unlockSeat.OnUnlockAction();
        trainList.Add(unlockSeat.transform);
    }

    public void ClearContent()
    {
        List<GameObject> list = null;


        for (int i = 0; i < maskContent.childCount; i++)
        {
            if (list == null) list = new List<GameObject>();
            var child = maskContent.GetChild(i);
            list.Add(child.gameObject);
        }
        if (list != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Destroy(list[i]);
            }
        }
    }

    #region 动画相关

    // 停止所有动画
    public void CallStopAllAnim()
    {
        foreach (var seq in _sequencesQueue)
        {
            seq.Kill();
        }

        _sequencesQueue.Clear();
    }

    // 播放排序动画
    private void _CallMoveGoodsAnim(StockItem insertItem = null, bool hasDestroy = false, Action onComplete = null)
    {
        float yOffset = 0.00f;
        for (var i = 0; i < stockItemList.Count; i++)
        {
            var item = stockItemList[i];
            var posTrans = trainList[i];
            if (insertItem == item)
            {
                var seq = DOTween.Sequence();
                var spped = 60f;

                var dPos = posTrans.position - new Vector3(0, yOffset, 0);
                var d = Vector3.Distance(item.transform.position, dPos);
                float dt = d / spped;
                //Debug.Log($"Call Dis :{d} dt: {dt}");
                seq.Append(item.transform.DOMove(dPos, dt).SetEase(Ease.OutQuad));

                if (!hasDestroy)
                {
                    seq.Append(item.transform.DOScale(Vector3.Scale(item.transform.localScale, new Vector3(1.05f, 0.80f, 1f)), 0.15f).SetEase(Ease.OutBack));
                    seq.Append(item.transform.DOScale(Vector3.Scale(item.transform.localScale, new Vector3(0.85f, 0.85f, 1f)), 0.15f).SetEase(Ease.OutBack));
                }
                else
                {
                    seq.Join(item.transform.DOScale(Vector3.Scale(item.transform.localScale, new Vector3(0.85f, 0.85f, 1f)), dt).SetEase(Ease.OutBack));
                }
                seq.OnComplete(() => { onComplete?.Invoke(); });
                seq.Play();
            }
            else
            {
                item.transform.DOMove(posTrans.position - new Vector3(0, yOffset, 0), 0.2f).SetEase(Ease.OutQuart);
                item.transform.DOScale(0.85f, 0.2f).SetEase(Ease.OutQuart);
            }
        }
    }

    #endregion


    #region Item相关操作

    public StockItem GetGoodsByIDX(int index)
    {
        if (index < 0 || index >= stockItemList.Count) return null;
        return stockItemList[index];
    }

    public bool IsFail()
    {
        if (stockItemList.Count >= trainList.Count)
        {
            var goodsType = -1;
            var count = 0;
            for (int i = 0; i < stockItemList.Count; ++i)
            {
                var item = stockItemList[i];
                if (goodsType == item.ItemType)
                {
                    count++;
                    if (count >= 3)
                    {
                        return false;
                    }
                }
                else
                {
                    goodsType = item.ItemType;
                    count = 1;
                }
            }
            return true;
        }
        return false;
    }

    public bool IsEmpty()
    {
        return stockItemList.Count == 0;
    }

    // 获取合成物体
    private List<StockItem> _CallGetCleanGoods()
    {
        var goods = new List<StockItem>();
        var type = -1;
        // 依赖items根据type排序好
        foreach (var good in stockItemList)
        {
            if (good.ItemType != type)
            {
                // 类型不同
                type = good.ItemType;
                goods.Clear();
                goods.Add(good);
            }
            else
            {
                // 类型相同
                goods.Add(good);
                if (goods.Count >= 3) break;
            }
        }
        if (goods.Count < 3) goods.Clear();
        return goods;
    }

    public void EliminateGoods(List<StockItem> targetGoods)
    {
        if (targetGoods == null || targetGoods.Count == 0) return;

        var iGoodsType = targetGoods[0].ItemType;
        for (int i = 0; i < targetGoods.Count; i++)
        {
            targetGoods[i].btnClick.interactable = false;
        }

        if (iGoodsType == GlobalSingleton.CaItemType())
        {
            //delay = 0.8f;
            MiniGame.Instance.ItemCanClick = false;
        }
        var seq = DOTween.Sequence();
        _sequencesQueue.Enqueue(seq);
        //bool waitRmoveAction = false;
        seq.AppendInterval(0.25f);
        seq.AppendCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.Elimination);
            var pos = targetGoods[^1].transform.position;
            // 删除物体
            for (int i = 0; i < targetGoods.Count; i++)
            {
                MiniGame.Instance.PlayEliminateEffect(targetGoods[i].transform.position + new Vector3(0, 0.2f, 0),
                    i == 0,
                    i == 0 ? () =>
                {
                    _sequencesQueue.Dequeue();
                    MiniGame.Instance.AddFillAmount(targetGoods[0].ItemType, pos);
                    MiniGame.Instance.ItemCanClick = true;

                    if (_sequencesQueue.Count == 0)
                    {
                        MiniGame.Instance.CheckComplete();
                    }
                    else
                    {
                        Debug.Log("忽略掉的CheckComplete");
                    }
                }
                : null);
                Destroy(targetGoods[i].gameObject);
            }

            //排序动画
            _CallMoveGoodsAnim();
            // 合成加1金币
            if (targetGoods[0].ItemType < GlobalSingleton.CaItemType())
            {
                GlobalSingleton.GetReward(GameAssetType.Coin, 1);
            }
            else
            {
                GlobalSingleton.EliminateSuperCoinCnt++;
            }

            curItemsCount += 3;
            GlobalSingleton.EliminateCnt += 1;
            GlobalSingleton.CurCount += 3;


        });

        _DestroyGoodsByList(targetGoods);
        for (int i = 0; i < targetGoods.Count; i++)
        {
            ActionLogManager.Destroy(targetGoods[i]);
        }
    }

    public void CallAddGoods(StockItem targetGoods, Action onComplete = null)
    {
        targetGoods.btnClick.interactable = false;
        targetGoods.transform.SetParent(maskContent);
        _SortGoods(targetGoods);
        var goods = _CallGetCleanGoods();

        _CallMoveGoodsAnim(targetGoods, goods.Count >= 3, () =>
        {
            _CheckCleanGoods(goods);
        });

        if (goods.Count >= 3)
        {
            // 数据层移除物体
            _DestroyGoodsByList(goods);

            for (int i = 0; i < goods.Count; i++)
            {
                ActionLogManager.Destroy(goods[i]);
            }
            //if (targetGoods.iGoodsType == 21)
            //{
            //    ShelfGameContent.Inst.waitForSettleTimes++;
            //}
        }
    }

    private void _SortGoods(StockItem targetGoods)
    {
        // _items.Add(targetItem);
        // _items.Sort((x, y) => x.type.CompareTo(y.type));
        int index = -1;
        if (stockItemList.Count > 0)
        {
            for (int i = stockItemList.Count - 1; i >= 0; i--)
            {
                if (stockItemList[i].ItemType == targetGoods.ItemType)
                {
                    index = i;
                    break;
                }
            }
        }
        if (index > -1)
        {
            stockItemList.Insert(index + 1, targetGoods);
        }
        else
        {
            stockItemList.Add(targetGoods);
        }
    }

    private void _DestroyGoodsByList(List<StockItem> items)
    {
        if (items.Count <= 0) return;
        for (var i = 0; i < stockItemList.Count; i++)
        {
            if (stockItemList[i].ItemType != items[0].ItemType) continue;
            //Debug.Log($"{i} - {items.Count} - {_goodsList.Count}");
            stockItemList.RemoveRange(i, items.Count);
            //CommandManager.CallCommand(GameConst.EV_COMBINE);
            break;
        }
    }

    private void _CheckCleanGoods(List<StockItem> cleanItemList)
    {
        if (cleanItemList.Count < 3)
        {
            MiniGame.Instance.CheckComplete();
            return;
        }
        var pos = cleanItemList[^1].transform.position;

        var seq = DOTween.Sequence();
        _sequencesQueue.Enqueue(seq);
        //bool waitRmoveAction = false;
        seq.AppendInterval(0.25f);
        seq.AppendCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.Elimination);
            // 合成判断
            if (cleanItemList.Count >= 3)
            {
                // 删除物体
                //waitRmoveAction = true;
                _RemoveGoodsByList(cleanItemList, () =>
                {
                    _sequencesQueue.Dequeue();
                    MiniGame.Instance.AddFillAmount(cleanItemList[0].ItemType, pos);
                    MiniGame.Instance.ItemCanClick = true;
                    if (_sequencesQueue.Count == 0)
                    {
                        MiniGame.Instance.CheckComplete();
                    }
                    else
                    {
                        Debug.Log("忽略掉的移除排序");
                    }

                    //Debug.Log($"Queue {_sequencesQueue.Count}");
                });
                //排序动画
                _CallMoveGoodsAnim();
                // 合成加1金币
                if (cleanItemList[0].ItemType < GlobalSingleton.CaItemType())
                {
                    GlobalSingleton.GetReward(GameAssetType.Coin, 1, startPos: cleanItemList[0].transform.position);
                }
                else
                {
                    GlobalSingleton.EliminateSuperCoinCnt++;
                }

                curItemsCount += 3;
                GlobalSingleton.EliminateCnt += 1;
                GlobalSingleton.CurCount += 3;
            }
            else
            {
                _sequencesQueue.Dequeue();

                MiniGame.Instance.ItemCanClick = true;
                if (_sequencesQueue.Count == 0)
                    MiniGame.Instance.CheckComplete();
            }
        });

    }

    public void CallRemoveGoods(ActionLogParmas recordParmas)
    {
        for (var i = 0; i < stockItemList.Count; i++)
        {
            var item = stockItemList[i];
            if (item.cabinetUnitIndex == recordParmas.cabinetIndex
                && item.stockItemIndex == recordParmas.itemIndex
                && item.ItemType == recordParmas.ItemType)
            {
                stockItemList.RemoveAt(i);
                item.transform.DOKill();
                Destroy(item.gameObject);
                _CallMoveGoodsAnim();
                MiniGame.Instance.BackItemPos = item.transform.position;
                //CommandManager.CallCommand(GameConst.EV_PROP_BACK);
                item.btnClick.interactable = true;
                break;
            }
        }
    }

    private void _RemoveGoodsByList(List<StockItem> items, Action onComplete)
    {
        for (var i = 0; i < items.Count; i++)
        {

            MiniGame.Instance.PlayEliminateEffect(items[i].transform.position + new Vector3(0, 0.2f, 0), i == 1, i == 0 ? onComplete : null);


            Destroy(items[i].gameObject);
        }
    }

    public int GetCurLevelCleanGoodsCount()
    {
        return curItemsCount;
    }

    #endregion
}
