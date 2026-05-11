// 控制電流物件沿著指定路徑移動，並依照 Gas Slider 動態調整電流速度與閃爍大小

using System.Collections;
using UnityEngine;

public class MoveAlongPoints : MonoBehaviour
{
    [Header("移動路徑點")]
    public Transform[] points;

    [Header("Gas Slider 控制器")]
    public FuelPowerController fuelPowerController;

    [Header("電流速度範圍")]
    public float minSpeed = 0f;
    public float maxSpeed = 10f;

    [Header("啟動延遲")]
    public float startDelay = 0f;

    [Header("是否循環")]
    public bool loop = true;

    [Header("電流閃爍大小")]
    public float minScale = 0f;
    public float maxScale = 0.07f;

    [Header("閃爍速度")]
    public float flickerSpeed = 20f;

    private int currentIndex = 0;

    private float currentSpeed = 2f;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(startDelay);

        if (points.Length > 0)
        {
            transform.position = points[0].position;
        }

        currentIndex = 1;
    }

    void Update()
    {
        if (points == null || points.Length < 2)
            return;

        UpdateElectricitySpeed();

        Transform target = points[currentIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            currentSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            currentIndex++;

            if (currentIndex >= points.Length)
            {
                if (loop)
                {
                    transform.position = points[0].position;
                    currentIndex = 1;
                }
                else
                {
                    enabled = false;
                }
            }
        }

        // 電流閃爍大小效果
        float s = Mathf.Lerp(
            minScale,
            maxScale,
            (Mathf.Sin(Time.time * flickerSpeed) + 1f) * 0.5f
        );

        transform.localScale = new Vector3(s, s, s);
    }

    // 根據天然氣 Slider 調整電流速度
    private void UpdateElectricitySpeed()
    {
        if (fuelPowerController == null)
        {
            currentSpeed = minSpeed;
            return;
        }

        float t = fuelPowerController.gasPercent / 100f;

        currentSpeed = Mathf.Lerp(
            minSpeed,
            maxSpeed,
            t
        );
    }
}