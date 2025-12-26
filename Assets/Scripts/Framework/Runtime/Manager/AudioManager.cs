using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundName
{
    Bgm = 0,
    UIClick,

    Fail,
    Win,
    RewardPop,
    BoxElimination,
    Elimination,
    HitBar,

    HitCoin,

    SwitchScenes,

    Click,

    CA,

}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(0.1f, 3f)]
    public float pitch = 1f;

    public bool loop = false;
    public bool playOnAwake = false;

    [HideInInspector]
    public AudioSource source;

    public AudioMixerGroup mixerGroup;

    public bool isLoading;
}

public static class AudioConfigs
{
    //预加载的音效
    public static readonly (SoundName soundTag, float volume, string path)[] Configs = new (SoundName soundTag, float volume, string path)[]
    {
        //(SoundName.UIClick, 1f,"Audios/UIClick.mp3"),
        //(SoundName.Bgm, 0.5f,"Audios/BGM.mp3"),

        //(SoundName.Fail,1f, "Audios/Fail.mp3"),
        //(SoundName.Win,1f, "Audios/Win.mp3"),
        //(SoundName.RewardPop, 1f,"Audios/RewardPop.mp3"),
        //(SoundName.BoxElimination, 1f,"Audios/BoxElimination.mp3"),
        //(SoundName.Elimination,1f, "Audios/Elimination.mp3"),
        //(SoundName.SwitchScenes,1f, "Audios/SwitchScenes.mp3"),
        //(SoundName.CA,1f, "Audios/CA.mp3"),
    };

    //public static readonly (SoundName soundTag, float timerLimit, float volume, string[] paths)[] stepAudioConfigs = new (SoundName soundTag, float timerLimit, float volume, string[] paths)[]
    //{
    //    (SoundName.Click,4f, 1f,new string[]{ "Audios/Click1.mp3","Audios/Click2.mp3","Audios/Click3.mp3","Audios/Click4.mp3",})
    //};

}

/// <summary>
/// AudioCtrlMgr
/// </summary>
public class AudioManager : MonoSingleton<AudioManager>, IManager, IManagerInit, IMsgObj
{
    public const int AudioChannel = 10;
    #region 消息响应
    [CmdCallback((ushort)FrameworksMsg.PlayAudio, AudioChannel)]
    private void _OnPlayAudio(object arg1)
    {
        if (arg1 == null) return;
        if (arg1 is string str) { AudioPlayer.Play(str); }
        else if (arg1 is SoundName soundName) { AudioPlayer.Play(soundName); }
    }
    [CmdCallback((ushort)FrameworksMsg.PlayMusic, AudioChannel)]
    private void _OnPlayMusic(object arg1)
    {
        if (arg1 == null) return;
        if (arg1 is string str) { AudioPlayer.Play(str); }
        else if (arg1 is SoundName soundName) { AudioPlayer.PlayMusic(soundName); }
    }
    [CmdCallback((ushort)FrameworksMsg.PlayAudioOneShot, AudioChannel)]
    private void _OnPlayAudioOneShot(object arg1)
    {
        if (arg1 == null) return;
        if (arg1 is string str) { AudioPlayer.PlayOneShot(str); }
        else if (arg1 is SoundName soundName) { AudioPlayer.PlayOneShot(soundName); }
    }
    [CmdCallback((ushort)FrameworksMsg.StopAudio, AudioChannel)]
    private void _OnStopAudio(object arg1)
    {
        if (arg1 == null) return;
        if (arg1 is string str) { AudioPlayer.Stop(str); }
        else if (arg1 is SoundName soundName) { AudioPlayer.Stop(soundName); }
    }
    [CmdCallback((ushort)FrameworksMsg.SetMasterVolume, AudioChannel)]
    private void _OnSetMasterVolume(float volume) => AudioPlayer.SetMasterVolume(volume);
    [CmdCallback((ushort)FrameworksMsg.SetMusicVolume, AudioChannel)]
    private void _OnSetMusicVolume(float volume) => AudioPlayer.SetMusicVolume(volume);
    [CmdCallback((ushort)FrameworksMsg.SetSFXVolume, AudioChannel)]
    private void _OnSetSFXVolume(float volume) => AudioPlayer.SetSFXVolume(volume);

