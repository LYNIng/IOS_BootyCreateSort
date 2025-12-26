using UnityEngine;
using UnityEngine.UI;

public class FillAmountToPosition : MonoBehaviour
{
    public Image imaBar;
    public float radius = 50f;

    private void Start()
    {
        if (imaBar == null)
        {
            Debug.Log($"{gameObject.name} FillAmountToPosition ImaBar Unset CheckIt!");
            Destroy(this);
            return;
        }

        RefreshPos();
    }

    public void RefreshPos()
    {
        transform.position = GetFillAmountPosition(imaBar);
        var localPos = transform.localPosition;
        localPos.z = 0;
        transform.localPosition = localPos;
    }

    /// <summary>
    /// 根据Image的fillMethod和fillOrigin，计算fillAmount对应的世界坐标
    /// </summary>
    public Vector2 GetFillAmountPosition(Image image)
    {
        if (image.type != Image.Type.Filled)
        {
            Debug.LogWarning("Image类型不是Filled，无法计算fillAmount位置");
            return image.transform.position;
        }

        RectTransform rect = image.rectTransform;
        Vector2 localPos = Vector2.zero;

        switch (image.fillMethod)
        {
            case Image.FillMethod.Horizontal:
                localPos = GetHorizontalFillPosition(image, rect);
                break;
            case Image.FillMethod.Vertical:
                localPos = GetVerticalFillPosition(image, rect);
                break;
            case Image.FillMethod.Radial360:
                localPos = GetRadialFillPosition(image, rect);
                break;
            default:
                Debug.LogWarning("暂不支持此FillMethod类型: " + image.fillMethod);
                break;
        }

        // 将局部坐标转换为世界坐标（考虑父级缩放）
        return rect.TransformPoint(localPos);
    }

    // 水平填充（考虑fillOrigin和pivot）
    private Vector2 GetHorizontalFillPosition(Image image, RectTransform rect)
    {
        float fillWidth = rect.rect.width * image.fillAmount;
        float pivotOffsetX = rect.pivot.x * rect.rect.width;

        // 根据fillOrigin调整方向（0=左，1=右）
        float xPos = (image.fillOrigin == 0)
            ? fillWidth - pivotOffsetX                // 从左向右填充
            : rect.rect.width - fillWidth - pivotOffsetX; // 从右向左填充

        return new Vector2(xPos, 0);
    }

    // 垂直填充（考虑fillOrigin和pivot）
    private Vector2 GetVerticalFillPosition(Image image, RectTransform rect)
    {
        float fillHeight = rect.rect.height * image.fillAmount;
        float pivotOffsetY = rect.pivot.y * rect.rect.height;

        // 根据fillOrigin调整方向（0=下，1=上）
        float yPos = (image.fillOrigin == 0)
            ? fillHeight - pivotOffsetY                 // 从下向上填充
            : rect.rect.height - fillHeight - pivotOffsetY; // 从上向下填充

        return new Vector2(0, yPos);
    }

    private Vector2 GetRadialFillPosition(Image image, RectTransform rect)
    {
        // 计算实际半径（根据UI尺寸和缩放比例调整）
        float actualRadius = radius * Mathf.Min(rect.rect.width, rect.rect.height) * 0.5f;

        // 根据fillMethod和fillOrigin计算起始角度和范围
        float startAngle = 0f;
        float fillRange = 360f;
        GetAngleParameters(image.fillMethod, image.fillOrigin, out startAngle, out fillRange);

        // 计算当前角度（考虑顺时针方向）
        float angle = startAngle - fillRange * image.fillAmount;

        // 转换为局部坐标
        float rad = angle * Mathf.Deg2Rad;
        Vector2 localPos = new Vector2(
            Mathf.Sin(rad) * actualRadius,
            Mathf.Cos(rad) * actualRadius
        );

        // 修正锚点偏移
        localPos.x -= (rect.pivot.x - 0.5f) * rect.rect.width;
        localPos.y -= (rect.pivot.y - 0.5f) * rect.rect.height;

        return localPos;
    }

    /// <summary>
    /// 根据fillMethod和fillOrigin返回起始角度和填充范围
    /// </summary>
    private void GetAngleParameters(Image.FillMethod fillMethod, int fillOrigin,
                                 out float startAngle, out float fillRange)
    {
        fillRange = 360f;
        switch (fillMethod)
        {
            case Image.FillMethod.Radial180:
                fillRange = 180f;
                break;
            case Image.FillMethod.Radial90:
                fillRange = 90f;
                break;
        }

        // 根据fillOrigin设置起始角度（Unity的径向填充方向为顺时针）
        switch (fillOrigin)
        {
            case 0: startAngle = 0f; break; // 右
            case 1: startAngle = 90f; break; // 上
            case 2: startAngle = 180f; break; // 左
            case 3: startAngle = 270f; break; // 下
            default: startAngle = 0f; break;
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        if (imaBar == null) return;
        if (imaBar.type != Image.Type.Filled) return;
        var pos = GetFillAmountPosition(imaBar);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pos, 0.5f);
    }

#endif
}
