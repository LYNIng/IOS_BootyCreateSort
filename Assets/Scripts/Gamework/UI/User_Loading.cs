using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[UISetting(UICanvasLayer.System_Camera, backgroundMask: EBackgroundMask.Transparency)]
public class User_Loading : UIBase
{
    protected override async Task Show_Internal()
    {
        await PlayShow_ScaleLessenAndFadeIn();
    }

    protected override async Task Hide_Internal()
    {
        await PlayHide_ScaleMagnifyFadeOut();
    }


}