    #endregion
    public static class AudioPlayer
    {
        public static void Play(string soundName) => Instance?.Play(soundName);
        public static void Play(SoundName soundName) { if (AudioPathHelperPendant.Instance.TryGetPathByTag(soundName, out var sName)) Play(sName); }
        public static void PlayMusic(string musicName) => Instance?.PlayMusic(musicName);
        public static void PlayMusic(SoundName musicName) { if (AudioPathHelperPendant.Instance.TryGetPathByTag(musicName, out var sName)) PlayMusic(sName); }
        public static void Stop(string soundName) => Instance?.Stop(soundName);
        public static void Stop(SoundName soundName) { if (AudioPathHelperPendant.Instance.TryGetPathByTag(soundName, out var sName)) Stop(sName); }
        public static void PlayOneShot(string soundName, float volumeScale = 1f) => Instance?.PlayOneShot(soundName, volumeScale);
        public static void PlayOneShot(SoundName soundName, float volumeScale = 1f) { if (AudioPathHelperPendant.Instance.TryGetPathByTag(soundName, out var sName)) PlayOneShot(sName, volumeScale); }
        public static void SetMasterVolume(float volume) => Instance?.SetMasterVolume(volume);
        public static void SetMusicVolume(float volume) => Instance?.SetMusicVolume(volume);
        public static void SetSFXVolume(float volume) => Instance?.SetSFXVolume(volume);

        public static void SetMusicMuted(bool muted) => Instance?.SetMusicMuted(muted);
        public static void SetSFXMuted(bool muted) => Instance?.SetSFXMuted(muted);
        public static bool GetMusicMuted() => Instance != null && Instance.IsMusicMuted();
        public static bool GetSFXMuted() => Instance != null && Instance.IsSFXMuted();
    }
    public override bool DontDestory => true;

    [Header("音频设置")]
    [SerializeField] private List<Sound> sounds;
    [SerializeField] private AudioMixerGroup masterMixerGroup;

    [Header("音量控制")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    private bool isMusicMuted = false;
    private bool isSFXMuted = false;
    private float prevMusicVolume = 1f;
    private float prevSFXVolume = 1f;

    private Dictionary<string, Sound> soundDictionary;
    private string currentMusic;

    private AudioListener audioListener;

    // 音频混合组枚举
    //public enum AudioType
    //{
    //    Master,
    //    Music,
    //    SFX
    //}

    GameObject SpawnSoundGO(Sound sound)
    {

        GameObject soundObject = new GameObject($"Sound:{sound.name}");
        soundObject.transform.SetParent(transform);

        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = sound.clip;
        audioSource.volume = sound.volume;
        audioSource.pitch = sound.pitch;
        audioSource.loop = sound.loop;
        audioSource.playOnAwake = sound.playOnAwake;

        // 设置音频混合组
        if (sound.mixerGroup != null)
        {
            audioSource.outputAudioMixerGroup = sound.mixerGroup;
        }
        else if (masterMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup = masterMixerGroup;
        }

        if (sound.name == currentMusic)
        {
            audioSource.mute = isMusicMuted;
        }
        else
        {
            audioSource.mute = isSFXMuted;
        }

        sound.source = audioSource;
        soundDictionary[sound.name] = sound;

        if (sound.playOnAwake)
        {
            audioSource.Play();
        }

        return soundObject;
    }

    void InitializeAudioManager()
    {
        audioListener = gameObject.GetOrAddComponent<AudioListener>();

        soundDictionary = new Dictionary<string, Sound>();

        UpdateAllVolumes();
    }
    private bool inited = false;
    public async Task<bool> AsyncInit()
    {
        if (inited) return true;
        inited = true;

        InitializeAudioManager();
        LoadAudioSettings();
        await Task.CompletedTask;
        return true;
    }

    // 在AudioManager类中添加以下方法

    #region 音频播放控制

    /// <summary>
    /// 播放音效
    /// </summary>
    public void Play(string soundName, bool isLoop = false)
    {
        _ = GetSoundObj(soundName, (sound) =>
        {
            sound.source.loop = isLoop;
            sound.source.Play();
        });
    }

    /// <summary>
    /// 停止播放音效
    /// </summary>
    public void Stop(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            sound.source?.Stop();
        }
    }

