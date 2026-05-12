// 控制電流光點沿路徑移動，保留原本閃爍效果，並用 Gas Slider 調整移動速度

using System.Collections;
using UnityEngine;

public class MoveAlongPoints : MonoBehaviour
{
    public Transform[] points;

    [Header("Gas Slider 控制器")]
    public FuelPowerController fuelPowerController;

    [Header("電流速度")]
    public float speed = 2f;

    [Header("Gas = 0% 時速度")]
    public float minSpeed = 2f;

    [Header("Gas = 100% 時速度")]
    public float maxSpeed = 10f;

    public float startDelay = 0f;
    public bool loop = true;

    public float minScale = 0.04f;
    public float maxScale = 0.07f;
    public float flickerSpeed = 20f;

    private int currentIndex = 0;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(startDelay);

        transform.position = points[0].position;
        currentIndex = 1;
    }

    void Update()
    {
        if (points.Length < 2) return;

        // 根據 Gas Slider 更新 speed，但保留原本移動邏輯
        if (fuelPowerController != null)
        {
            float t = fuelPowerController.gasPercent / 100f;
            speed = Mathf.Lerp(minSpeed, maxSpeed, t);
        }

        Transform target = points[currentIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            currentIndex++;

            if (currentIndex >= points.Length)
            {
                transform.position = points[0].position;
                currentIndex = 1;
            }
        }

        float s = Mathf.Lerp(
            minScale,
            maxScale,
            (Mathf.Sin(Time.time * flickerSpeed) + 1f) * 0.5f
        );

        transform.localScale = new Vector3(s, s, s);
    }
}