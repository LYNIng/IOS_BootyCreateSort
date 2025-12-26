using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ComponentPart))]
[CanEditMultipleObjects]
public class ComponentPartEditor : Editor
{
    public const string ExtendFileName = "_";
    private const string PREFS_KEY_AUTOCODE_PATH = "UIComponentEditor_AutoCodePath";

    public const string c_DefaultCodePath = "/Scripts/Gamework/AutoCode/ComponentPart";

    [SerializeField]
    public static string s_AutoCodePath = c_DefaultCodePath;

    #region Static Methods

    [UnityEditor.Callbacks.DidReloadScripts(0)]
    private static void OnScriptReload()
    {
        s_OnScriptReload = true;
    }

    [MenuItem("GameObject/Add Bind &b", false, -1)]
    public static void AddBind()
    {
        foreach (var gameObject in Selection.gameObjects)
        {
            if (gameObject && !gameObject.GetComponent<ComponentPart>())
            {
                var component = gameObject.AddComponent<ComponentPart>();
                Undo.RegisterCreatedObjectUndo(component, "Add UIComponent");
            }
        }
    }

    public string ErrorStr;
    #endregion

    private ComponentPart[] m_UIBinds;
    private MonoComposite m_MonoComposite;
    private List<ComponentPart> m_CachedComponents;
    private List<MonoComposite> m_CachedMonoComposites;
    private Dictionary<int, List<ComponentPart>> m_ComponentCache;
    private static Dictionary<Type, Dictionary<string, FieldInfo>> s_FieldCache =
        new Dictionary<Type, Dictionary<string, FieldInfo>>();

    private static bool s_OnScriptReload = false;
    private bool m_CacheDirty = true;

    // 组件类型与必需组件的映射
    public static readonly Dictionary<ComponentPart.ComponentType, Type[]> s_RequiredComponents = new Dictionary<ComponentPart.ComponentType, Type[]>
    {
        { ComponentPart.ComponentType.Button, new Type[] { typeof(UnityEngine.UI.Button) } },
        { ComponentPart.ComponentType.Image, new Type[] { typeof(UnityEngine.UI.Image) } },
        { ComponentPart.ComponentType.Text, new Type[] { typeof(TMPro.TMP_Text) } },
        { ComponentPart.ComponentType.TextMeshPro, new Type[] { typeof(TMPro.TextMeshPro) } },
        { ComponentPart.ComponentType.TextMeshProUGUI, new Type[] { typeof(TMPro.TextMeshProUGUI) } },
        { ComponentPart.ComponentType.RawImage, new Type[] { typeof(UnityEngine.UI.RawImage) } },
        { ComponentPart.ComponentType.Slider, new Type[] { typeof(UnityEngine.UI.Slider) } },
        { ComponentPart.ComponentType.Scrollbar, new Type[] { typeof(UnityEngine.UI.Scrollbar) } },
        { ComponentPart.ComponentType.Dropdown, new Type[] { typeof(TMPro.TMP_Dropdown), typeof(UnityEngine.UI.Dropdown) } },
        { ComponentPart.ComponentType.InputField, new Type[] {  typeof(UnityEngine.UI.InputField) } },
        { ComponentPart.ComponentType.TMP_InputField, new Type[] { typeof(TMPro.TMP_InputField) } },
        { ComponentPart.ComponentType.Toggle, new Type[] { typeof(UnityEngine.UI.Toggle) } },
        { ComponentPart.ComponentType.ScrollRect, new Type[] { typeof(UnityEngine.UI.ScrollRect) } },
        { ComponentPart.ComponentType.Canvas, new Type[] { typeof(UnityEngine.Canvas) } },
        { ComponentPart.ComponentType.CanvasGroup, new Type[] { typeof(UnityEngine.CanvasGroup) } },
        { ComponentPart.ComponentType.Transform, new Type[] { typeof(UnityEngine.Transform) } },
        { ComponentPart.ComponentType.RectTransform, new Type[] { typeof(UnityEngine.RectTransform) } },
        { ComponentPart.ComponentType.GameObject, new Type[] { typeof(UnityEngine.GameObject) } },
        { ComponentPart.ComponentType.TextLanguagePro, new Type[] { typeof(TextLanguagePro) } },
        { ComponentPart.ComponentType.TabGroup, new Type[] { typeof(TabGroup) } },
    };