    /// <summary>
    /// 暂停音效
    /// </summary>
    public void Pause(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            sound.source.Pause();
        }
    }

    /// <summary>
    /// 恢复播放音效
    /// </summary>
    public void UnPause(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            sound.source.UnPause();
        }
    }

    /// <summary>
    /// 播放音乐（会自动停止当前音乐）
    /// </summary>
    public void PlayMusic(string musicName)
    {
        if (currentMusic == musicName && IsPlaying(musicName))
            return;

        // 停止当前音乐
        if (!string.IsNullOrEmpty(currentMusic))
        {
            Stop(currentMusic);
        }

        currentMusic = musicName;
        Play(musicName, true);

    }

    /// <summary>
    /// 停止当前音乐
    /// </summary>
    public void StopMusic()
    {
        if (!string.IsNullOrEmpty(currentMusic))
        {
            Stop(currentMusic);
            currentMusic = null;
        }
    }

    /// <summary>
    /// 检查音效是否正在播放
    /// </summary>
    public bool IsPlaying(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            return !sound.isLoading && sound.source.isPlaying;
        }
        return false;
    }

    /// <summary>
    /// 设置音效循环
    /// </summary>
    public void SetLoop(string soundName, bool loop)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            sound.source.loop = loop;
        }
    }

    #endregion

    #region 音量控制

    /// <summary>
    /// 设置主音量
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
        SaveAudioSettings();
    }

    /// <summary>
    /// 设置音乐音量
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (isMusicMuted)
        {
            prevMusicVolume = musicVolume;
        }
        UpdateAllVolumes();
        SaveAudioSettings();
    }

    /// <summary>
    /// 设置音效音量
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (isSFXMuted)
        {
            prevSFXVolume = sfxVolume;
        }
        UpdateAllVolumes();
        SaveAudioSettings();
    }

    /// <summary>
    /// 更新所有音频源的音量
    /// </summary>
    private void UpdateAllVolumes()
    {
        foreach (var sound in soundDictionary.Values)
        {
            float typeVolume = GetTypeVolumeBySound(sound);
            sound.source.volume = sound.volume * masterVolume * typeVolume;
            if (sound.name == currentMusic)
            {
                sound.source.mute = isMusicMuted;
            }
            else
            {
                sound.source.mute = isSFXMuted;
            }
        }
    }

    private float GetTypeVolumeBySound(Sound sound)
    {
        if (sound.name == currentMusic)
        {
            return musicVolume;
        }
        else
            return sfxVolume;
    }

    /// <summary>
    /// 根据音频混合组获取对应类型的音量
    /// </summary>
    //private float GetAudioTypeVolume(AudioMixerGroup mixerGroup)
    //{
    //    if (mixerGroup == null) return 1f;

    //    string groupName = mixerGroup.name.ToLower();

    //    if (groupName.Contains("music"))
    //        return musicVolume;
    //    else if (groupName.Contains("sfx") || groupName.Contains("sound"))
    //        return sfxVolume;
    //    else
    //        return 1f;
    //}

    /// <summary>
    /// 淡入音效
    /// </summary>
    public void FadeIn(string soundName, float duration = 1f)
    {
        _ = GetSoundObj(soundName, (sound) =>
        {
            StartCoroutine(FadeAudio(sound.source, 0f, sound.volume * masterVolume, duration));
            if (!sound.source.isPlaying)
            {
                sound.source.Play();
            }
        });
    }

    /// <summary>
    /// 淡出音效
    /// </summary>
    public void FadeOut(string soundName, float duration = 1f)
    {
        _ = GetSoundObj(soundName, (sound) =>
        {
            StartCoroutine(FadeAudio(sound.source, sound.source.volume, 0f, duration, true));
        });
    }

    private System.Collections.IEnumerator FadeAudio(AudioSource audioSource, float fromVolume, float toVolume, float duration, bool stopAfterFade = false)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(fromVolume, toVolume, timer / duration);
            yield return null;
        }

        audioSource.volume = toVolume;

        if (stopAfterFade && audioSource.volume <= 0.01f)
        {
            audioSource.Stop();
        }
    }

    #endregion

    #region 设置保存和工具方法

    /// <summary>
    /// 保存音频设置
    /// </summary>
    private void SaveAudioSettings()
    {
        DataManager.SetDataByFloat("MasterVolume", masterVolume);
        DataManager.SetDataByFloat("MusicVolume", musicVolume);
        DataManager.SetDataByFloat("SFXVolume", sfxVolume);

        DataManager.SetDataByFloat("MusicMuted", isMusicMuted ? 1f : 0f);
        DataManager.SetDataByFloat("SFXMuted", isSFXMuted ? 1f : 0f);
        DataManager.SetDataByFloat("PrevMusicVolume", prevMusicVolume);
        DataManager.SetDataByFloat("PrevSFXVolume", prevSFXVolume);
    }

    /// <summary>
    /// 加载音频设置
    /// </summary>
    private void LoadAudioSettings()
    {
        masterVolume = DataManager.GetDataByFloat("MasterVolume", 1f);
        float loadedMusicVol = DataManager.GetDataByFloat("MusicVolume", 1f);
        float loadedSfxVol = DataManager.GetDataByFloat("SFXVolume", 1f);

        prevMusicVolume = DataManager.GetDataByFloat("PrevMusicVolume", loadedMusicVol);
        prevSFXVolume = DataManager.GetDataByFloat("PrevSFXVolume", loadedSfxVol);
        isMusicMuted = DataManager.GetDataByFloat("MusicMuted", 0f) > 0.5f;
        isSFXMuted = DataManager.GetDataByFloat("SFXMuted", 0f) > 0.5f;

        musicVolume = isMusicMuted ? 0f : loadedMusicVol;
        sfxVolume = isSFXMuted ? 0f : loadedSfxVol;

        UpdateAllVolumes();
    }

    /// <summary>
    /// 静音所有音频
    /// </summary>
    public void MuteAll()
    {
        SetMasterVolume(0f);
    }

    /// <summary>
    /// 取消静音
    /// </summary>
    public void UnmuteAll()
    {
        SetMasterVolume(1f);
    }

    /// <summary>
    /// 设置音乐静音/取消静音（会保留之前的音乐音量用于恢复）
    /// </summary>
    public void SetMusicMuted(bool muted)
    {
        if (muted == isMusicMuted) return;
        isMusicMuted = muted;
        if (muted)
        {
            prevMusicVolume = musicVolume;
            musicVolume = 0f;
        }
        else
        {
            musicVolume = Mathf.Clamp01(prevMusicVolume);
        }
        UpdateAllVolumes();
        SaveAudioSettings();
    }

    /// <summary>
    /// 设置音效静音/取消静音（会保留之前的音效音量用于恢复）
    /// </summary>
    public void SetSFXMuted(bool muted)
    {
        if (muted == isSFXMuted) return;
        isSFXMuted = muted;
        if (muted)
        {
            prevSFXVolume = sfxVolume;
            sfxVolume = 0f;
        }
        else
        {
            sfxVolume = Mathf.Clamp01(prevSFXVolume);
        }
        UpdateAllVolumes();
        SaveAudioSettings();
    }

    /// <summary>
    /// 查询当前音乐是否静音
    /// </summary>
    public bool IsMusicMuted() => isMusicMuted;

    /// <summary>
    /// 查询当前音效是否静音
    /// </summary>
    public bool IsSFXMuted() => isSFXMuted;

    /// <summary>
    /// 播放一次性音效（适合UI音效等）
    /// </summary>
    public void PlayOneShot(string soundName, float volumeScale = 1f)
    {
        _ = GetSoundObj(soundName, (sound) =>
         {
             float typeVolume = GetTypeVolumeBySound(sound);
             float finalVolume = sound.volume * masterVolume * typeVolume * volumeScale;
             sound.source.PlayOneShot(sound.clip, finalVolume);
         });
    }

    /// <summary>
    /// 在指定位置播放3D音效
    /// </summary>
    public void PlayAtPoint(string soundName, Vector3 position, float volumeScale = 1f)
    {
        _ = GetSoundObj(soundName, (sound) =>
        {
            float typeVolume = GetTypeVolumeBySound(sound);
            float finalVolume = sound.volume * masterVolume * typeVolume * volumeScale;
            AudioSource.PlayClipAtPoint(sound.clip, position, finalVolume);
        });
    }

    private async Task GetSoundObj(string soundName, Action<Sound> onGet)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound result))
        {
            while (result.isLoading)
                await Task.Yield();

            onGet?.Invoke(result);
        }
        else
        {
            Sound sound = new Sound();
            sound.name = soundName;
            sound.volume = sound.name == currentMusic ? 0.3f : 1f;
            sound.pitch = 1f;
            sound.isLoading = true;
            soundDictionary.Add(soundName, sound);
            var clip = await AssetsManager.AsyncLoadAsset<AudioClip>(soundName);
            sound.clip = clip;
            SpawnSoundGO(sound);
            sound.isLoading = false;
            onGet?.Invoke(sound);
        }
    }

    /// <summary>
    /// 获取音频长度
    /// </summary>
    public float GetClipLength(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            return sound.clip.length;
        }
        return 0f;
    }

    #endregion


}


