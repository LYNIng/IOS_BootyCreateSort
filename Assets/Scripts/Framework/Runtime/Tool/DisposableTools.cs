using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// 暂停游戏工具 配合using使用
/// </summary>
public class PauseGame : IDisposable
{
    private static int _pauseCount = 0;
    private bool _disposed = false;

    public PauseGame()
    {
        if (Interlocked.Increment(ref _pauseCount) == 1)
        {
            this.SendCommand((ushort)FrameworksMsg.PauseGame);
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            if (Interlocked.Decrement(ref _pauseCount) == 0)
            {
                this.SendCommand((ushort)FrameworksMsg.RestartGame);
            }
        }
    }
}

/// <summary>
/// 用于计算区间代码执行时间
/// </summary>
public class StopwatchTool : IDisposable
{
    Stopwatch stopwatch;
    string blockName;
    public StopwatchTool(string blockName)
    {
        stopwatch = Stopwatch.StartNew();
        this.blockName = blockName;
    }

    public void Dispose()
    {
        UnityEngine.Debug.Log($"[代码计时] {blockName}: {stopwatch.ElapsedMilliseconds}ms ({stopwatch.ElapsedTicks} ticks)");
        stopwatch.Stop();
    }
}

public class AsyncWaitMessage : IDisposable, IMsgObj
{
    private bool waitFlag = false;
    private ushort waitMsg;
    private object sender;
    private object[] param;
    public AsyncWaitMessage(ushort msg)
    {
        waitMsg = msg;
        this.RegistCommand(msg, (obj, resultArr) =>
        {
            sender = obj;
            param = resultArr;
            waitFlag = true;
        });
    }

    public async Task<(object, object[])> AsyncWait()
    {
        while (!waitFlag) await Task.Yield();
        return (sender, param);
    }

    public void Dispose()
    {
        this.ClearRegistedCommand();
        sender = null;
        param = null;
    }
}

/// <summary>
/// 简单的异步等待计数器工具
/// </summary>
public class AsyncWaitCount : IDisposable
{
    public DateTime beganTime;
    public float OutTimeSec { get; set; }
    private int waitCount = 0;
    public int Count { get; private set; }
    /// <summary>
    /// 简单的异步等待计数器工具
    /// </summary>
    /// <param name="waitCount">等待计数器,调用PlusCount来增加计数</param>
    /// <param name="outTimeSec">超时时间,超出这个时间将会跳出AsyncWait</param>
    public AsyncWaitCount(int waitCount, float outTimeSec = 0)
    {
        this.waitCount = waitCount;
        this.OutTimeSec = outTimeSec;
        beganTime = DateTime.Now;
    }

    public async Task AsyncWait()
    {
        while (Count < waitCount)
        {
            if (OutTimeSec > 0)
            {
                var span = DateTime.Now - beganTime;
                if (span.TotalSeconds >= OutTimeSec)
                {
                    break;
                }
            }
            await Task.Yield();
        }
    }

    public void PlusCount()
    {
        Count++;
    }

    public void Dispose()
    {

    }
}


public class AsyncWaitCallback : IDisposable
{
    public Action waitCallback { get; private set; }
    private bool isCalled;
    public AsyncWaitCallback(Action waitCallback)
    {
        this.waitCallback = () =>
        {
            isCalled = true;
            waitCallback?.Invoke();
            waitCallback = null;
        };
    }

    public async Task AsyncWait()
    {
        while (!isCalled)
        {
            await Task.Yield();
        }
    }

    public void Dispose()
    {
        waitCallback = null;
    }
}
public class AsyncWaitCallback<T> : IDisposable
{
    public Action<T> waitCallback { get; private set; }
    private bool isCalled;
    public AsyncWaitCallback(Action<T> waitCallback)
    {
        this.waitCallback = (arg1) =>
        {
            isCalled = true;
            waitCallback?.Invoke(arg1);
            waitCallback = null;
        };
    }

    public async Task AsyncWait()
    {
        while (!isCalled)
        {
            await Task.Yield();
        }
    }

    public void Dispose()
    {
        waitCallback = null;
    }
}
public class AsyncWaitCallback<T1, T2> : IDisposable
{
    public Action<T1, T2> waitCallback { get; private set; }
    private bool isCalled;
    public AsyncWaitCallback(Action<T1, T2> waitCallback)
    {
        this.waitCallback = (arg1, arg2) =>
        {
            isCalled = true;
            waitCallback?.Invoke(arg1, arg2);
            waitCallback = null;
        };
    }

    public async Task AsyncWait()
    {
        while (!isCalled)
        {
            await Task.Yield();
        }
    }

    public void Dispose()
    {
        waitCallback = null;
    }
}
