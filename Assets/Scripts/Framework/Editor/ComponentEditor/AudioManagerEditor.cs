#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AudioManager audioManager = (AudioManager)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("快速控制", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("静音所有"))
        {
            audioManager.MuteAll();
        }
        if (GUILayout.Button("取消静音"))
        {
            audioManager.UnmuteAll();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("音量控制", EditorStyles.boldLabel);

        float masterVolume = EditorGUILayout.Slider("主音量", audioManager.masterVolume, 0f, 1f);
        float musicVolume = EditorGUILayout.Slider("音乐音量", audioManager.musicVolume, 0f, 1f);
        float sfxVolume = EditorGUILayout.Slider("音效音量", audioManager.sfxVolume, 0f, 1f);

        if (masterVolume != audioManager.masterVolume)
            audioManager.SetMasterVolume(masterVolume);
        if (musicVolume != audioManager.musicVolume)
            audioManager.SetMusicVolume(musicVolume);
        if (sfxVolume != audioManager.sfxVolume)
            audioManager.SetSFXVolume(sfxVolume);
    }
}
#endif