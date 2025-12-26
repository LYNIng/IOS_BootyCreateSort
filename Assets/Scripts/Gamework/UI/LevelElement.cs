using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LevelElement : MonoCompositeSingleton<LevelElement>, IMsgObj
{
    public override bool DontDestory => false;

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    [CmdCallback((ushort)GameEvent.RefreshLevel)]
    private void OnRefresh()
    {
        txtLevel.text = GlobalSingleton.Level.ToString();
    }
}
