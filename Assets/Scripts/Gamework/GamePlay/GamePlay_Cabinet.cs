using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 单个柜子
/// </summary>
public class GamePlay_Cabinet : MonoBehaviour
{
    private float OffsetWeight = 20f;
    private const int RowCapacity = 6;

    public GamePlay_Goods goodsPrefab;

    public Image imaBackGround;
    public GameObject goGroup;

    public RectTransform transLineRoot;
    public Image imaBar;

    public bool IsItemGroup { get; private set; } = false;

    public int layerIndex;
    public int cabinetIndex;
    public int startPointPos;
    public int cabinetCount;
    private float _floorWidth;
    public List<GamePlay_Goods> items = new();
    public RectTransform rectTransform { get; private set; }

    public TextMeshProUGUI txtNum;

    private float oneBlockWidth;
    private float barMaxWidth;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void InitData(GamePlay_ShelfLayer layer, int cabinetIndex, int startPointPos, int cabinetCount, bool isGroup)
    {
        layerIndex = layer.layerIndex;
        this.cabinetIndex = cabinetIndex;
        this.startPointPos = startPointPos;
        this.cabinetCount = cabinetCount;

        _floorWidth = layer.rectTransform.rect.width;
        transform.localPosition = GetLocalPosition();
        rectTransform.sizeDelta = GetSizeDelta();

        IsItemGroup = isGroup;
        //Debug.Log($"{floor.rid} ===>> {volume}");
        FillBox();

        if (isGroup)
        {
            if (GlobalAssetSingleton.Instance.TryGetSprite("tb-hgxz", out var Sp))
            {
                imaBackGround.sprite = Sp;
            }
            goGroup.gameObject.SetActive(true);

            var cCount = items[0].MaxGroupCount;
            oneBlockWidth = transLineRoot.sizeDelta.x / cCount;
            barMaxWidth = transLineRoot.sizeDelta.x;
            txtNum.text = cCount.ToString();
            for (int i = 0; i < cCount - 1; ++i)
            {
                if (i < transLineRoot.childCount)
                {
                    var line = transLineRoot.GetChild(i);
                    var rt = line.GetComponent<RectTransform>();
                    rt.SetAnchoredPositionX(oneBlockWidth * (i + 1));
                }
                else
                {
                    var line = transLineRoot.GetChild(0);
                    var rt = line.gameObject.SpawnNewOne(transLineRoot).GetComponent<RectTransform>();
                    rt.SetAnchoredPositionX(oneBlockWidth * (i + 1));
                }
            }
        }
        else
        {
            goGroup.gameObject.SetActive(false);
        }

    }

    #region Shelf相关操作

    public void OnGroupItemPop()
    {
        var item = items[0];
        imaBar.DOFillAmount((float)item.ItemCount / (float)item.MaxGroupCount, 0.15f).SetEase(Ease.Linear);
        txtNum.text = item.ItemCount.ToString();
        txtNum.transform.parent.ClickScaleAni((btn) => { });
    }

    // 填充货柜
    private void FillBox()
    {
        for (var i = 0; i < cabinetCount; i++)
        {
            var item = Instantiate(goodsPrefab, transform, false);
            item.CallInit(this, i);
            items.Add(item);
            item.transform.SetSiblingIndex(1);
        }
    }

    // 判断货柜是否为空
    public bool IsNull()
    {
        foreach (var item in items)
        {
            if (item.ItemType >= 0) return false;
        }

        return true;
    }

    #endregion

    #region Item相关操作

    // 添加商品
    public void AddGoods(ActionLogParmas recordParmas)
    {
        foreach (var item in items)
        {
            if (item.itemIndex == recordParmas.itemIndex)
            {
                if (recordParmas.isGroup)
                {
                    item.AddItemType(recordParmas.ItemType);
                    OnGroupItemPop();
                }
                else
                {
                    item.SetItemType(recordParmas.ItemType);
                }
            }
        }
    }

    // 移除商品
    public void RemoveGoods(GamePlay_Goods goods)
    {
        for (var i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].itemIndex != goods.itemIndex) continue;
            items.RemoveAt(i);

            // 创建一个新的item填充位置
            var newItem = Instantiate(goodsPrefab, transform, false);
            newItem.CallInit(this, goods.itemIndex);
            items.Add(newItem);
            break;
        }
    }

    #endregion


    public Vector3 GetLocalPosition()
    {
        var cellWidth = _floorWidth / RowCapacity;
        return new Vector3(startPointPos * cellWidth, 0, 0);
    }

    private Vector2 GetSizeDelta()
    {
        var shelfHeight = rectTransform.rect.height;
        var cellWidth = _floorWidth / RowCapacity;
        return new Vector2(cellWidth * cabinetCount, shelfHeight);
    }
}
