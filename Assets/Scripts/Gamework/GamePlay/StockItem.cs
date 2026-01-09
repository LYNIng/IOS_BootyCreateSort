using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class StockItem : MonoBehaviour
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
    public int cabinetUnitIndex;
    /// <summary>
    /// myId
    /// </summary>
    public int stockItemIndex;
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

    private StockItem stockItemPrefab;

    private void Awake()
    {
        btnClick = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
        btnClick.RegistBtnCallback(CallOnClick);
    }

    private void RefreshName()
    {
        name = $"layer:{layerIndex},cIndex:{cabinetUnitIndex},sPos:{startPointPos},count:{cabinetCount},item:{this.stockItemIndex}";
    }

    public void CallInit(CabinetUnit cabinet, int stockItemIndex)
    {
        layerIndex = cabinet.oneLayerIndex;
        cabinetUnitIndex = cabinet.cabinetUnitIndex;
        startPointPos = cabinet.beginPos;
        cabinetCount = cabinet.cabinetUnitCount;
        this.stockItemIndex = stockItemIndex;

        canbinetSizeWidth = cabinet.rectTransform.rect.width;
        gameObject.SetActive(false);
        transform.localPosition = GetLocalPos();
        setted = false;

        IsGroup = cabinet.IsItemGroup;

        stockItemPrefab = cabinet.stockItemPrefab;

        if (IsGroup)
        {
            var range = GlobalSingleton.ItemGroupCountRange();
            MaxGroupCount = RandomHelp.RandomRange(range.min, range.max);
        }

        RefreshName();
    }

    // 设置商品
    public void SetIconType(int itemType)
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
            PlaySuperCoinIconEffect();
        }
        else
        {
            ResetIconEffect();
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

    public void AddIconType(int itemType)
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
            PlaySuperCoinIconEffect();
        }
        else
        {
            ResetIconEffect();
        }
        RefreshName();
    }

    public void SetIconTypes(IEnumerable<int> itemTypeArr)
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
            PlaySuperCoinIconEffect();
        }
        RefreshName();
    }

    public void CallOnClick()
    {
        if (!MiniGame.Instance.ItemCanClick) return;
        AudioManager.AudioPlayer.PlayOneShot(SoundName.Click);
        transform.ClickScaleAni((t) =>
        {
            MiniGame.Instance.OnClickedGoods(this);
        }, 0.5f);
    }

    public Vector3 GetLocalPos()
    {
        var tmpBoxWidth = (canbinetSizeWidth - EdOffset * 2) / cabinetCount;

        var x = EdOffset + stockItemIndex * tmpBoxWidth + tmpBoxWidth / 2;
        return new Vector3(x, botOffset, 0);
    }

    public StockItem PopItem(int goodType)
    {
        if (ItemCount == 1)
        {
            return this;
        }
        else
        {
            var item = Instantiate(stockItemPrefab, transform.parent, false);
            item.layerIndex = layerIndex;
            item.cabinetUnitIndex = cabinetUnitIndex;
            item.startPointPos = startPointPos;
            item.cabinetCount = cabinetCount;
            item.stockItemIndex = stockItemIndex;
            item.canbinetSizeWidth = canbinetSizeWidth;
            transform.localPosition = GetLocalPos();
            item.IsGroup = IsGroup;
            item.stockItemPrefab = stockItemPrefab;

            item.SetIconType(ItemType);
            if (goodType == -1)
            {
                ItemType = -1;
            }
            else
            {
                for (int i = 0; i < ItemTypesArr.Count; ++i)
                {
                    var type = ItemTypesArr[i];
                    if (goodType == type)
                    {
                        ItemTypesArr.RemoveAt(i);
                        break;
                    }
                }
            }

            if (GlobalAssetSingleton.Instance.TryGetSprite(ItemType, out var resultSp))
            {
                gameObject.SetActive(true);
                imaIcon.sprite = resultSp;

            }

            if (ItemType == GlobalSingleton.CaItemType())
            {
                PlaySuperCoinIconEffect();
            }
            else
                ResetIconEffect();

            item.RefreshName();

            return item;
        }
    }
    private readonly static Color shineColor = "E0D838".ToColor();
    private void PlaySuperCoinIconEffect()
    {
        imaIcon.material = GlobalAssetSingleton.SuperCoinMate;
        var anim = imaIcon.GetOrAddComponent<UIShineAnimator>();
        anim.shineColor = shineColor;
        anim.effectDuration = 2f;
        anim.SetRandomInterval(true);
        anim.SetRandomStartDelay(true);
        anim.enabled = true;
    }

    private void ResetIconEffect()
    {
        imaIcon.material = imaIcon.defaultMaterial;
        if (imaIcon.TryGetComponent<UIShineAnimator>(out var comp))
        {
            comp.enabled = false;
        }
    }
}
