using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class GamePlay_Storage : MonoSingleton<GamePlay_Storage>
{
    public GamePlay_ShelfLayer layerPrefab;
    //public SkeletonGraphic boxEffect;
    public Transform transInvenTop;
    public Transform transInvenBottom;
    public Image imaInvenBg;
    //public Transform snowSkeletonPos;
    //public SkeletonGraphic snowSkeleton;

    //public ParticleSystem magicDustParticle;

    // 货柜会掉落到其他行，要确保索引唯一
    public static int Sid;
    private readonly List<GamePlay_ShelfLayer> _layerList = new();
    public List<GamePlay_Goods> guideTempGoodsList { get; set; } = new();


    private void Awake()
    {

    }

    private void OnDestroy()
    {
        transform.DOKill();
    }

    private void Start()
    {
        //magicDustParticle.Play();
        //snowSkeleton.transform.position = snowSkeletonPos.position;
        //imaInvenBg.rectTransform.sizeDelta = new Vector2(Screen.width, 4000);
    }

    public void CleanFloor()
    {
        foreach (var floor in _layerList)
        {
            Destroy(floor.gameObject);
        }

        _layerList.Clear();
    }

    public void InitData()
    {
        foreach (var row in _layerList)
        {
            Destroy(row.gameObject);
        }

        _layerList.Clear();

        Sid = 0;
        var minVolume = GlobalSingleton.GetCanbinetMinLength();
        var maxVolume = GlobalSingleton.GetCanbinetMaxLength();
        var floorCount = GlobalSingleton.GetLayerCountMax();

        var overGroupFloor = RandomHelp.RandomPointByLenght(floorCount, GlobalSingleton.GetOverlayInterval());

        for (var i = 0; i < floorCount; i++)
        {
            var floor = SpawnFloor();
            floor.FillFloor(_layerList, minVolume, maxVolume, overGroupFloor.Contains(i));
        }

        var itemVolume = GetVol();

        // 确保货栈容量是3的倍数
        if (itemVolume % 3 != 0)
        {
            var row = SpawnFloor();
            row.FillFloor(_layerList, 3 - itemVolume % 3 + 3, false);
            // 更新Items
        }

        var items = GetGoodsItems();
        var groupItems = GetGroupGoodsItems();
        #region 新手指引

        //_guideTempGoodsList.Clear();
        //if (GlobalSingleton.GuideState == 1)
        //{
        //    _guideTempGoodsList.AddRange(items.GetRange(0, 3));
        //    items.RemoveRange(0, 3);

        //    foreach (var item in _guideTempGoodsList)
        //    {
        //        item.SetItemType(AppExcuteFlagSettings.ToBFlag ? GlobalSingleton.CaItemType() : 1);
        //    }
        //}

        #endregion

        // 乱序items
        //var randomNum = new System.Random();
        //for (var i = 0; i < items.Count; i++)
        //{
        //    var index = randomNum.Next(0, items.Count - 1);
        //    if (index == i) continue;
        //    (items[i], items[index]) = (items[index], items[i]);
        //}
        var groupItemsCnt = groupItems.Select(gitem => gitem.MaxGroupCount).Sum();
        var groupCnt = (items.Count + groupItemsCnt) / 3;

        if (GlobalSingleton.GuideState == 1)
        {
            groupCnt -= 1;//抽离一组作为引导
        }

        var elementNumber = GlobalSingleton.GetGoodsTypeCount();
        var coinGroup = Mathf.FloorToInt(groupCnt * GlobalSingleton.GetGoldRate() * 0.01f);
        var insertItemTypeList = new List<int>();



        int itemCount = 0;
        //算出需要添加的Group;
        for (var i = 0; i < groupCnt; i++)
        {
            var type = Random.Range(1, elementNumber);
            if (AppExcuteFlagSettings.ToBFlag && i < coinGroup)
            {
                // 钞票产出
                type = GlobalSingleton.CaItemType();
            }
            insertItemTypeList.Add(type);
            insertItemTypeList.Add(type);
            insertItemTypeList.Add(type);

        }

        //LogList(insertItemTypeList);

        var randomNum = new System.Random();
        for (var i = 0; i < insertItemTypeList.Count; i++)
        {
            var index = randomNum.Next(0, insertItemTypeList.Count - 1);
            if (index == i) continue;
            (insertItemTypeList[i], insertItemTypeList[index]) = (insertItemTypeList[index], insertItemTypeList[i]);
        }

        for (int i = 0; i < groupItems.Count; i++)
        {
            var item = groupItems[i];
            for (int j = 0; j < item.MaxGroupCount; ++j)
            {
                var t = insertItemTypeList[itemCount++];
                item.AddItemType(t);
            }
        }

        guideTempGoodsList.Clear();
        int startCnt = 0;
        if (GlobalSingleton.GuideState == 1)
        {
            int guideItemType = AppExcuteFlagSettings.ToBFlag ? GlobalSingleton.CaItemType() : 1;
            items[0].SetItemType(guideItemType);
            items[1].SetItemType(guideItemType);
            items[2].SetItemType(guideItemType);
            guideTempGoodsList.Add(items[0]);
            guideTempGoodsList.Add(items[1]);
            guideTempGoodsList.Add(items[2]);

            startCnt = 3;
        }

        for (int i = startCnt; i < items.Count; i++)
        {
            var t = insertItemTypeList[itemCount++];
            items[i].SetItemType(t);
        }

        //float y = transInvenTop.position.y - transInvenBottom.position.y;
        //gameObject.GetComponent<RectTransform>().localPosition = new Vector3(-500, -_floorsLs.Count * 240 + y, 0);
        //transform.DOLocalMoveY(0, 2f).SetDelay(0.5f);
        //imaInvenBg.rectTransform.sizeDelta = new Vector2(Screen.width, 4000 + _floorsLs.Count * 240);
        //imaInvenBg.rectTransform.localPosition = new Vector3(0, -_floorsLs.Count * 240 + y, 0);
        //imaInvenBg.transform.DOLocalMoveY(-transInvenBottom.localPosition.y, 2).SetDelay(0.5f).OnComplete(() =>
        //{
        //    //magicDustParticle.Play();
        //});
    }

    private void LogList(List<int> list)
    {
        Dictionary<int, int> dict = new Dictionary<int, int>();
        for (int i = 0; i < list.Count; i++)
        {
            var idx = list[i];
            if (dict.ContainsKey(list[i]))
            {
                dict[list[i]]++;
            }
            else
            {
                dict[list[i]] = 1;
            }
        }

        foreach (var kvp in dict)
        {
            Debug.Log($"Random Item id : {kvp.Key} -> count : {kvp.Value}");
        }
    }

    public bool IsEmpty()
    {
        return _layerList.All(row => row.shelves.Count <= 0);
    }

    public int GetVol()
    {
        var result = 0;
        foreach (var floor in _layerList)
        {
            foreach (var shelf in floor.shelves)
            {
                if (!shelf.IsItemGroup)
                    result += shelf.cabinetCount;
                else
                {
                    foreach (var goods in shelf.items)
                    {
                        result += goods.MaxGroupCount;
                    }
                }
            }
        }

        return result;
    }



    //public List<GamePlay_Goods> GetGuideGoods()
    //{
    //    return guideTempGoodsList;
    //}

    #region Row相关操作

    // 创建货柜行
    private GamePlay_ShelfLayer SpawnFloor()
    {
        var floor = Instantiate(layerPrefab, transform, false);
        floor.InitData(_layerList.Count);
        _layerList.Add(floor);
        return floor;
    }

    // 删除货柜行
    private void DestroyFloor(int floorIndex)
    {
        // 遍历下移的元素，row全部 -1
        var rowCount = GetActiveFloorCount();
        for (var i = floorIndex; i < rowCount; i++)
        {
            var row = _layerList[i];
            var dropRow = _layerList[i + 1];
            for (var j = dropRow.shelves.Count - 1; j >= 0; j--)
            {
                var shelf = dropRow.shelves[j];
                shelf.layerIndex -= 1;
                dropRow.RemoveBox(shelf);
                row.AddBox(shelf);
            }
        }
    }

    // 获得有货柜的行数
    private int GetActiveFloorCount()
    {
        return _layerList.Count(row => row.shelves.Count > 0);
    }

    // 遍历检查行是否有改变
    private void LoopCheckFloor()
    {
        var loopMax = 200;
        var isLoop = true;
        while (isLoop && loopMax > 0)
        {
            var isChange = false;
            var rowCount = GetActiveFloorCount();
            for (var i = 0; i < rowCount; i++)
            {
                if (CheckFloorChanged(i))
                {
                    isChange = true;
                    break;
                }
            }

            loopMax--;
            if (!isChange) isLoop = false;
        }

        InfiniteFloor();
    }

    // 检查行是否有改变
    private bool CheckFloorChanged(int floorIndex)
    {
        // 到顶了，不用判断
        var floorCount = GetActiveFloorCount();
        if (floorIndex >= floorCount) return false;

        var row = _layerList[floorIndex];
        // 整一层空了,删除整一层
        if (row.shelves.Count <= 0)
        {
            DestroyFloor(floorIndex);
            return true;
        }
        else if (row.shelves.Count != transform.childCount)
        {
            //货架数量不对,重新刷新
            row.shelves.Clear();
            foreach (Transform child in row.transform)
            {
                var box = child.GetComponent<GamePlay_Cabinet>();
                if (box != null)
                {
                    if (!box.IsNull())
                        row.shelves.Add(box);
                    else
                    {
                        Destroy(box.gameObject);
                    }
                }
            }
        }


        // 最底下，不用判断能否掉下去
        if (floorIndex <= 0) return false;
        var isRemove = false;
        for (var i = row.shelves.Count - 1; i >= 0; i--)
        {
            var shelve = row.shelves[i];
            var can = _layerList[floorIndex - 1].CanAddBox(shelve.startPointPos, shelve.cabinetCount);
            // 不能就插入就判断下一个
            if (!can) continue;

            // 能插入，货柜就要掉一层
            isRemove = true;
            _layerList[floorIndex].RemoveBox(shelve);
            _layerList[floorIndex - 1].AddBox(shelve);
        }

        return isRemove;
    }

    // 无限关
    private void InfiniteFloor()
    {
        // 小于5关不开启
        if (GlobalSingleton.Level < 4) return;
        var floorCount = GetActiveFloorCount();
        var needAddRowCount = GlobalSingleton.GetLayerCountMax() - floorCount;
        if (needAddRowCount <= 0) return;

        // 添加n个空的Row
        var itemsVolume = GetVol();
        var minVolume = GlobalSingleton.GetCanbinetMinLength();
        var maxVolume = GlobalSingleton.GetCanbinetMinLength();
        for (var i = 0; i < needAddRowCount; i++)
        {
            var row = SpawnFloor();
            row.FillFloor(_layerList, minVolume, maxVolume, false);
        }

        var objects = GetGoodsItems();
        var items = objects.GetRange(itemsVolume, objects.Count - itemsVolume);
        //Debug.Log($"=====> objects : {objects.Count} | items : {items.Count}");

        var typeCount = GlobalSingleton.GetGoodsTypeCount();
        foreach (var item in items)
        {
            var type = Random.Range(1, typeCount);
            if (AppExcuteFlagSettings.ToBFlag && Random.Range(0, 100) > 100 - GlobalSingleton.GetInfiniteRate())
            {
                // 钞票和亚马逊卡随机产出
                type = GlobalSingleton.CaItemType();
            }

            item.SetItemType(type);
        }

        //ClearEmptyFloor();
        ClearEmptyFloor();
    }

    private void ClearEmptyFloor()
    {
        List<GamePlay_Cabinet> boxs = new List<GamePlay_Cabinet>();
        foreach (var row in _layerList)
        {
            foreach (var shelf in row.shelves)
            {
                if (shelf.IsNull()) boxs.Add(shelf);
            }

        }

        for (int i = 0; i < boxs.Count; i++)
        {
            var targetBox = boxs[i];
            var row = _layerList[targetBox.layerIndex];
            row.RemoveBox(targetBox);
        }

    }

    #endregion

    #region Shelf相关

    // 删除Shelf，后续会触发Row元素排序
    public void RemoveBox(GamePlay_Cabinet targetBox)
    {
        var row = _layerList[targetBox.layerIndex];
        row.RemoveBox(targetBox);

        //TODO 箱子炸裂特效
        //ShelfGameContent.Inst.PlayBoxEffect(targetBox.transform.position +
        //           new Vector3(targetBox.rectTransform.rect.width / 400,
        //               targetBox.rectTransform.rect.height / 400, 0));
        AudioManager.AudioPlayer.PlayOneShot(SoundName.BoxElimination);
        Destroy(targetBox.gameObject);

        LoopCheckFloor();
    }

    #endregion

    #region Item相关

    // 获取全部商品位
    //private List<GamePlay_Goods> _GetGoods()
    //{
    //    var items = new List<GamePlay_Goods>();
    //    foreach (var row in _layerList)
    //    {
    //        foreach (var shelf in row.shelves)
    //        {
    //            foreach (var item in shelf.items)
    //            {
    //                if (item.ItemType >= 0)
    //                {
    //                    items.Add(item);
    //                }
    //            }
    //        }
    //    }

    //    return items;
    //}

    // 获取显示的商品位
    private List<GamePlay_Goods> GetGoodsItems(bool includeCabinet = false)
    {
        var items = new List<GamePlay_Goods>();
        foreach (var row in _layerList)
        {
            foreach (var shelf in row.shelves)
            {
                if (includeCabinet)
                    items.AddRange(shelf.items);
                else if (!shelf.IsItemGroup)
                {
                    items.AddRange(shelf.items);
                }
            }
        }

        return items;
    }

    private List<GamePlay_Goods> GetGroupGoodsItems()
    {
        var items = new List<GamePlay_Goods>();
        foreach (var row in _layerList)
        {
            foreach (var shelf in row.shelves)
            {
                if (shelf.IsItemGroup)
                    items.AddRange(shelf.items);
            }
        }
        return items;
    }

    //private List<GamePlay_Goods> GetEmptyItems()
    //{
    //    var items = new List<GamePlay_Goods>();
    //    foreach (var row in _layerList)
    //    {
    //        foreach (var shelf in row.shelves)
    //        {
    //            foreach (var item in shelf.items)
    //            {
    //                if (!item.setted)
    //                {
    //                    items.Add(item);
    //                }
    //            }
    //        }
    //    }
    //    return items;
    //}

    //public int GetActiveGoodsCount()
    //{
    //    return GetGoodsItems().Count(item => item.ItemType >= 0);
    //}

    public GamePlay_Goods GetGoodsByGoodsType(int goodsType)
    {
        foreach (var row in _layerList)
        {
            foreach (var shelf in row.shelves)
            {
                foreach (var item in shelf.items)
                {
                    if (item.ItemType == goodsType)
                    {
                        return item;
                    }
                }
            }
        }

        return null;
    }

    public GamePlay_Cabinet EliminateGoods(GamePlay_Goods goods)
    {
        var box = CallRemoveGoods(goods);
        GamePlay_ShelfGame.Instance.PlayEliminateEffect(goods.transform.position + new Vector3(0, 0.2f, 0));
        Destroy(goods.gameObject);
        return box.cabinet;
    }

    // 移除商品
    public (GamePlay_Cabinet cabinet, GamePlay_Goods goods) CallRemoveGoods(GamePlay_Goods goods)
    {
        if (goods.ItemCount == 1)
        {
            // 从仓库移除item
            goods.btnClick.interactable = false;
            var row = _layerList[goods.layerIndex];
            var shelf = row.GetBox(goods.cabinetIndex);
            shelf.RemoveGoods(goods);
            return (shelf, goods);
        }
        else
        {
            var tmpGoods = goods.PopGoods();
            var row = _layerList[goods.layerIndex];
            var shelf = row.GetBox(goods.cabinetIndex);
            shelf.OnGroupItemPop();
            tmpGoods.btnClick.interactable = false;
            return (shelf, tmpGoods);
        }

    }

    // 恢复商品
    public void CallRestoreGoods(ActionLogParmas recordParmas)
    {
        var row = _layerList[recordParmas.layerIndex];
        var box = row.GetBox(recordParmas.cabinetIndex);
        if (box != null)
        {
            // 货柜还存在
            box.AddGoods(recordParmas);

        }
        else
        {
            box = row.CreateBox(recordParmas);
            box.AddGoods(recordParmas);

            box.layerIndex -= 1;
            var result = new List<GamePlay_Cabinet> { box };
            var nextCollideShelves = new List<GamePlay_Cabinet> { box };
            var rowCount = GetActiveFloorCount();
            for (var i = box.layerIndex + 1; i < rowCount; i++)
            {
                if (nextCollideShelves.Count <= 0) break;
                // 临时结果
                var tempShelves = new Dictionary<int, GamePlay_Cabinet>();
                // 找到所有碰撞的货栈
                foreach (var ncs in nextCollideShelves)
                {
                    var collideShelves =
                        _layerList[ncs.layerIndex + 1].GetCollideShelves(ncs.startPointPos, ncs.cabinetCount);
                    foreach (var collideShelf in collideShelves)
                    {
                        tempShelves[collideShelf.cabinetIndex] = collideShelf;
                    }
                }

                // 清空下一个row的货栈list
                nextCollideShelves.Clear();
                foreach (var (_, s) in tempShelves)
                {
                    result.Add(s);
                    nextCollideShelves.Add(s);
                }
            }

            for (var i = result.Count - 1; i >= 0; i--)
            {
                var s = result[i];
                if (s.layerIndex >= 0) _layerList[s.layerIndex].RemoveBox(s);
                if (s.layerIndex + 1 >= _layerList.Count) SpawnFloor();
                _layerList[s.layerIndex + 1].AddBox(s);
            }
        }
    }

    #endregion

    #region 乱序道具

    private bool _isDoShufflying;

    public override bool DontDestory => false;

    public void RandomShuffleGoods()
    {
        if (_isDoShufflying) return;
        _isDoShufflying = true;

        var items = GetGoodsItems(true);// _GetGoods();

        List<int> allItemID = new List<int>();
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            if (item.IsGroup)
            {
                allItemID.AddRange(item.ItemTypesArr);
            }
            else
            {
                allItemID.Add(item.ItemType);
            }
        }

        var seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            // 乱序ItemType
            var random = new System.Random();
            for (var i = 0; i < allItemID.Count; i++)
            {
                var index = random.Next(0, allItemID.Count - 1);
                if (index == i) continue;
                (allItemID[i], allItemID[index]) = (allItemID[index], allItemID[i]);
            }

            foreach (var item in items)
            {
                // 禁止点击
                item.btnClick.interactable = false;
                // 移动到中间
                var rect = ((RectTransform)transform.parent).rect;
                item.transform.DOMove(new Vector3(rect.width / 2, rect.height / 2, 0), 0.5f);
            }
        });
        seq.AppendInterval(0.55f);
        seq.AppendCallback(() =>
        {
            List<int> tmpLs = new List<int>();
            foreach (var item in items)
            {
                // 回到原点
                item.transform.DOLocalMove(item.GetLocalPos(), 0.5f);
                // 重新设置ItemType
                if (item.IsGroup)
                {
                    tmpLs.Clear();
                    var cnt = item.ItemCount;
                    for (int i = 0; i < cnt; i++)
                    {
                        if (allItemID.TryPop(out var resultValue))
                        {
                            tmpLs.Add(resultValue);
                        }
                    }
                    item.ReplaceItemArrType(tmpLs);
                }
                else
                    item.SetItemType(item.ItemType);
            }
        });
        seq.AppendInterval(0.55f);
        seq.AppendCallback(() =>
        {
            // 恢复点击
            foreach (var item in items)
            {
                item.btnClick.interactable = true;
            }

            _isDoShufflying = false;
        });
    }

    #endregion
}
