using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class GamePlay_Goods : MonoBehaviour
{
    private const int botOffset = 25;
    private const int EdOffset = 20;

    public RectTransform rectTransform { get; private set; }
    public Button btnClick { get; private set; }
    public Image imaIcon;

    /// <summary>
    /// floorId
    /// </summary>
    public int layerIndex;
    /// <summary>
    /// boxId
    /// </summary>
    public int cabinetIndex;
    /// <summary>
    /// myId
    /// </summary>
    public int itemIndex;
    /// <summary>
    /// goodsType
    /// </summary>
    public int ItemType
    {
        get
        {
            if (ItemTypesArr == null || ItemTypesArr.Count == 0)
                return -1;
            return ItemTypesArr[0];
        }
        set
        {
            if (value < 0)
            {
                if (ItemTypesArr.Count > 0) ItemTypesArr.RemoveAt(0);
            }
            else
            {
                ItemTypesArr.Insert(0, value);
            }
        }
    }

    public List<int> ItemTypesArr { get; private set; } = new List<int>();

    public bool IsGroup = false;
    public int ItemCount => ItemTypesArr.Count;

    public int MaxGroupCount;

    public int startPointPos;
    public int cabinetCount;
    private float canbinetSizeWidth;

    public bool setted;

    private GamePlay_Goods goodsPrefab;

    private void Awake()
    {
        btnClick = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
        btnClick.RegistBtnCallback(CallOnClick);
    }

    private void RefreshName()
    {
        name = $"layer:{layerIndex},cIndex:{cabinetIndex},sPos:{startPointPos},count:{cabinetCount},item:{this.itemIndex}";
    }

    public void CallInit(GamePlay_Cabinet cabinet, int itemIndex)
    {
        layerIndex = cabinet.layerIndex;
        cabinetIndex = cabinet.cabinetIndex;
        startPointPos = cabinet.startPointPos;
        cabinetCount = cabinet.cabinetCount;
        this.itemIndex = itemIndex;

        canbinetSizeWidth = cabinet.rectTransform.rect.width;
        gameObject.SetActive(false);
        transform.localPosition = GetLocalPos();
        setted = false;

        IsGroup = cabinet.IsItemGroup;

        goodsPrefab = cabinet.goodsPrefab;

        if (IsGroup)
        {
            var range = GlobalSingleton.ItemGroupCountRange();
            MaxGroupCount = RandomHelp.RandomRange(range.min, range.max);
        }

        RefreshName();
    }

    // 设置商品
    public void SetItemType(int itemType)
    {
        setted = true;
        if (ItemTypesArr.Count == 0)
            ItemTypesArr.Add(itemType);
        else
            ItemTypesArr[0] = itemType;

        gameObject.SetActive(false);

        if (itemType < 0)
        {
            Debug.Log($"SetByType Fail : {itemType}");
            return;
        }

        if (GlobalAssetSingleton.Instance.TryGetSprite(itemType, out var resultSp))
        {
            gameObject.SetActive(true);
            imaIcon.sprite = resultSp;
        }

        if (ItemType == GlobalSingleton.CaItemType())
        {
            //var psComp = gameObject.GetOrAddComponent<LoopPlayParticle>();
            //psComp.enabled = true;
        }

        RefreshName();
    }

    public void ReplaceItemArrType(List<int> items)
    {
        if (items.Count != ItemTypesArr.Count)
        {
            Debug.Log($"替换items 数量对不上 {items.Count} - {ItemTypesArr.Count}");
            return;
        }
        for (int i = 0; i < items.Count; i++)
        {
            ItemTypesArr[i] = items[i];
        }

        RefreshIconImage();
    }

    public void AddItemType(int itemType)
    {
        ItemType = itemType;
        RefreshIconImage();
    }

    private void RefreshIconImage()
    {
        if (GlobalAssetSingleton.Instance.TryGetSprite(ItemType, out var resultSp))
        {
            gameObject.SetActive(true);
            imaIcon.sprite = resultSp;
        }

        if (ItemType == GlobalSingleton.CaItemType())
        {
            //var psComp = gameObject.GetOrAddComponent<LoopPlayParticle>();
            //psComp.enabled = true;
        }
        RefreshName();
    }

    public void SetItemTypes(IEnumerable<int> itemTypeArr)
    {
        ItemTypesArr.AddRange(itemTypeArr);

        gameObject.SetActive(false);

        if (GlobalAssetSingleton.Instance.TryGetSprite(ItemType, out var resultSp))
        {
            gameObject.SetActive(true);
            imaIcon.sprite = resultSp;
        }

        if (ItemType == GlobalSingleton.CaItemType())
        {
            //var psComp = gameObject.GetOrAddComponent<LoopPlayParticle>();
            //psComp.enabled = true;
        }
        RefreshName();
    }

    public void CallOnClick()
    {
        if (!GamePlay_ShelfGame.Instance.GoodsCanClickSwitch) return;
        AudioManager.AudioPlayer.PlayOneShot(SoundName.Click);
        transform.ClickScaleAni((t) =>
        {
            GamePlay_ShelfGame.Instance.OnClickedGoods(this);
        }, 0.5f);
    }

    public Vector3 GetLocalPos()
    {
        var tmpBoxWidth = (canbinetSizeWidth - EdOffset * 2) / cabinetCount;

        var x = EdOffset + itemIndex * tmpBoxWidth + tmpBoxWidth / 2;
        return new Vector3(x, botOffset, 0);
    }

    public GamePlay_Goods PopGoods()
    {
        if (ItemCount == 1)
        {
            return this;
        }
        else
        {
            var item = Instantiate(goodsPrefab, transform, false);
            item.layerIndex = layerIndex;
            item.cabinetIndex = cabinetIndex;
            item.startPointPos = startPointPos;
            item.cabinetCount = cabinetCount;
            item.itemIndex = itemIndex;
            item.canbinetSizeWidth = canbinetSizeWidth;
            transform.localPosition = GetLocalPos();
            item.IsGroup = IsGroup;
            item.goodsPrefab = goodsPrefab;

            item.SetItemType(ItemType);
            ItemType = -1;

            if (GlobalAssetSingleton.Instance.TryGetSprite(ItemType, out var resultSp))
            {
                gameObject.SetActive(true);
                imaIcon.sprite = resultSp;
            }

            if (ItemType == GlobalSingleton.CaItemType())
            {
                //var psComp = gameObject.GetOrAddComponent<LoopPlayParticle>();
                //psComp.enabled = true;
            }

            item.RefreshName();

            return item;
        }
    }
}
