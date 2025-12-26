using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AutoMonoComposite))]
public class AutoMonoCompositeEditor : Editor
{
    public const string ExtendFileName = "_";
    private const string PREFS_KEY_AUTOCODE_PATH = "AutoMonoCompositeEditor_AutoCodePath";

    public const string c_DefaultCodePath = "/Scripts/Gamework/AutoCode/AutoMonoComposite";

    [SerializeField]
    public static string s_AutoCodePath = c_DefaultCodePath;

    private void OnEnable()
    {
        if (EditorPrefs.HasKey(PREFS_KEY_AUTOCODE_PATH))
        {
            s_AutoCodePath = EditorPrefs.GetString(PREFS_KEY_AUTOCODE_PATH, c_DefaultCodePath);
        }
    }
}
