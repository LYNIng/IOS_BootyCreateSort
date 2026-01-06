using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[UISetting(UICanvasLayer.Overlay_Camera, UIGroupTag: EUIGroupTag.HomePage)]
public partial class PNL_MainPage_Top : UIBase
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
