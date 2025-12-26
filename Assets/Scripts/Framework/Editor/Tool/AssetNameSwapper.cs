using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetNameSwapper : Editor
{
    [MenuItem("Assets/交换资源名称", true, 100)]
    private static bool ValidateSwapAssetNames()
    {
        // 只有在选中两个资源时才启用菜单项
        return Selection.objects.Length == 2;
    }

    [MenuItem("Assets/交换资源名称", false, 100)]
    private static void SwapAssetNames()
    {
        // 获取选中的两个资源
        Object[] selectedObjects = Selection.objects;

        if (selectedObjects.Length != 2)
        {
            EditorUtility.DisplayDialog("错误", "请选择两个资源进行名称交换", "确定");
            return;
        }

        Object asset1 = selectedObjects[0];
        Object asset2 = selectedObjects[1];

        // 获取资源的完整路径
        string path1 = AssetDatabase.GetAssetPath(asset1);
        string path2 = AssetDatabase.GetAssetPath(asset2);

        // 获取文件信息
        string dir1 = Path.GetDirectoryName(path1);
        string dir2 = Path.GetDirectoryName(path2);
        string name1 = Path.GetFileNameWithoutExtension(path1);
        string name2 = Path.GetFileNameWithoutExtension(path2);
        string ext1 = Path.GetExtension(path1);
        string ext2 = Path.GetExtension(path2);

        // 检查扩展名是否相同（可选，可根据需求调整）
        if (ext1 != ext2)
        {
            bool continueSwap = EditorUtility.DisplayDialog("警告",
                "两个资源的文件类型不同,是否继续交换名称?", "继续", "取消");

            if (!continueSwap)
                return;
        }

        // 检查两个资源是否在同一目录
        bool sameDirectory = dir1 == dir2;

        if (!sameDirectory)
        {
            bool continueSwap = EditorUtility.DisplayDialog("警告",
                "两个资源在不同目录,是否继续交换名称?", "继续", "取消");

            if (!continueSwap)
                return;
        }

        try
        {
            // 生成临时名称避免冲突
            string tempName = "__temp_swap_name__";
            string tempPath1 = Path.Combine(dir1, tempName + ext1);

            // 开始资产操作（支持撤销）
            AssetDatabase.StartAssetEditing();

            // 记录原始名称用于撤销
            string originalName1 = name1;
            string originalName2 = name2;

            // 先重命名第一个资源为临时名称
            string error = AssetDatabase.RenameAsset(path1, tempName);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"重命名失败: {error}");
                AssetDatabase.StopAssetEditing();
                return;
            }

            // 重命名第二个资源为第一个资源的名称
            error = AssetDatabase.RenameAsset(path2, originalName1);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"重命名失败: {error}");
                // 尝试恢复第一个资源
                AssetDatabase.RenameAsset(tempPath1, originalName1);
                AssetDatabase.StopAssetEditing();
                return;
            }

            // 重命名第一个资源（现在是临时名称）为第二个资源的名称
            error = AssetDatabase.RenameAsset(tempPath1, originalName2);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"重命名失败: {error}");
                // 尝试恢复
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(asset2), originalName2);
                AssetDatabase.RenameAsset(path2, originalName1);
                AssetDatabase.StopAssetEditing();
                return;
            }

            AssetDatabase.StopAssetEditing();

            // 刷新并选中交换后的资源
            AssetDatabase.Refresh();

            // 获取交换后的资源并选中
            string finalPath1 = Path.Combine(dir1, originalName2 + ext1);
            string finalPath2 = Path.Combine(dir2, originalName1 + ext2);

            Object swappedAsset1 = AssetDatabase.LoadAssetAtPath<Object>(finalPath1);
            Object swappedAsset2 = AssetDatabase.LoadAssetAtPath<Object>(finalPath2);

            Selection.objects = new Object[] { swappedAsset1, swappedAsset2 };

            Debug.Log($"成功交换资源名称: '{originalName1}'<-> '{originalName2}'");
        }
        catch (System.Exception e)
        {
            AssetDatabase.StopAssetEditing();
            Debug.LogError($"交换资源名称时出错: {e.Message}");
            EditorUtility.DisplayDialog("错误", $"交换资源名称失败: {e.Message}", "确定");
        }
    }

    // 添加一个窗口工具，提供更多选项
    [MenuItem("Tools/资源工具/高级资源名称交换器")]
    public static void ShowSwapWindow()
    {
        SwapAssetWindow window = EditorWindow.GetWindow<SwapAssetWindow>("资源名称交换器");
        window.minSize = new Vector2(350, 200);
    }
}

// 高级交换窗口
public class SwapAssetWindow : EditorWindow
{
    private Object asset1;
    private Object asset2;
    private bool swapExtensions = false;
    private bool keepOriginalSelection = true;

