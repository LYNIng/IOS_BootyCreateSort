using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PNL_ClaimPopParam : UIData
{
    public bool isDouble;
    public int reward;
}

[UISetting(UICanvasLayer.Main_Camera, backgroundMask: EBackgroundMask.Black_80F, UIGroupTag: EUIGroupTag.GamePop)]
public partial class PNL_ClaimPop : UIBase<PNL_ClaimPopParam>
{
    private bool result = false;
    private bool playVideoAD = false;

    public override void OnInit()
    {
        base.OnInit();

        btnDoubleClaim.gameObject.SetActive(Data.isDouble);
        btnTxtClaim.gameObject.SetActive(Data.isDouble);
        btnClaim.gameObject.SetActive(!Data.isDouble);

        goDouble.gameObject.SetActive(Data.isDouble);
        goSingle.gameObject.SetActive(!Data.isDouble);

        txtSuperCoin.text = Data.reward.ToPriceStr();
        txtSingleSuperCoin.text = Data.reward.ToPriceStr();
        txtDoubleSuperCoin.text = (Data.reward * 2).ToPriceStr();

    }

    protected override void OnShowed()
    {
        base.OnShowed();

        btnDoubleClaim.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.Play(SoundName.UIClick);
            ShowADManager.PlayVideoAD("User_SuperCoinClaim", (code, msg) =>
            {
                if (code > 0)
                {
                    UIManager.ShowToast(msg);
                    return;
                }

                if (GlobalSingleton.InsertAdSpan == 0)
                {
                    GlobalSingleton.PlayedADByGetSuperCoinCnt = GlobalSingleton.InsertAdSpan = 4;
                }

                result = true;
                playVideoAD = true;
                Close();
            });


        });

        btnTxtClaim.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.Play(SoundName.UIClick);
            if (GlobalSingleton.EliminateSuperCoinCnt > 3 && GlobalSingleton.InsertAdSpan == 0)
            {
                GlobalSingleton.PlayedADByGetSuperCoinCnt = GlobalSingleton.InsertAdSpan = 5;
            }
            Close();
        });

        btnClaim.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.Play(SoundName.UIClick);

            Close();
        });
    }


    public override async Task<object> WaitClose()
    {
        await base.WaitClose();
        return (result ? Data.reward * 2 : Data.reward, playVideoAD);
    }
}
