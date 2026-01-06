// ReSharper disable CheckNamespace

public enum GameEvent : ushort
{
    EnterNextLevel = FrameworksMsg.Max + 1,

    RefreshCoin,
    RefreshSuperCoin,
    RefreshLevel,

    RefreshTask,

    OnEliminate,//消除时
    EliminateUIRefresh,//
    EliminateUIStart,//
    EliminateEnd,

    RefreshProgressBar,

    GamePlay_Begin,//游戏启动
    GamePlay_BackHome,//返回主页面
    GamePlay_LevelComplete, //过关

    UnlockNewSpace,//解锁额外位
    ResetNewSpace,//重置额外位

    RefreshADFREE,

    ShowBackHomeButtom,
    HideBackHomeButtom,

    GameFail,
    GameSuccess,


}