    private void OnGUI()
    {
        GUILayout.Label("资源名称交换器", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("选择两个要交换名称的资源", MessageType.Info);

        EditorGUILayout.Space();

        // 资源选择字段
        asset1 = EditorGUILayout.ObjectField("资源 1", asset1, typeof(Object), false);
        asset2 = EditorGUILayout.ObjectField("资源 2", asset2, typeof(Object), false);

        EditorGUILayout.Space();

        // 选项
        swapExtensions = EditorGUILayout.Toggle("交换文件扩展名", swapExtensions);
        keepOriginalSelection = EditorGUILayout.Toggle("保持原始资源选中", keepOriginalSelection);

        EditorGUILayout.Space();

        GUI.enabled = asset1 != null && asset2 != null;

        if (GUILayout.Button("交换名称", GUILayout.Height(30)))
        {
            SwapSelectedAssets();
        }

        GUI.enabled = true;

        EditorGUILayout.Space();

        // 快速选择当前选中的资源
        if (Selection.objects.Length == 2)
        {
            if (GUILayout.Button("使用当前选中资源"))
            {
                asset1 = Selection.objects[0];
                asset2 = Selection.objects[1];
            }
        }
    }

    private void SwapSelectedAssets()
    {
        if (asset1 == null || asset2 == null)
        {
            EditorUtility.DisplayDialog("错误", "请选择两个资源", "确定");
            return;
        }

        string path1 = AssetDatabase.GetAssetPath(asset1);
        string path2 = AssetDatabase.GetAssetPath(asset2);

        if (string.IsNullOrEmpty(path1) || string.IsNullOrEmpty(path2))
        {
            EditorUtility.DisplayDialog("错误", "无效的资源路径", "确定");
            return;
        }

        string dir1 = Path.GetDirectoryName(path1);
        string dir2 = Path.GetDirectoryName(path2);
        string name1 = Path.GetFileNameWithoutExtension(path1);
        string name2 = Path.GetFileNameWithoutExtension(path2);
        string ext1 = Path.GetExtension(path1);
        string ext2 = Path.GetExtension(path2);

        // 根据设置决定是否交换扩展名
        string targetExt1 = swapExtensions ? ext2 : ext1;
        string targetExt2 = swapExtensions ? ext1 : ext2;

        try
        {
            // 执行交换
            AssetDatabase.RenameAsset(path1, name2 + targetExt1);
            AssetDatabase.RenameAsset(path2, name1 + targetExt2);

            AssetDatabase.Refresh();

            if (!keepOriginalSelection)
            {
                // 选中交换后的资源
                string finalPath1 = Path.Combine(dir1, name2 + targetExt1);
                string finalPath2 = Path.Combine(dir2, name1 + targetExt2);

                Object swappedAsset1 = AssetDatabase.LoadAssetAtPath<Object>(finalPath1);
                Object swappedAsset2 = AssetDatabase.LoadAssetAtPath<Object>(finalPath2);

                Selection.objects = new Object[] { swappedAsset1, swappedAsset2 };
            }

            Debug.Log($"成功交换: '{name1}' <-> '{name2}' (扩展名交换: {swapExtensions})");

            // 清除选择以便重新使用
            asset1 = null;
            asset2 = null;
            Repaint();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"交换失败: {e.Message}");
            EditorUtility.DisplayDialog("错误", $"交换失败: {e.Message}", "确定");
        }
    }
}

// 添加一个更简单的右键菜单版本（仅交换名称，不交换扩展名）
public class SimpleAssetNameSwapper : Editor
{
    [MenuItem("Assets/快速交换资源名称", true)]
    private static bool ValidateQuickSwap()
    {
        return Selection.objects.Length == 2;
    }

    [MenuItem("Assets/快速交换资源名称", false, 101)]
    private static void QuickSwapAssetNames()
    {
        Object[] selected = Selection.objects;
        if (selected.Length != 2) return;

        string path1 = AssetDatabase.GetAssetPath(selected[0]);
        string path2 = AssetDatabase.GetAssetPath(selected[1]);

        string name1 = Path.GetFileNameWithoutExtension(path1);
        string name2 = Path.GetFileNameWithoutExtension(path2);

        // 使用临时名称避免冲突
        string tempName = "_temp_swap_" + System.DateTime.Now.Ticks;

        // 执行交换
        AssetDatabase.RenameAsset(path1, tempName);
        AssetDatabase.RenameAsset(path2, name1);
        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(selected[0]), name2);

        AssetDatabase.Refresh();

        Debug.Log($"快速交换完成: '{name1}' <-> '{name2}'");
    }
}