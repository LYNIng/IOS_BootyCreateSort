using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PNL_GuideParam : UIData
{
    public int guideIDX;
    public Vector3 pos;
    public StockItem goods;
}

[UISetting(UICanvasLayer.Overlay_Camera, backgroundMask: EBackgroundMask.Transparency, UIGroupTag: EUIGroupTag.GamePop)]
public partial class PNL_Guide : UIBase<PNL_GuideParam>
{
    protected override async Task Show_Internal()
    {
        await Task.CompletedTask;
    }
    protected override async Task Hide_Internal()
    {
        await Task.CompletedTask;
    }

    public override void OnInit()
    {
        base.OnInit();

        _ = ShowGuide(Data.guideIDX);
    }

    protected override async Task OnReplaceData()
    {
        await ShowGuide(Data.guideIDX);
    }
    int step1Cnt = 0;
    Transform guideTrans = null;
    public async Task ShowGuide(int index)
    {
        if (guideTrans != null)
        {
            guideTrans.gameObject.SetActive(false);
            guideTrans = null;
        }

        guideTrans = transform.Find($"Guide{index}");
        guideTrans.gameObject.SetActive(true);
        var fingerTrans = transform.Find("imaFinge").GetComponent<RectTransform>();

        if (index == 0)
        {
            var click = guideTrans.Find("btnPlay").GetComponent<Button>();
            click.RegistBtnCallback(() =>
            {
                AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
                Close();
                this.SendCommand((ushort)GameEvent.GamePlay_Begin);
            });
            InitFinger(fingerTrans, click.transform.position + new Vector3(1, 0, 0));
        }
        else if (index == 1)
        {
            var imaIcon = guideTrans.Find("imaIcon");
            //var canvas = UIManager.GetCanvasLayerTransform(CanvasLayer).GetComponent<Canvas>();
            //var rtCanvas = canvas.GetComponent<RectTransform>();
            //var dataUIPos = RectTransformUtil.WorldToCanvasPosition(canvas, Data.pos);

            var frame = guideTrans.Find("Frame").GetComponent<RectTransform>();

            if (GlobalAssetSingleton.Instance.TryGetSprite(Data.goods.ItemType, out Sprite result))
            {
                imaIcon.GetComponent<Image>().sprite = result;
                if (step1Cnt == 1)
                {
                    GuideItem1.sprite = result;
                    GuideItem1.gameObject.SetActive(true);
                }
                else if (step1Cnt == 2)
                {
                    GuideItem2.sprite = result;
                    GuideItem2.gameObject.SetActive(true);
                }
                step1Cnt++;
            }

            frame.transform.position = new Vector3(frame.transform.position.x, Data.goods.transform.position.y + 10f, frame.transform.position.z);

            imaIcon.transform.position = Data.goods.transform.position;
            var btn = imaIcon.GetComponent<Button>();
            btn.ClearAllBtnCallback();
            btn.RegistBtnCallback(() =>
            {
                AudioManager.AudioPlayer.Play(SoundName.UIClick);
                MiniGame.Instance.OnClickedGoods(Data.goods);
                ExcuteInteraction();
            });
            InitFinger(fingerTrans, Data.goods.transform.position + new Vector3(1, 0, 0));
        }
        else if (index == 2)
        {
            guideTrans.Find("btnClick").GetComponent<Button>().RegistBtnCallback(() =>
            {
                AudioManager.AudioPlayer.Play(SoundName.UIClick);
                ExcuteInteraction();
            });
            InitFinger(fingerTrans, AllocationSlot.Instance.unlockSeat.transform.position + new Vector3(-0.6f, 0), -1);
        }
        else if (index == 3)
        {

            AssetLoader<Sprite> loader = new AssetLoader<Sprite>("Sprites/MainGamePlay/tb-mj.png");
            await loader.AsyncLoad();

            imaGuide3Icon.sprite = loader.Asset;
            //var newOne = loader.Asset.SpawnNewOne(guideTrans);
            //newOne.transform.localScale = Vector3.one;
            //newOne.transform.position = SuperCoinElement.Instance.transform.position;
            TopSuperCoinPart.transform.position = SuperCoinElement.Instance.transform.position;
            TopSuperCoinPart.Find("txtValue").GetComponent<TextMeshProUGUI>().text = GlobalSingleton.SuperCoin.ToPriceStr();
            btnCa.RegistBtnCallback(() =>
            {
                AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
                UIManager.OpenUI<PNL_TX>();
                ExcuteInteraction();
            });
            InitFinger(fingerTrans, btnCa.transform.position + new Vector3(0, -1, 0));
        }
    }
    Tween fingerTween = null;
    private void InitFinger(RectTransform rtFinger, Vector3 fingerPos, int dir = 1)
    {
        rtFinger.DOKill();
        var pos = fingerPos;
        rtFinger.transform.position = pos;
        rtFinger.transform.localScale = new Vector3(1f * dir, 1f, 1f);
        fingerTween = rtFinger.transform.DOMove(pos + new Vector3(0.3f * dir, -0.3f), 0.6f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutCubic);
    }

    protected override void OnShowed()
    {
        base.OnShowed();


    }
}
