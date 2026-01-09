using UnityEngine;

public class AdvancedBuoyancy : MonoBehaviour
{
    [Header("波浪参数")]
    [SerializeField] private float waveHeight = 0.5f;
    [SerializeField] private float waveLength = 2f;
    [SerializeField] private float waveSpeed = 1f;

    [Header("物理参数")]
    [SerializeField] private float buoyancyForce = 1f;
    [SerializeField] private float damping = 0.1f;
    [SerializeField] private float waterDrag = 0.5f;

    [Header("噪音参数")]
    [SerializeField] private bool usePerlinNoise = true;
    [SerializeField] private float noiseScale = 0.1f;
    [SerializeField] private float noiseSpeed = 0.5f;

    private Vector3 velocity;
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private float noiseOffset;

    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition;
        noiseOffset = Random.Range(0f, 100f);
    }

    void FixedUpdate()
    {
        float time = Time.time;

        // 计算目标位置（包含波浪和噪音）
        Vector3 waveOffset = CalculateAdvancedWaveOffset(time);
        targetPosition = startPosition + waveOffset;

        // 物理模拟：弹簧-阻尼系统
        Vector3 displacement = targetPosition - transform.position;
        Vector3 springForce = displacement * buoyancyForce;

        // 添加阻尼
        springForce -= velocity * damping;

        // 添加水阻力
        springForce -= velocity * waterDrag;

        // 更新速度
        velocity += springForce * Time.fixedDeltaTime;

        // 更新位置
        transform.position += velocity * Time.fixedDeltaTime;
    }

    Vector3 CalculateAdvancedWaveOffset(float time)
    {
        Vector3 offset = Vector3.zero;

        // 使用多个正弦波叠加
        offset.x = Mathf.Sin(time * waveSpeed + startPosition.x * waveLength) * waveHeight;
        offset.y = Mathf.Cos(time * waveSpeed + startPosition.y * waveLength) * waveHeight;

        // 添加二次谐波
        offset.x += Mathf.Sin(time * waveSpeed * 2f + startPosition.x * waveLength * 2f) * (waveHeight * 0.3f);
        offset.y += Mathf.Cos(time * waveSpeed * 2f + startPosition.y * waveLength * 2f) * (waveHeight * 0.3f);

        // 添加柏林噪音
        if (usePerlinNoise)
        {
            float noiseTime = time * noiseSpeed + noiseOffset;
            float noiseX = Mathf.PerlinNoise(noiseTime, startPosition.x * noiseScale) * 2f - 1f;
            float noiseY = Mathf.PerlinNoise(startPosition.y * noiseScale, noiseTime) * 2f - 1f;

            offset.x += noiseX * waveHeight * 0.2f;
            offset.y += noiseY * waveHeight * 0.2f;
        }

        return offset;
    }
}