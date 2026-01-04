using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CMP_SigninItem : MonoComposite
{
    private Action onClickCallback;

    public void Set(int idx, Action onClick)
    {
        onClickCallback = onClick;

    }
}
