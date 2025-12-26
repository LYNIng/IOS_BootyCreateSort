using UnityEngine;

public struct TaskStateResult
{
    public enum State
    {
        /// <summary>
        /// 可提交
        /// </summary>
        Completable,
        /// <summary>
        /// 激活的
        /// </summary>
        Available,
        /// <summary>
        /// 未激活
        /// </summary>
        Inactive,
        /// <summary>
        /// 已完成
        /// </summary>
        Completed,
    }

    public State ResultState;

}

public partial class GlobalSingleton
{
    public bool GetTasksState(int type, int taskID)
    {
        string key = $"TaskState_{type}_{taskID}";
        return DataManager.GetDataByBool(key, false);
    }

    public void SetTasksState(int type, int taskID)
    {
        string key = $"TaskState_{type}_{taskID}";
        Debug.Log($"Set State ->> {key}");
        DataManager.SetDataByBool(key, true);
    }

    //public int StartGameCount
    //{
    //    get
    //    {
    //        return DataManager.GetDataByInt("StartGameCount");
    //    }
    //    set
    //    {
    //        DataManager.SetDataByInt("StartGameCount", value);
    //    }
    //}

    //public int CompleteGameCount
    //{
    //    get
    //    {
    //        return DataManager.GetDataByInt("CompleteGameCount");
    //    }
    //    set
    //    {
    //        DataManager.SetDataByInt("CompleteGameCount", value);
    //    }
    //}
}

public class TasksRecordManager : Singleton<TasksRecordManager>
{
    public class IDS
    {
        //public const int Sign = 0;
        public const int PlayTime = 1;
        //public const int Elimination = 2;
    }

    private readonly static (int reward, int cnt, GameAssetType assetType)[] taskPassingTasksRewardArr =
    {
        (50,1,GameAssetType.Coin),
        (100,2,GameAssetType.Coin),
        (200,4,GameAssetType.Coin),
        (300,8,GameAssetType.Coin),
        (400,16,GameAssetType.Coin),
        (500,32,GameAssetType.Coin),
        (600,48,GameAssetType.Coin),
        (700,60,GameAssetType.Coin),
        (800,120,GameAssetType.Coin),
        (2000,480,GameAssetType.Coin)
    };

    public static (int reward, int cnt, GameAssetType assetType)[] taskEliminationRewardArr =
{
        (50,10,GameAssetType.Coin),
        (100,20,GameAssetType.Coin),
        (200,40,GameAssetType.Coin),
        (300,80,GameAssetType.Coin),
        (400,160,GameAssetType.Coin),
        (500,300,GameAssetType.Coin),
        (600,600,GameAssetType.Coin),
        (700,1200,GameAssetType.Coin),
        (800,2400,GameAssetType.Coin),
        (2000,5000,GameAssetType.Coin)
    };

    public static (int reward, int cnt, GameAssetType assetType) GetTaskRewardsByIndex(int index)
    {
        if (index < 0) return taskPassingTasksRewardArr[0];
        else if (index >= taskPassingTasksRewardArr.Length) return taskPassingTasksRewardArr[^1];

        return taskPassingTasksRewardArr[index];
    }


    public bool GetTaskState(int type, int taskID)
    {
        return GlobalSingleton.Instance.GetTasksState(type, taskID);
    }

    public void SetTaskState(int type, int taskID)
    {
        GlobalSingleton.Instance.SetTasksState(type, taskID);
    }

    //public void SetEliminationCompleteState(int taskID)
    //{
    //    SetTaskState(IDS.Elimination, taskID);
    //    RedPointManager.Instance.RefreshRedPoint(RedPointManager.IDS.RP_Eli);
    //    RedPointManager.Instance.RefreshRedPoint(RedPointManager.IDS.RP_Task);
    //}

    //public int GetEliminationCompleteState(int taskID)
    //{
    //    if (GetTaskState(IDS.Elimination, taskID))
    //    {
    //        //拿过了
    //        return 0;
    //    }
    //    else if (taskEliminationRewardArr.TryGet(taskID, out var result) && GlobalSingleton.EliminateCnt >= result.cnt)
    //    {
    //        //能拿的
    //        return 2;
    //    }
    //    else
    //    {
    //        return 1;
    //    }
    //}

    //public bool HasEliminationComplete()
    //{
    //    for (int i = 0; i < taskEliminationRewardArr.Length; ++i)
    //    {
    //        if (GetEliminationCompleteState(i) == 2)
    //            return true;
    //    }

    //    return false;
    //}

    public bool HasPlayTimeTasksCanComplete()
    {
        for (int i = 0; i < taskPassingTasksRewardArr.Length; ++i)
        {
            if (GetPlayTimeTasksCompleteState(i).ResultState == TaskStateResult.State.Completable)
                return true;
        }

        return false;
    }

    public void SetPlayTimeTasksCompleteState(int taskID)
    {
        SetTaskState(IDS.PlayTime, taskID);
        RedPointManager.Instance.RefreshRedPoint(RedPointManager.IDS.RP_Pass);
        RedPointManager.Instance.RefreshRedPoint(RedPointManager.IDS.RP_Task);
        this.SendCommand((ushort)GameEvent.RefreshTask);
    }

    public TaskStateResult GetPlayTimeTasksCompleteState(int taskID)
    {
        if (GetTaskState(IDS.PlayTime, taskID))
        {
            return new TaskStateResult { ResultState = TaskStateResult.State.Completed };
        }
        else if (taskPassingTasksRewardArr.TryGet(taskID, out var result) && GlobalSingleton.PlayTimeBySeconds >= (result.cnt * 60))
        {
            return new TaskStateResult { ResultState = TaskStateResult.State.Completable };
        }
        else
        {
            return new TaskStateResult { ResultState = TaskStateResult.State.Available };
        }

    }




}
