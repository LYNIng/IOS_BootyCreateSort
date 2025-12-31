using System.Collections.Generic;

public class ActionLogParmas
{
    public ActionLogParmas(StockItem goods)
    {
        layerIndex = goods.floorIndex;
        cabinetIndex = goods.cabinetUnitIndex;
        startPointPos = goods.startPointPos;
        cabinetCount = goods.cabinetCount;
        itemIndex = goods.stockItemIndex;
        ItemType = goods.ItemType;
        isGroup = goods.IsGroup;
    }
    public readonly int layerIndex;
    public readonly int cabinetIndex;
    public readonly int startPointPos;
    public readonly int cabinetCount;
    public readonly int itemIndex;
    public readonly int ItemType;
    public readonly bool isGroup;
}
/// <summary>
/// RecordsCtrl
/// </summary>
public static class ActionLogManager
{
    private static readonly List<ActionLogParmas> RecordList = new();

    public static void CallInit()
    {
        RecordList.Clear();
    }

    public static bool IsEmpty()
    {
        return RecordList.Count <= 0;
    }

    public static void AddActionLog(ActionLogParmas recordParmas)
    {
        RecordList.Add(recordParmas);
    }

    public static ActionLogParmas DestroyAtLast()
    {
        var record = RecordList[^1];
        RecordList.RemoveAt(RecordList.Count - 1);
        return record;
    }

    public static void Destroy(StockItem goods)
    {
        for (var i = RecordList.Count - 1; i >= 0; i--)
        {
            var record = RecordList[i];
            if (record.cabinetIndex == goods.cabinetUnitIndex && record.itemIndex == goods.stockItemIndex)
            {
                RecordList.RemoveAt(i);
            }
        }
    }
}