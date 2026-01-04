using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// 一层
/// </summary>
public class OneFloor : MonoBehaviour
{
    private const int OneFloorLeftOffset = 20;

    private const int OneFloorCapacity = 6;
    private const int OneFloorOffset = 20;

    public CabinetUnit cabinetUnitPrefab;

    public int OneFloorIndex { get; set; }
    public List<CabinetUnit> CabinetUnitList { get; set; } = new();
    public RectTransform rectTransform { get; private set; }


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void InitData(int index)
    {
        OneFloorIndex = index;

        transform.localPosition = GetLocalPosition();
    }

    #region Row相关操作

    // 根据范围去随机填充货柜
    public void FillLayer(List<OneFloor> rows, int min, int max, bool hasGroup)
    {
        //int loop = isMax ? 100 : 10;
        //Debug.Log($"{minCapacity} - {maxCapacity}");

        //for (var i = 0; i < 100; i++)
        //{
        //    var tempMax = maxCapacity - i / 20;
        //    var newMax = Mathf.Max(1, tempMax) + 1;

        //    //if (i > 60) maxCapacity = Mathf.Min(2, maxCapacity);
        //    //if (i > 80) maxCapacity = Mathf.Min(1, maxCapacity);

        //    var startPoint = Random.Range(0, FloorCapacity);
        //    var capacity = Random.Range(minCapacity, newMax);
        //    //if (ShelfGameContent.Level == 0)
        //    //{
        //    //    startPoint = GetGuildStartPoint(rid);
        //    //    if (isMax) capacity = 6;
        //    //}
        //    // 越界判断
        //    if (startPoint + capacity > FloorCapacity) continue;
        //    var can = CanAddBox(startPoint, capacity);
        //    // 当前行能不能插入
        //    if (!can) continue;

        //    CreateBox(rows, startPoint, capacity);
        //}

        int startPoint = RandomHelp.RandomRange(0, OneFloorCapacity); // 起始点
        int floorIDX = (rows.Count % GlobalSingleton.GetSparsityByFloorCnt());
        var sparsity = GlobalSingleton.GetSparsityByFloor(floorIDX);
        max = Mathf.RoundToInt(Mathf.Lerp(min, max, sparsity.max));
        min = Mathf.RoundToInt(Mathf.Lerp(min, max, sparsity.min));

        //var hollowOut = GlobalSingleton.GetHollowOutByFloor(floorIDX);
        //var basic = hollowOut.basic; // 镂空基础概率
        int cntLenght = 0; //累计的长度

        while (cntLenght < OneFloorCapacity)
        {
            bool isGroup = false;
            int randBoxLenght = RandomHelp.RandomRange(min, Mathf.Min(max, OneFloorCapacity - cntLenght));

            //随机出箱子的长度
            randBoxLenght = Mathf.Clamp(randBoxLenght, 0, OneFloorCapacity - cntLenght);

            if (startPoint + randBoxLenght > OneFloorCapacity)
            {
                randBoxLenght = OneFloorCapacity - startPoint;
            }

            if (hasGroup)
            {
                isGroup = true;
                hasGroup = false;
                randBoxLenght = 1;
            }

            cntLenght += randBoxLenght;

            CreateBox(rows, startPoint, randBoxLenght, isGroup);

            int startPointPlus = 0;

            //if (RandomHelp.RandomTesting(basic))
            //{
            //    startPointPlus = RandomHelp.RandomRange(1, 3); //后移1-2
            //    basic = hollowOut.basic;
            //}
            //else
            //{
            //    basic += hollowOut.plus;//叠加基础概率
            //}

            cntLenght += startPointPlus;
            startPoint = startPoint.Next(0, OneFloorCapacity, randBoxLenght + startPointPlus);
        }


    }

    // 新手关卡
    private static int GetGuildStartPoint(int rowIndex)
    {
        var s1 = new[] { 0, 4 };
        var s2 = new[] { 1, 3 };
        var s = rowIndex % 2 == 0 ? s1 : s2;
        return s[Random.Range(0, 2)];
    }

    // 指定大小去填充货柜
    public void FillLayer(List<OneFloor> rows, int capacity, bool isGroup)
    {
        CreateBox(rows, 0, capacity, isGroup);
    }

    #endregion

    #region Shelf相关操作

    // 添加货柜
    public void AddBox(CabinetUnit targetBox)
    {
        targetBox.oneLayerIndex = OneFloorIndex;
        foreach (var item in targetBox.items)
        {
            item.layerIndex = OneFloorIndex;
        }
        targetBox.transform.SetParent(transform);
        targetBox.transform.DOKill();
        targetBox.transform.DOLocalMove(targetBox.GetLocalPosition(), 0.2f);
        targetBox.transform.DOLocalMoveY(10, 0.1f).SetDelay(0.2f).SetLoops(2, LoopType.Yoyo);

        CabinetUnitList.Add(targetBox);
    }

    // 移除货柜
    public void RemoveBox(CabinetUnit targetBox)
    {
        targetBox.transform.DOKill();
        for (var i = CabinetUnitList.Count - 1; i >= 0; i--)
        {
            var shelf = CabinetUnitList[i];
            if (shelf.cabinetUnitIndex != targetBox.cabinetUnitIndex) continue;

            CabinetUnitList.RemoveAt(i);
            break;
        }
    }

    // 获取货柜
    public CabinetUnit GetBox(int index)
    {
        foreach (var shelf in CabinetUnitList)
        {
            if (shelf.cabinetUnitIndex == index) return shelf;
        }

        return null;
    }

    // 创建货柜
    private void CreateBox(List<OneFloor> rows, int startPoint, int capacity, bool isGroup)
    {
        var row = this;
        for (var i = OneFloorIndex - 1; i >= 0; i--)
        {
            if (!rows[i].CanAddBox(startPoint, capacity)) break;
            row = rows[i];
        }


        var shelf = Instantiate(cabinetUnitPrefab, row.transform, false);
        shelf.InitData(row, StorageUnit.Sid++, startPoint, capacity, isGroup);
        row.CabinetUnitList.Add(shelf);
    }

    // 根据记录恢复货柜
    public CabinetUnit CreateBox(ActionLogParmas recordParmas)
    {
        var shelf = Instantiate(cabinetUnitPrefab, transform, false);
        shelf.InitData(this, recordParmas.cabinetIndex, recordParmas.startPointPos, recordParmas.cabinetCount, recordParmas.isGroup);
        return shelf;
    }

    #endregion

    #region Shelf插入判断

    // 是否可以插入到货柜行中
    public bool CanAddBox(int startPoint, int capacity)
    {
        return GetCollideShelves(startPoint, capacity).Count <= 0;
    }

    // 获取碰撞货柜列表
    public List<CabinetUnit> GetCollideShelves(int startPoint, int capacity)
    {
        var result = new List<CabinetUnit>();
        var minA = startPoint;
        var maxA = startPoint + capacity;
        foreach (var shelf in CabinetUnitList)
        {
            var minB = shelf.beginPos;
            var maxB = shelf.beginPos + shelf.cabinetUnitCount;
            // https://blog.csdn.net/mrwangweijin/article/details/76302778
            if (Mathf.Max(minA, minB) < Mathf.Min(maxA, maxB))
            {
                result.Add(shelf);
            }
        }

        return result;
    }

    #endregion

    public Vector3 GetLocalPosition()
    {
        var rowHeight = rectTransform.rect.height - OneFloorOffset;
        return new Vector3(OneFloorLeftOffset, rowHeight * OneFloorIndex, 0);
    }
}
