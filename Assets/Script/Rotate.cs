// 控制物件旋轉，並可依照 Gas Slider 數值調整旋轉速度

using UnityEngine;

public class Rotate : MonoBehaviour
{
    [Header("Gas Slider 控制器")]
    public FuelPowerController fuelPowerController;

    [Header("最低旋轉速度")]
    public Vector3 minSpeed = new Vector3(0f, 0f, 0f);

    [Header("最高旋轉速度")]
    public Vector3 maxSpeed = new Vector3(0f, 500f, 0f);

    void Update()
    {
        float t = 1f;

        if (fuelPowerController != null)
        {
            t = fuelPowerController.gasPercent / 100f;
        }

        Vector3 currentSpeed = Vector3.Lerp(
            minSpeed,
            maxSpeed,
            t
        );

        transform.Rotate(currentSpeed * Time.deltaTime);
    }
}