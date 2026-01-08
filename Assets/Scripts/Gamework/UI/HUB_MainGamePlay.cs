using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[UISetting(UICanvasLayer.Main_Camera, UIGroupTag: EUIGroupTag.GamePop)]
public partial class HUB_MainGamePlay : UIBase, IMsgObj
{
    public FillAmountToPosition fillAmount;

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
        RefreshADSkip();
    }

    [CmdCallback((ushort)GameEvent.RefreshADFREE)]
    private void RefreshADSkip()
    {
        btnADFREE.gameObject.SetActive(!ShowADManager.IsSkipAD);
    }


    protected override void OnShowed()
    {
        base.OnShowed();

        btnRemove.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            if (GlobalSingleton.RemoveTool > 0)
            {
                MiniGame.Instance.OnUseRemoveProp();
            }
            else
            {
                UIManager.OpenUI<PNL_ShellProp>(new PNL_ShellPropParam { gameAssetType = GameAssetType.RemoveTool });
            }
        });

        btnBack.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            if (GlobalSingleton.BackTool > 0)
            {
                MiniGame.Instance.OnUseBackProp();
            }
            else
            {
                UIManager.OpenUI<PNL_ShellProp>(new PNL_ShellPropParam { gameAssetType = GameAssetType.BackTool });
            }
        });

        btnRefresh.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            if (GlobalSingleton.RefreshTool > 0)
            {
                MiniGame.Instance.OnUseShuffleProp();
            }
            else
            {
                UIManager.OpenUI<PNL_ShellProp>(new PNL_ShellPropParam { gameAssetType = GameAssetType.RefreshTool });
            }
        });

        btnADFREE.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            UIManager.OpenUI<PNL_RemoveADs>();
        });
        //btnTask.RegistBtnCallback(() =>
        //{
        //    AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
        //    UIManager.OpenUI<User_Tasks>();
        //});
    }

    [CmdCallback((ushort)GameEvent.RefreshProgressBar)]
    private void OnRefresh()
    {
        if (GlobalSingleton.MaxCount == 0)
        {
            imaBar.fillAmount = 0;
            fillAmount.RefreshPos();
            return;
        }
        float fileP = GetProgress();
        imaBar.DOFillAmount(fileP, 0.2f).SetEase(Ease.Linear).OnUpdate(() =>
        {
            fillAmount.RefreshPos();
        });

        Sequence seq = DOTween.Sequence();
        seq.Append(imaBar.DOColor(Color.white * 0.9f, 0.05f).SetEase(Ease.Linear));
        seq.Append(imaBar.DOColor(Color.white * 0.95f, 0.05f).SetEase(Ease.Linear));
        seq.Append(imaBar.DOColor(Color.white * 0.9f, 0.05f).SetEase(Ease.Linear));
        seq.Append(imaBar.DOColor(Color.white * 1, 0.05f).SetEase(Ease.Linear));
    }

    float cntPro = 1f;
    float cnt = 0;
    private float GetProgress()
    {
        if (GlobalSingleton.Level >= 4)
        {
            float tmp = 0f;
            if (cnt < 100)
            {
                tmp = 0.2f / 100f;
            }
            else if (cnt < 200)
            {
                tmp = 0.15f / 100f;
            }
            else if (cnt < 300)
            {
                tmp = 0.1f / 100f;
            }
            else if (cnt < 400)
            {
                tmp = 0.05f / 100f;
            }
            cntPro -= tmp;
            cntPro = Mathf.Clamp(cntPro, 0.0001f, 1f);
            cnt++;
            return cntPro;
        }
        float fileP = (GlobalSingleton.MaxCount - GlobalSingleton.CurCount) / (float)GlobalSingleton.MaxCount;
        return fileP;
    }
}
