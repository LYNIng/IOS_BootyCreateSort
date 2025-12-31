using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TopLevelPart : MonoCompositeSingleton<TopLevelPart>, IMsgObj
{
    public override bool DontDestory => false;

    protected override void OnAwake()
    {
        base.OnAwake();
        txtLevel.text = $"Level:{GlobalSingleton.LevelString}";
    }

    [CmdCallback((ushort)GameEvent.RefreshLevel)]
    private void OnRefresh()
    {
        txtLevel.text = $"Level:{GlobalSingleton.LevelString}";
    }
}