    private void OnEnable()
    {
        // 读取保存的路径设置
        if (EditorPrefs.HasKey(PREFS_KEY_AUTOCODE_PATH))
        {
            s_AutoCodePath = EditorPrefs.GetString(PREFS_KEY_AUTOCODE_PATH, "/Scripts/Gamework/AutoCode/ComponentPart");
        }

        ErrorStr = null;
        m_UIBinds = targets.Select((obj) => (ComponentPart)obj).ToArray();
        if (m_UIBinds.Length > 0)
        {
            MonoComposite panel = null;
            for (int i = 0; i < m_UIBinds.Length; ++i)
            {
                var tmp = GetMonoComposite(m_UIBinds[i].transform);
                if (panel == null) panel = tmp;
                else if (tmp != panel)
                {
                    ErrorStr = "选择的组件父节点不一致.";
                    break;
                }
            }
            if (!string.IsNullOrEmpty(ErrorStr)) return;
            m_MonoComposite = panel;
        }
        RefreshCache();
    }

    public override void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("只在编辑时运行", MessageType.Info);
            return;
        }

        if (m_UIBinds == null || m_UIBinds.Length == 0) return;

        // 显示多选信息
        if (targets.Length > 1)
        {
            EditorGUILayout.HelpBox($"正在编辑 {targets.Length} 个UIComponent", MessageType.Info);
        }

        if (!string.IsNullOrEmpty(ErrorStr))
        {
            EditorGUILayout.HelpBox($"{ErrorStr}",
                MessageType.Warning);
            return;
        }

        if (m_MonoComposite == null)
        {
            EditorGUILayout.HelpBox($"这是一个UI组件,需要父节点拥有继承自{typeof(UIBase)}的组件才能起作用.",
                MessageType.Warning);
            return;
        }

        EditorGUI.BeginChangeCheck();

        DrawSettingsSection();
        DrawComponentPopup();
        DrawCommentAndGenerateButton();

        // 绘制必要组件检测警告
        DrawRequiredComponentWarnings();

        // 绘制自动修复按钮（如果有问题）
        DrawAutoFixButton();

        if (EditorGUI.EndChangeCheck())
        {
            // 标记所有目标为脏
            foreach (var targetObj in targets)
            {
                EditorUtility.SetDirty(targetObj);
            }
            m_CacheDirty = true;
        }

        if (s_OnScriptReload)
        {
            GUIComponentSetValue();
            s_OnScriptReload = false;
        }
    }

    private void DrawSettingsSection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("设置", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        s_AutoCodePath = EditorGUILayout.TextField("代码生成路径:", s_AutoCodePath);
        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetString(PREFS_KEY_AUTOCODE_PATH, s_AutoCodePath);
        }

        EditorGUILayout.Space();
    }

    private void DrawComponentPopup()
    {
        var componentNames = Enum.GetNames(typeof(ComponentPart.ComponentType));

        // 检查所有选中的组件是否类型一致
        bool allSameType = CheckAllComponentsSameType(out int? commonType);

        int selectedIndex = -1;

        // 根据类型一致性决定显示的值
        if (allSameType && commonType.HasValue)
        {
            // 所有组件类型相同，显示当前类型
            selectedIndex = commonType.Value;
        }
        else
        {
            // 类型不一致，显示"Mixed"
            selectedIndex = -1;
        }

        // 绘制下拉菜单
        EditorGUI.BeginChangeCheck();
        int newSelectedIndex = EditorGUILayout.Popup("组件类型:", selectedIndex, componentNames);

        // 如果用户选择了新的类型
        if (EditorGUI.EndChangeCheck() && newSelectedIndex >= 0 && newSelectedIndex != selectedIndex)
        {
            ApplyComponentTypeToAll(newSelectedIndex);
        }

        // 如果当前显示"Mixed"，添加一个提示
        if (selectedIndex == -1 && !allSameType)
        {
            EditorGUILayout.HelpBox("选中了不同类型的组件", MessageType.Info);
        }
    }

    /// <summary>
    /// 检查所有选中组件的类型是否一致
    /// </summary>
    private bool CheckAllComponentsSameType(out int? commonType)
    {
        commonType = null;

        if (m_UIBinds == null || m_UIBinds.Length == 0)
            return false;

        // 获取第一个组件的类型作为基准
        int firstType = m_UIBinds[0].m_Component;
        commonType = firstType;

        // 检查后续组件是否与第一个组件类型相同
        for (int i = 1; i < m_UIBinds.Length; i++)
        {
            if (m_UIBinds[i].m_Component != firstType)
            {
                commonType = null;
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 将组件类型应用到所有选中组件
    /// </summary>
    private void ApplyComponentTypeToAll(int componentType)
    {
        if (m_UIBinds == null || m_UIBinds.Length == 0)
            return;

        // 记录撤销操作
        Undo.RecordObjects(m_UIBinds.ToArray(), "Change UIComponent Type");

        // 应用新类型到所有组件
        foreach (var bind in m_UIBinds)
        {
            bind.m_Component = componentType;
            EditorUtility.SetDirty(bind);
        }

    }

    private void DrawCommentAndGenerateButton()
    {
        // 检查所有注释是否一致
        bool allSameComment = CheckAllComponentsSameComment(out string commonComment);

        EditorGUI.BeginChangeCheck();

        string newComment = null;
        if (allSameComment)
        {
            // 所有注释相同，显示当前注释
            newComment = EditorGUILayout.TextField("注释:", commonComment, GUILayout.Height(20));
        }
        else
        {
            // 注释不一致，显示空
            newComment = EditorGUILayout.TextField("注释:", "", GUILayout.Height(20));
            // 只有选中多个对象时才显示"注释不一致"的提示
            if (m_UIBinds.Length > 1)
            {
                EditorGUILayout.HelpBox("注释不一致", MessageType.Info);
            }
        }

        if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(newComment))
        {
            ApplyCommentToAll(newComment);
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.enabled = m_MonoComposite != null;
        if (GUILayout.Button("生成代码", GUILayout.Height(20), GUILayout.Width(80)))
        {
            GenerateCodeFile();
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 检查所有选中组件的注释是否一致
    /// </summary>
    private bool CheckAllComponentsSameComment(out string commonComment)
    {
        commonComment = null;

        if (m_UIBinds == null || m_UIBinds.Length == 0)
            return false;

        // 获取第一个组件的注释作为基准
        string firstComment = m_UIBinds[0].m_Comment;
        commonComment = firstComment;

        // 检查后续组件是否与第一个组件注释相同
        for (int i = 1; i < m_UIBinds.Length; i++)
        {
            if (m_UIBinds[i].m_Comment != firstComment)
            {
                commonComment = null;
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 将注释应用到所有选中组件
    /// </summary>
    private void ApplyCommentToAll(string comment)
    {
        if (m_UIBinds == null || m_UIBinds.Length == 0)
            return;

        // 记录撤销操作
        Undo.RecordObjects(m_UIBinds.ToArray(), "Change UIComponent Comment");

        // 应用新注释到所有组件
        foreach (var bind in m_UIBinds)
        {
            bind.m_Comment = comment;
            EditorUtility.SetDirty(bind);
        }
    }

    /// <summary>
    /// 检查并绘制必要组件警告
    /// </summary>
    private void DrawRequiredComponentWarnings()
    {
        // 检查所有选中组件是否有警告
        List<string> warnings = new List<string>();

        foreach (var uiBind in m_UIBinds)
        {
            var componentType = (ComponentPart.ComponentType)uiBind.m_Component;

            // 跳过 None 类型
            if (componentType == ComponentPart.ComponentType.None || componentType == ComponentPart.ComponentType.GameObject)
                continue;

            // 检查是否有必要组件的定义
            if (s_RequiredComponents.TryGetValue(componentType, out Type[] requiredTypes))
            {
                bool hasRequiredComponent = false;
                List<string> missingComponentNames = new List<string>();

                foreach (var requiredType in requiredTypes)
                {
                    var component = uiBind.gameObject.GetComponent(requiredType);
                    if (component != null)
                    {
                        hasRequiredComponent = true;
                        break;
                    }
                    missingComponentNames.Add(requiredType.Name);
                }

                if (!hasRequiredComponent)
                {
                    string missingNames = string.Join(" 或 ", missingComponentNames);
                    warnings.Add($"{uiBind.gameObject.name}: 需要添加 {missingNames} 组件（{componentType}）");
                }
            }
        }

        if (warnings.Count > 0)
        {
            EditorGUILayout.Space();
            string warningMessage = $"检测到 {warnings.Count} 个组件缺少必要组件";

            if (warnings.Count <= 3)
            {
                warningMessage += ":\n" + string.Join("\n", warnings);
            }
            else
            {
                warningMessage += $"，前3个为:\n" + string.Join("\n", warnings.Take(3)) + $"\n... 等 {warnings.Count - 3} 个更多";
            }

            EditorGUILayout.HelpBox(warningMessage, MessageType.Warning);
        }
    }

    /// <summary>
    /// 绘制自动修复按钮
    /// </summary>
    private void DrawAutoFixButton()
    {
        bool needsFix = false;

        foreach (var uiBind in m_UIBinds)
        {
            var componentType = (ComponentPart.ComponentType)uiBind.m_Component;

            if (componentType == ComponentPart.ComponentType.None || componentType == ComponentPart.ComponentType.GameObject)
                continue;

            if (s_RequiredComponents.TryGetValue(componentType, out Type[] requiredTypes))
            {
                bool hasRequiredComponent = false;

                foreach (var requiredType in requiredTypes)
                {
                    if (uiBind.gameObject.GetComponent(requiredType) != null)
                    {
                        hasRequiredComponent = true;
                        break;
                    }
                }

                if (!hasRequiredComponent)
                {
                    needsFix = true;
                    break;
                }
            }
        }

        if (needsFix)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("自动添加必要组件", GUILayout.Width(150)))
            {
                AddRequiredComponentsForAll();
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 添加必要组件（多选版本）
    /// </summary>
    private void AddRequiredComponentsForAll()
    {
        int addedCount = 0;

        // 记录撤销操作
        Undo.SetCurrentGroupName("Add Required Components to All");
        int undoGroup = Undo.GetCurrentGroup();

        foreach (var uiBind in m_UIBinds)
        {
            var componentType = (ComponentPart.ComponentType)uiBind.m_Component;

            if (!s_RequiredComponents.TryGetValue(componentType, out Type[] requiredTypes))
                continue;

            Undo.RecordObject(uiBind.gameObject, "Add Required Components");

            bool addedComponent = false;

            foreach (var requiredType in requiredTypes)
            {
                // 如果已经有该类型组件，跳过
                if (uiBind.gameObject.GetComponent(requiredType) != null)
                    continue;

                try
                {
                    uiBind.gameObject.AddComponent(requiredType);
                    addedComponent = true;
                    addedCount++;
                    break; // 只添加第一个需要的组件
                }
                catch (Exception ex)
                {
                    Debug.LogError($"无法为 {uiBind.gameObject.name} 添加 {requiredType.Name} 组件: {ex.Message}");
                }
            }

            if (addedComponent)
            {
                EditorUtility.SetDirty(uiBind.gameObject);
                EditorUtility.SetDirty(uiBind);
            }
        }

        Undo.CollapseUndoOperations(undoGroup);

        if (addedCount > 0)
        {
            EditorUtility.DisplayDialog("完成", $"已为 {addedCount} 个组件添加了必要组件", "确定");
        }
    }

    private void GenerateCodeFile()
    {
        // 先检查所有 UIComponent 的必要组件
        if (!ValidateAllComponents())
        {
            bool shouldContinue = EditorUtility.DisplayDialog("警告",
                "检测到一些 GameObject 缺少必要的组件，继续生成代码可能会导致运行时错误。是否继续？",
                "继续生成", "取消");

            if (!shouldContinue)
                return;
        }

        if (m_MonoComposite == null)
        {
            EditorUtility.DisplayDialog("错误", "未找到UIBase组件", "确定");
            return;
        }
        var MonoCompositeType = m_MonoComposite.GetType();
        string fileName = MonoCompositeType.Name;
        string fullPath = CreateCode(s_AutoCodePath, fileName, m_MonoComposite.GetType());

        if (!string.IsNullOrEmpty(fullPath))
        {
            EditorUtility.DisplayDialog("成功", $"代码文件已生成:\n{fullPath}", "确定");
            AssetDatabase.Refresh();
        }
    }

    /// <summary>
    /// 验证所有 UIComponent 的必要组件
    /// </summary>
    private bool ValidateAllComponents()
    {
        if (m_CachedComponents == null)
            RefreshCache();

        if (m_CachedComponents == null)
            return true;

        bool allValid = true;

        foreach (var component in m_CachedComponents)
        {
            if (component == null) continue;

            var componentType = (ComponentPart.ComponentType)component.m_Component;

            if (componentType == ComponentPart.ComponentType.None || componentType == ComponentPart.ComponentType.GameObject)
                continue;

            if (s_RequiredComponents.TryGetValue(componentType, out Type[] requiredTypes))
            {
                bool hasRequiredComponent = false;

                foreach (var requiredType in requiredTypes)
                {
                    if (component.gameObject.GetComponent(requiredType) != null)
                    {
                        hasRequiredComponent = true;
                        break;
                    }
                }

                if (!hasRequiredComponent)
                {
                    Debug.LogWarning($"GameObject '{component.gameObject.name}' 缺少必要组件。类型：{componentType}，需要：{string.Join(" 或 ", requiredTypes.Select(t => t.Name))}");
                    allValid = false;
                }
            }
        }

        return allValid;
    }

    private MonoComposite GetMonoComposite(Transform transform)
    {
        if (transform == null) return null;

        MonoComposite MonoComposite = transform.GetComponentInParent<MonoComposite>(true);
        return MonoComposite;
    }

    private void RefreshCache()
    {
        if (m_MonoComposite == null) return;

        if (m_CachedComponents == null) m_CachedComponents = new List<ComponentPart>();
        else m_CachedComponents.Clear();
        //m_CachedComponents = m_MonoComposite.GetComponentsInChildren<ComponentPart>(true).ToList();
        if (m_CachedMonoComposites == null) m_CachedMonoComposites = new List<MonoComposite>();
        else m_CachedMonoComposites.Clear();

        CacheNestdComposite(m_MonoComposite.transform, m_CachedComponents, m_CachedMonoComposites);


        BuildComponentCache();
        m_CacheDirty = false;
    }

    private void CacheNestdComposite(Transform trans, List<ComponentPart> cachedComponentList, List<MonoComposite> monoCompositesList)
    {
        if (trans.TryGetComponent(out MonoComposite monoCom) && monoCom != m_MonoComposite)
        {
            m_CachedMonoComposites.Add(monoCom);
            return;
        }

        if (trans.TryGetComponent(out ComponentPart comPart))
        {
            cachedComponentList.Add(comPart);
        }

        for (int i = 0; i < trans.childCount; ++i)
        {
            var child = trans.GetChild(i);
            CacheNestdComposite(child, cachedComponentList, monoCompositesList);
        }
    }

    private void BuildComponentCache()
    {
        m_ComponentCache = new Dictionary<int, List<ComponentPart>>();

        if (m_CachedComponents == null) return;

        foreach (var component in m_CachedComponents)
        {
            if (!m_ComponentCache.ContainsKey(component.m_Component))
                m_ComponentCache[component.m_Component] = new List<ComponentPart>();

            m_ComponentCache[component.m_Component].Add(component);
        }
    }

    private string CreateCode(string filePath, string fileName, Type uiType)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("文件路径不能为空");
            return null;
        }

        try
        {
            StringBuilder sbr = new StringBuilder(2048);

            // 文件头部
            AppendFileHeader(sbr);

            // Using语句
            AppendUsingStatements(sbr);

            // 类定义
            AppendClassDefinition(sbr, fileName, uiType);

            // 组件字段
            AppendComponentFields(sbr);

            sbr.AppendLine("}");

            // 确保目录存在
            string folderPath = Application.dataPath + filePath;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fullFilePath = Path.Combine(folderPath, $"{fileName}{ExtendFileName}.cs");
            File.WriteAllText(fullFilePath, sbr.ToString(), Encoding.UTF8);

            Debug.Log($"代码文件生成成功: {fullFilePath}");
            return fullFilePath;
        }
        catch (Exception ex)
        {
            Debug.LogError($"生成代码文件失败: {ex.Message}");
            EditorUtility.DisplayDialog("错误", $"生成代码文件失败: {ex.Message}", "确定");
            return null;
        }
    }

    private void AppendFileHeader(StringBuilder sbr)
    {
        sbr.AppendLine("//===================================================");
        sbr.AppendLine($"// 创建时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sbr.AppendLine("// 备    注：此代码为工具生成 请勿手工修改");
        sbr.AppendLine("//===================================================");
        sbr.AppendLine();
    }

    private void AppendUsingStatements(StringBuilder sbr)
    {
        sbr.AppendLine("using UnityEngine;");
        sbr.AppendLine("using UnityEngine.UI;");
        sbr.AppendLine("using TMPro;");
        sbr.AppendLine();
    }

    private void AppendClassDefinition(StringBuilder sbr, string fileName, Type uiType)
    {
        sbr.AppendLine("/// <summary>");
        sbr.AppendLine($"/// {fileName}");
        sbr.AppendLine("/// </summary>");

        if (uiType.BaseType != null && uiType.BaseType.IsGenericType)
        {
            var genericTypeStr = uiType.BaseType.Name.Split('`')[0];
            string genericArgument = uiType.BaseType.GetGenericArguments()[0].Name;
            sbr.AppendLine($"public partial class {fileName} : {genericTypeStr}<{genericArgument}>");
        }
        else
        {
            sbr.AppendLine($"public partial class {fileName} : {uiType.BaseType.Name}");
        }

        sbr.AppendLine("{");
        sbr.AppendLine();
        sbr.AppendLine($"    public const string UIName = \"{fileName}\";");
        sbr.AppendLine();
    }

    private void AppendComponentFields(StringBuilder sbr)
    {
        if (m_ComponentCache == null) return;

        for (int i = 0; i < ComponentPart.MaxUIComponentNum; i++)
        {
            if (m_ComponentCache.ContainsKey(i) && m_ComponentCache[i].Count > 0)
            {
                AppendComponentFieldsByType(sbr, i, m_ComponentCache[i]);
            }
        }
    }

    private void AppendComponentFieldsByType(StringBuilder sbr, int componentType, List<ComponentPart> components)
    {
        if (componentType == 0) return; // 跳过None类型

        string componentTypeName = ((ComponentPart.ComponentType)componentType).ToString();

        foreach (var component in components)
        {
            if (string.IsNullOrEmpty(component.gameObject.name)) continue;

            sbr.AppendLine("    /// <summary>");
            sbr.AppendLine($"    /// {component.m_Comment}");
            sbr.AppendLine("    /// </summary>");
            sbr.AppendLine($"    public {componentTypeName} {component.gameObject.name};");
            sbr.AppendLine();
        }
    }

    private void GUIComponentSetValue()
    {
        if (m_MonoComposite == null) return;

        var panelType = m_MonoComposite.GetType();

        for (int i = 1; i < ComponentPart.MaxUIComponentNum; i++)
        {
            if (!m_ComponentCache.ContainsKey(i)) continue;

            foreach (var component in m_ComponentCache[i])
            {
                if (component == null) continue;

                var fieldInfo = GetCachedField(panelType, component.gameObject.name);
                if (fieldInfo != null && s_RequiredComponents.TryGetValue((ComponentPart.ComponentType)component.m_Component, out var typeArr))
                {
                    var resultT = typeArr.Where((t) =>
                    {
                        return fieldInfo.FieldType.IsAssignableFrom(t);
                    });
                    if (resultT.Count() > 0)
                    {
                        try
                        {
                            var firstT = resultT.First();
                            if (firstT == typeof(GameObject))
                            {
                                fieldInfo.SetValue(m_MonoComposite, component.gameObject);
                            }
                            else
                            {
                                fieldInfo.SetValue(m_MonoComposite, component.GetComponent(resultT.First()));
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"设置字段 {component.gameObject.name} 失败: {ex.Message}");
                        }
                    }
                    else
                    {
                        Debug.LogError($"设置字段 {component.gameObject.name} 失败: 没有找到对应的类型");
                    }
                }
            }
        }

        // 标记场景为已修改
        if (!Application.isPlaying)
        {
            for (int i = 0; i < m_UIBinds.Length; ++i)
            {
                var bind = m_UIBinds[i];
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(bind.gameObject.scene);
            }
        }
    }

    private FieldInfo GetCachedField(Type type, string fieldName)
    {
        if (type == null || string.IsNullOrEmpty(fieldName))
            return null;

        if (!s_FieldCache.ContainsKey(type))
            s_FieldCache[type] = new Dictionary<string, FieldInfo>();

        if (!s_FieldCache[type].ContainsKey(fieldName))
        {
            s_FieldCache[type][fieldName] = type.GetField(fieldName,
                BindingFlags.Public | BindingFlags.Instance);
        }

        return s_FieldCache[type][fieldName];
    }

    [MenuItem("Tools/UI组件/清除字段缓存")]
    private static void ClearFieldCache()
    {
        s_FieldCache.Clear();
        Debug.Log("字段缓存已清除");
    }
}