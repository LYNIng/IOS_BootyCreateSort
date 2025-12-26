using UnityEngine;
using UnityEngine.UI;

public class ImageEffect : MonoBehaviour
{
    [SerializeField] private Image _image;
    public Image ImageComponent => _image ? _image : (_image = GetComponent<Image>());

    private Material _originalMaterial;
    private Material _effectMaterial;
    // 专门用于在 Mask 下作为最终渲染用的实例（包含 stencil 信息）
    private Material _renderMaterialInstance;

    private static readonly int GreyscaleBlendProperty = Shader.PropertyToID("_GreyscaleBlend");
    private bool _isEffectActive = false;

    // 添加一个标志来跟踪是否已经初始化
    private bool _isInitialized = false;

    private void Awake()
    {
        if (ImageComponent == null)
        {
            Debug.LogError($"Image component not found on {gameObject.name}", this);
            enabled = false;
            return;
        }

        // 保存原始 base 材质的引用（不要使用 materialForRendering）
        _originalMaterial = ImageComponent.material;

        _isInitialized = true;
    }

    private void OnEnable()
    {
        if (!_isInitialized || ImageComponent == null) return;

        // 如果已经有激活的效果材质，确保应用它
        if (_isEffectActive && _effectMaterial != null)
        {
            ImageComponent.material = _effectMaterial;
            // 强制刷新Canvas
            ForceRefreshCanvas();
        }
    }

    private void OnDisable()
    {
        // OnDisable中不恢复材质，避免不必要的刷新
    }

    private void OnDestroy()
    {
        CleanupEffectMaterial();
    }

    public void SetGreyscaleBlend(float value, bool forceRefresh = true)
    {
        if (!_isInitialized || ImageComponent == null) return;

        float clampedValue = Mathf.Clamp01(value);

        // 首次设置效果时创建材质（以 base material 为基础）
        if (!_isEffectActive || _effectMaterial == null)
        {
            // 优先使用 Image.material（base），避免复制带 stencil 的临时材质
            Material currentMaterial = ImageComponent.material ?? ImageComponent.materialForRendering;

            if (currentMaterial == null)
            {
                Debug.LogError("Image has no material to duplicate", this);
                return;
            }

            _effectMaterial = new Material(currentMaterial);
            _effectMaterial.name = $"{currentMaterial.name}_Effect";
            _isEffectActive = true;

            // 应用为 base material，Mask 系统会基于它生成渲染材质
            ImageComponent.material = _effectMaterial;

            //Debug.Log($"Created effect material: {_effectMaterial.name}");
        }

        // 设置属性到效果材质
        if (_effectMaterial != null && _effectMaterial.HasProperty(GreyscaleBlendProperty))
        {
            _effectMaterial.SetFloat(GreyscaleBlendProperty, clampedValue);

            // 确保 Image.material 指向我们的效果材质
            if (ImageComponent.material != _effectMaterial)
                ImageComponent.material = _effectMaterial;

            // 标记脏，触发 UI 系统更新（尝试触发 Mask 的 GetModifiedMaterial）
            ImageComponent.SetAllDirty();

            // 关键：在 Mask 下，materialForRendering 可能是一个临时带 stencil 的材质实例，它可能不会在属性变更时自动被重新创建并带上新的效果属性。
            // 所以这里基于当前 materialForRendering 创建/更新一个渲染用实例，并直接设置到 canvasRenderer 上，
            // 保证最终渲染材质包含我们设置的 _GreyscaleBlend。
            TryUpdateRenderMaterialInstance(clampedValue);

            if (forceRefresh)
            {
                ForceRefreshCanvas();
            }
        }
        else if (_effectMaterial != null)
        {
            Debug.LogError($"Effect material doesn't have property _GreyscaleBlend. Shader: {_effectMaterial.shader.name}");
        }
    }

    // 基于 materialForRendering 创建/更新最终渲染实例，并把需要的属性写上，然后直接应用到 CanvasRenderer
    private void TryUpdateRenderMaterialInstance(float greyscaleValue)
    {
        if (ImageComponent == null) return;

        var baseRenderMat = ImageComponent.materialForRendering;
        if (baseRenderMat == null)
            baseRenderMat = ImageComponent.material; // 兜底

        if (baseRenderMat == null) return;

        // 如果已有实例且 shader 与当前 materialForRendering 匹配，就重用实例，否则重新创建
        if (_renderMaterialInstance == null || _renderMaterialInstance.shader != baseRenderMat.shader)
        {
            // 清理旧的
            if (_renderMaterialInstance != null)
            {
                if (Application.isEditor && !Application.isPlaying) DestroyImmediate(_renderMaterialInstance);
                else Destroy(_renderMaterialInstance);
                _renderMaterialInstance = null;
            }

            // 创建基于 materialForRendering 的实例，保留 stencil、render queue 等信息
            _renderMaterialInstance = new Material(baseRenderMat)
            {
                name = $"{baseRenderMat.name}_RenderInstance"
            };
        }
        else
        {
            // 将 baseRenderMat 的属性拷贝到实例，确保 stencil 等被保留/更新
            _renderMaterialInstance.CopyPropertiesFromMaterial(baseRenderMat);
        }

        // 将我们的效果属性写到渲染实例上（若 shader 支持）
        if (_renderMaterialInstance.HasProperty(GreyscaleBlendProperty))
            _renderMaterialInstance.SetFloat(GreyscaleBlendProperty, greyscaleValue);

        // 直接提交给 CanvasRenderer，保证最终渲染使用包含 stencil 与效果属性的材质
        var canvasRenderer = ImageComponent.canvasRenderer;
        if (canvasRenderer != null)
        {
            canvasRenderer.SetMaterial(_renderMaterialInstance, ImageComponent.mainTexture);
        }
    }

