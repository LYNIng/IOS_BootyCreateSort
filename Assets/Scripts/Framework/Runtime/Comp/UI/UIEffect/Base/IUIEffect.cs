// UIEffectInterface.cs
public interface IUIEffect
{
    bool IsPlaying { get; }
    void PlayEffect();
    void StopEffect();
    void RestartEffect();

    // 用于序列化播放
    float GetDuration();
    bool WaitForCompletion { get; }
}