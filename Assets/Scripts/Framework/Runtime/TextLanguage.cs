using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class TextLanguage : MonoBehaviour, IMultiText
{
    public int LanguageID;

    void Awake()
    {
        if (LanguageTextManager.Inited)
        {
            LanguageTextManager.RegText(this);
        }
    }

    private void Start()
    {
        if (LanguageTextManager.Inited)
            UpdateText();
    }


    void OnDestroy()
    {
        if (LanguageTextManager.Inited)
            LanguageTextManager.RemText(this);
    }

    public void UpdateText()
    {
        var Text = GetComponent<TextMeshPro>();
        if (LanguageID == 0)
        {
            return;
        }
        if (Text == null)
        {
            Debug.Log($"{name} is null");
            return;
        }
        string str = LanguageTextManager.GetLangStr(LanguageID);
        if (!string.IsNullOrEmpty(str))
            Text.text = str.Replace("\\n", "\n").Replace("/r/n", "\n");

    }




}