    // 改进的刷新方法：尽量在短时间内触发 UI 更新（注意：Canvas.ForceUpdateCanvases 性能开销较大）
    private void ForceRefreshCanvas()
    {
        if (ImageComponent == null) return;

        // 标记脏（包括材质、顶点和布局）
        ImageComponent.SetAllDirty();

        // 立即触发 Canvas 重建（强制）
        Canvas.ForceUpdateCanvases();

        // 触发 Graphic 的 Rebuild，以确保 UpdateMaterial 被调用（PreRender 阶段）
        ImageComponent.Rebuild(CanvasUpdate.PreRender);

        // 在下一帧再一次简单标记，确保无残留
        StartCoroutine(ForceRefreshNextFrame());
    }

    private System.Collections.IEnumerator ForceRefreshNextFrame()
    {
        yield return null; // 等待一帧

        if (ImageComponent != null)
        {
            ImageComponent.SetMaterialDirty();
            ImageComponent.SetVerticesDirty();
        }
    }

    // 重置方法
    public void ResetToOriginalMaterial()
    {
        if (!_isInitialized || ImageComponent == null) return;

        _isEffectActive = false;

        if (_originalMaterial != null)
        {
            ImageComponent.material = _originalMaterial;
        }
        else
        {
            // 如果没有原始材质，使用默认UI材质
            ImageComponent.material = null;
        }

        CleanupEffectMaterial();
        ForceRefreshCanvas();
    }

    private void CleanupEffectMaterial()
    {
        // 回收并恢复 canvasRenderer 使用的渲染实例
        if (_renderMaterialInstance != null)
        {
            // 如果当前 canvasRenderer 正在使用该实例，先切回原始渲染材质
            if (ImageComponent != null)
            {
                var canvasRenderer = ImageComponent.canvasRenderer;
                if (canvasRenderer != null)
                {
                    // 恢复到 materialForRendering 或 base material
                    var restoreMat = ImageComponent.materialForRendering ?? ImageComponent.material;
                    canvasRenderer.SetMaterial(restoreMat, ImageComponent.mainTexture);
                }
            }

            if (Application.isEditor && !Application.isPlaying) DestroyImmediate(_renderMaterialInstance);
            else Destroy(_renderMaterialInstance);

            _renderMaterialInstance = null;
        }

        if (_effectMaterial != null)
        {
            // 确保不再被引用
            if (ImageComponent != null && ImageComponent.material == _effectMaterial)
            {
                ImageComponent.material = _originalMaterial;
            }

            if (Application.isEditor && !Application.isPlaying)
                DestroyImmediate(_effectMaterial);
            else
                Destroy(_effectMaterial);

            _effectMaterial = null;
        }
    }

    // 添加一个方法来检查Shader属性
    [ContextMenu("Check Shader Properties")]
    public void CheckShaderProperties()
    {
        if (ImageComponent == null || ImageComponent.material == null) return;

        var material = ImageComponent.material;
        Debug.Log($"Current Material: {material.name}");
        Debug.Log($"Shader: {material.shader.name}");

        // 列出所有属性
        int propertyCount = material.shader.GetPropertyCount();
        for (int i = 0; i < propertyCount; i++)
        {
            var propertyName = material.shader.GetPropertyName(i);
            Debug.Log($"Property {i}: {propertyName}");
        }

        // 检查是否有_GreyscaleBlend属性
        if (material.HasProperty(GreyscaleBlendProperty))
        {
            Debug.Log($"Has _GreyscaleBlend property: {material.GetFloat(GreyscaleBlendProperty)}");
        }
        else
        {
            Debug.LogWarning("Material doesn't have _GreyscaleBlend property!");
        }
    }

    // 调试方法
    [ContextMenu("Debug Current State")]
    public void DebugCurrentState()
    {
        Debug.Log($"Effect Active: {_isEffectActive}");
        Debug.Log($"Image Material: {ImageComponent?.material?.name}");
        Debug.Log($"Effect Material: {_effectMaterial?.name}");
        Debug.Log($"Original Material: {_originalMaterial?.name}");

        if (ImageComponent?.material != null && ImageComponent.material.HasProperty(GreyscaleBlendProperty))
        {
            Debug.Log($"Greyscale Value: {ImageComponent.material.GetFloat(GreyscaleBlendProperty)}");
        }

        if (_renderMaterialInstance != null)
        {
            Debug.Log($"Render Instance: {_renderMaterialInstance.name}, Shader: {_renderMaterialInstance.shader.name}");
            if (_renderMaterialInstance.HasProperty(GreyscaleBlendProperty))
                Debug.Log($"RenderInstance Greyscale: {_renderMaterialInstance.GetFloat(GreyscaleBlendProperty)}");
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_image == null)
            _image = GetComponent<Image>();
    }
#endif
}