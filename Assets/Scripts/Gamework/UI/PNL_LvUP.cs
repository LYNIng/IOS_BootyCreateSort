using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[UISetting(UICanvasLayer.Popup_Camera,
    backgroundMask: EBackgroundMask.Black_80F)]
public partial class PNL_LvUP : UIBase
{
    public override void OnInit()
    {
        base.OnInit();
        txtLv.text = GlobalSingleton.LevelString;
    }

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
