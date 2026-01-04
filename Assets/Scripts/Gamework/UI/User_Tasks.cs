using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[PreloadAssets("Sprites/Tasks/tb-mj.png")]
[UISetting(UICanvasLayer.Popup_Camera,
    backgroundMask: EBackgroundMask.Black_80F | EBackgroundMask.CloseUIOnBackGroundClick | EBackgroundMask.CloseUIOnCancelClick)]
public partial class User_Tasks : UIBase, IMsgObj
{

    protected override async Task Show_Internal()
    {
        await PlayShow_ScaleLessenAndFadeIn();
    }

    protected override async Task Hide_Internal()
    {
        await PlayHide_ScaleMagnifyFadeOut();
    }

    public override void OnInit()
    {
        base.OnInit();

        if (UIManager.TryGetUI(out HUB_Topbar bar))
        {
            bar.ShowBackTopBar(() =>
            {

                Close();
            });
        }

        RefreshTask();
    }

    protected override void OnHideBefore()
    {
        if (UIManager.TryGetUI(out HUB_Topbar bar))
        {
            bar.ShowNormalTopBar();
        }
    }
    [CmdCallback((ushort)GameEvent.RefreshTask)]
    private void RefreshTask()
    {
        int cnt = rtTaskContent.childCount;

        for (int i = 0; i < cnt; i++)
        {
            var rt = rtTaskContent.GetChild(i);
            RefreshItem(i, rt);
        }
    }



    private void RefreshItem(int index, Transform trans)
    {
        var result = TasksRecordManager.Instance.GetPlayTimeTasksCompleteState(index);
        var reward = TasksRecordManager.GetTaskRewardsByIndex(index);
        var transEnable = trans.Find("Enable");
        var transDisable = trans.Find("Disable");
        var redPoint = trans.Find("imaRedPoint");

        var btn = trans.GetComponent<Button>();
        btn.ClearAllBtnCallback();

        if (result.ResultState == TaskStateResult.State.Available)
        {

            transEnable.gameObject.SetActive(true);
            transDisable.gameObject.SetActive(false);

            var f = Mathf.Clamp01((float)GlobalSingleton.PlayTimeBySeconds / ((float)reward.cnt * 60f));

            transEnable.Find("Bar/imaBar").GetComponent<Image>().fillAmount = f;
            if (reward.assetType == GameAssetType.SuperCoin && TryGetPreloadAsset("tb-mj", out Sprite mSprite))
            {
                var imaIcon = transEnable.Find("imaIcon").GetComponent<Image>();
                imaIcon.sprite = mSprite;
            }
            transEnable.Find("txtValue").GetComponent<TextMeshProUGUI>().text = reward.reward.ToNumText(reward.assetType);
            transEnable.Find("txtDec").GetComponent<TextMeshProUGUI>().text = 1002.ToMulStrFormat(reward.cnt);
            redPoint.gameObject.SetActive(false);
        }
        else if (result.ResultState == TaskStateResult.State.Completable)
        {
            redPoint.gameObject.SetActive(true);
            transEnable.gameObject.SetActive(true);
            transDisable.gameObject.SetActive(false);

            transEnable.Find("Bar/imaBar").GetComponent<Image>().fillAmount = 1;
            var imaIcon = transEnable.Find("imaIcon").GetComponent<Image>();
            if (reward.assetType == GameAssetType.SuperCoin && TryGetPreloadAsset("tb-mj", out Sprite mSprite))
            {

                imaIcon.sprite = mSprite;
            }

            transEnable.Find("txtDec").GetComponent<TextMeshProUGUI>().text = 1002.ToMulStrFormat(reward.cnt);
            transEnable.Find("txtValue").GetComponent<TextMeshProUGUI>().text = reward.reward.ToNumText(reward.assetType);
            btn.RegistBtnCallback(async () =>
            {
                AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
                TasksRecordManager.Instance.SetPlayTimeTasksCompleteState(index);

                await GlobalSingleton.AsyncGetReward(reward.assetType, reward.reward, imaIcon.transform.position, RandomHelp.RandomRange(8, 16));
            });
        }
        else if (result.ResultState == TaskStateResult.State.Completed)
        {
            redPoint.gameObject.SetActive(false);
            transEnable.gameObject.SetActive(false);
            transDisable.gameObject.SetActive(true);

            transDisable.Find("Bar/imaBar").GetComponent<Image>().fillAmount = 1;
            transDisable.Find("txtValue").GetComponent<TextMeshProUGUI>().text = reward.reward.ToNumText(reward.assetType);
            if (reward.assetType == GameAssetType.SuperCoin && TryGetPreloadAsset("tb-mj", out Sprite mSprite))
            {
                var imaIcon = transDisable.Find("imaIcon").GetComponent<Image>();
                imaIcon.sprite = mSprite;
            }

            transDisable.Find("txtDec").GetComponent<TextMeshProUGUI>().text = 1002.ToMulStrFormat(reward.cnt);
        }

    }
}
