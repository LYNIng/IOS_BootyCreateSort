using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[UISetting(UICanvasLayer.Overlay_Camera, backgroundMask: EBackgroundMask.Black_80F | EBackgroundMask.CloseUIOnCancelClick | EBackgroundMask.CloseUIOnBackGroundClick, UIGroupTag: EUIGroupTag.GamePop)]
public partial class User_Contact : UIBase
{
    protected override void OnShowed()
    {
        base.OnShowed();

        btnClose.RegistBtnCallback(() =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);
            Close();
        });
        btnClick.RegistBtnCallback(async () =>
        {
            AudioManager.AudioPlayer.PlayOneShot(SoundName.UIClick);

            if (!CommonUtil.IsValidEmailAddressFunc(inputAddress.text))
            {
                UIManager.ShowToast(1011.ToMultiLanguageText());
                return;
            }

            imaBlackMask.gameObject.SetActive(true);

            await Task.Delay(Random.Range(1000, 3000));

            UIManager.OpenUI<User_Thanks>();
            imaBlackMask.gameObject.SetActive(false);
            Close();
        });
    }
}
