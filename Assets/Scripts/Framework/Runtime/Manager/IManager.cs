using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public interface IManager
{

}

public interface IManagerInit : IManager
{
    Task<bool> AsyncInit();
}

public interface IManagerUpdate : IManager
{
    bool InvokeUpdateTick { get; }

    void UpdateTick(float dt);
}

public class Managers : MonoSingleton<Managers>
{
    public override bool DontDestory => true;

    private Dictionary<Type, IManager> managerDict = new Dictionary<Type, IManager>();

    public bool Inited { get; private set; }

    public async Task<bool> AsyncInit()
    {
        if (Inited)
        {
            await Task.CompletedTask;
            return true;
        }

        MessageDispatch.BindMessage(this);

        //初始化所有管理器
        await InitManager(AssetsManager.Instance as IManager);
        await InitManager(LanguageTextManager.Instance as IManager);
        await InitManager(UIManager.Instance as IManager);
        await InitManager(GlobalAssetSingleton.Instance as IManager);
        await InitManager(AudioManager.Instance as IManager);
        return true;
    }

    //public async Task AsyncCreateNewGame(CreateResult result)
    //{
    //    foreach (var manager in managerDict.Values)
    //    {
    //        if (manager is IManagerSaveObj savebj)
    //        {
    //            result.ADDTotalProgress(savebj.GetProgressCount());
    //        }
    //    }

    //    foreach (var manager in managerDict.Values)
    //    {
    //        if (manager is IManagerSaveObj savebj)
    //        {
    //            await savebj.AsyncCreateNew(result);
    //        }
    //    }
    //}

    //public async Task AsyncSaveGame(SaveResult result)
    //{
    //    foreach (var manager in managerDict.Values)
    //    {
    //        if (manager is IManagerSaveObj savebj)
    //        {
    //            result.ADDTotalProgress(savebj.GetProgressCount());
    //        }
    //    }

    //    foreach (var manager in managerDict.Values)
    //    {
    //        if (manager is IManagerSaveObj savebj)
    //        {
    //            await savebj.AsyncSave(result);
    //        }
    //    }
    //}

    //public async Task AsyncLoadGame(LoadResult result)
    //{
    //    foreach (var manager in managerDict.Values)
    //    {
    //        if (manager is IManagerSaveObj savebj)
    //        {
    //            result.ADDTotalProgress(savebj.GetProgressCount());
    //        }
    //    }

    //    foreach (var manager in managerDict.Values)
    //    {
    //        if (manager is IManagerSaveObj savebj)
    //        {
    //            await savebj.AsyncLoad(result);
    //        }
    //    }
    //}

    private async Task<bool> InitManager(IManager manager)
    {
        if (managerDict.TryAdd(manager.GetType(), manager))
        {
            if (manager is IManagerInit initManager)
            {
                MessageDispatch.CallMessageCommand((ushort)FrameworksMsg.Log, param: $"{manager.GetType()} 开始初始化");
                await initManager.AsyncInit();
            }

            MessageDispatch.BindMessage(manager);
            return true;
        }

        return false;
    }

    public bool TryGetManager(Type managerType, out IManager resultManager)
    {
        if (managerDict.TryGetValue(managerType, out resultManager))
        {
            return true;
        }
        resultManager = null;
        return false;
    }

    public bool TryGetManager<T>(out T resultManager) where T : IManager
    {
        if (managerDict.TryGetValue(typeof(T), out var result) && result is T tmp)
        {
            resultManager = tmp;
            return true;
        }
        resultManager = default(T);
        return false;
    }

}


