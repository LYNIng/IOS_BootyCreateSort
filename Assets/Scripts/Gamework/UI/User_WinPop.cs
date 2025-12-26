using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[UISetting(UICanvasLayer.Popup_Camera,
    backgroundMask: EBackgroundMask.Black_80F)]
public class User_WinPop : UIBase
{
    protected override async Task Show_Internal()
    {
        await base.Show_Internal();

        await Task.Delay(1000);
    }

    protected override void OnShowed()
    {
        base.OnShowed();
    }
}
