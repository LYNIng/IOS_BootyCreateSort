using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComponentPart : MonoBehaviour
{
#if UNITY_EDITOR
    public enum ComponentType
    {
        None = 0,
        Image,
        Button,
        Text,
        TextMeshProUGUI,
        Transform,
        RectTransform,
        GameObject,
        Slider,
        InputField,
        ScrollRect,
        Dropdown,
        TabGroup,
        TextLanguagePro,
        Toggle,
        TMP_InputField,
        TextMeshPro,
        RawImage,
        Scrollbar,
        Canvas,
        CanvasGroup,
    }

    public static int MaxUIComponentNum = (int)ComponentType.CanvasGroup + 1;


    public int m_Component;
    public string m_Comment;

#endif
}