public interface IAudioSet
{
    public string GetPath();

    public float GetVolume();
}

public class SingleAudio : IAudioSet
{
    public float volmue;
    public string path;

    public string GetPath()
    {
        return path;
    }

    public float GetVolume()
    {
        return volmue;
    }
}

public class StepAudio : IAudioSet
{
    public int curStep = 0;
    public float timer = 0f;
    public float timerLimit = 0f;

    public float volmue = 0f;

    public string[] paths;

    public string GetPath()
    {
        var idx = Mathf.Clamp(curStep, 0, paths.Length - 1);
        var path = paths[idx];
        //Debug.Log($"get {idx}");
        curStep.ToNext(0, paths.Length - 1);
        return path;
    }

    public float GetVolume()
    {
        return volmue;
    }
}

/// <summary>
/// 挂件 处理获取AudioPath的相关功能
/// </summary>
public class AudioPathHelperPendant : MonoSingleton<AudioPathHelperPendant>
{
    public override bool DontDestory => true;

    private Dictionary<SoundName, IAudioSet> _audioSetDict;

    protected override void OnAwake()
    {
        _audioSetDict = new Dictionary<SoundName, IAudioSet>();

        for (int i = 0; i < AudioConfigs.Configs.Length; i++)
        {
            var config = AudioConfigs.Configs[i];
            var audioSet = new SingleAudio();
            audioSet.volmue = config.volume;
            audioSet.path = config.path;

            _audioSetDict.Add(config.Item1, audioSet);
        }

        //for (int i = 0; i < AudioConfigs.stepAudioConfigs.Length; ++i)
        //{
        //    var config = AudioConfigs.stepAudioConfigs[i];
        //    var counter = new StepAudio();
        //    counter.volmue = config.volume;
        //    counter.timerLimit = config.timerLimit;
        //    counter.paths = config.paths;
        //    _audioSetDict.Add(config.Item1, counter);

        //}
    }

    public bool TryGetPathByTag(SoundName soundName, out string name)
    {
        if (_audioSetDict.TryGetValue(soundName, out var set))
        {
            name = set.GetPath();
            return true;
        }
        name = string.Empty;
        return false;
    }

    public bool TryGetVolumeByTag(SoundName soundName, out float volume)
    {
        if (_audioSetDict.TryGetValue(soundName, out var set))
        {
            volume = set.GetVolume();
            return true;
        }

        volume = 1f;
        return false;
    }
}