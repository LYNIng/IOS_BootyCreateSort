using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[PreloadAssets(
    "Sprites/Signin/tb-ch.png", "Sprites/Signin/tb-coin.png", "Sprites/Signin/tb-wash.png", "Sprites/Signin/tb-xxyz.png")]
[UISetting(UICanvasLayer.Popup_Camera, backgroundMask: EBackgroundMask.Black_80F | EBackgroundMask.CloseUIOnCancelClick | EBackgroundMask.CloseUIOnBackGroundClick, UIGroupTag: EUIGroupTag.GamePop)]
public partial class PNL_Signin : UIBase
{
    public CMP_SigninItem[] signinItems;
    public override void OnInit()
    {
        base.OnInit();

        for (int i = 0; i < signinItems.Length; i++)
        {
            var item = signinItems[i];
            item.Set(i);
        }
    }
}
