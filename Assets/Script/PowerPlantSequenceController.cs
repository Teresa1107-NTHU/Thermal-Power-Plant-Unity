// 控制整個天然氣發電廠展示流程，可用單一按鈕分段啟動天然氣、HRSG、蒸氣、冷卻、電力與燈泡效果

using System.Collections;
using UnityEngine;

public class PowerPlantSequenceController : MonoBehaviour
{
    [System.Serializable]
    public class EffectStep
    {
        [Header("顯示用名稱，方便辨識")]
        public string stepName;

        [Header("分類名稱：Gas / Boiler / Exhaust / Steam / Cooling / Electricity")]
        public string group;

        [Header("要啟動的物件，例如粒子、光源、LineRenderer、電流球")]
        public GameObject target;

        [Header("同一系統內延遲幾秒後啟動")]
        public float delayTime;
    }

    [Header("所有效果物件")]
    public EffectStep[] effectSteps;

    [Header("會旋轉的物件，例如 Shaft 或 Turbine")]
    public GameObject[] rotatingObjects;

    [Header("天然氣滑桿與燈泡控制")]
    public FuelPowerController fuelPowerController;

    // 啟動某一類效果，並依照 delayTime 分批出現
    public void StartGroup(string groupName)
    {
        foreach (EffectStep step in effectSteps)
        {
            if (step.target != null && step.group == groupName)
            {
                StartCoroutine(EnableEffectAfterDelay(step));
            }
        }
    }

    private IEnumerator EnableEffectAfterDelay(EffectStep step)
    {
        yield return new WaitForSeconds(step.delayTime);

        if (step.target != null)
        {
            step.target.SetActive(true);
        }
    }

    // Step 1：天然氣供應
    public void StartGas()
    {
        StartGroup("Gas");
    }

    // Step 2：HRSG / Boiler 與排氣
    public void StartBoiler()
    {
        StartGroup("Boiler");
        StartGroup("Exhaust");
    }

    // Step 3：蒸氣循環與轉軸
    public void StartSteamCycle()
    {
        StartGroup("Steam");
        SetRotatingObjects(true);
    }

    // Step 4：冷卻水循環
    public void StartCoolingSystem()
    {
        StartGroup("Cooling");
    }

    // Step 5：電力輸出
    public void StartElectricity()
    {
        StartGroup("Electricity");
    }

    // Step 6：燈泡亮起
    public void TurnOnLightbulb()
    {
        if (fuelPowerController != null)
        {
            fuelPowerController.ActivateLightbulb();
        }
    }

    // 重置整個電廠
    public void ResetPlant()
    {
        StopAllCoroutines();

        foreach (EffectStep step in effectSteps)
        {
            if (step.target != null)
            {
                step.target.SetActive(false);

                ParticleSystem ps = step.target.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }
        }

        SetRotatingObjects(false);

        if (fuelPowerController != null)
        {
            fuelPowerController.DeactivateLightbulb();
        }
    }

    // 控制旋轉物件上的 Rotate 腳本
    private void SetRotatingObjects(bool active)
    {
        foreach (GameObject obj in rotatingObjects)
        {
            if (obj != null)
            {
                Rotate rotate = obj.GetComponent<Rotate>();

                if (rotate != null)
                {
                    rotate.enabled = active;
                }
            }
        }
    }

    private void Start()
    {
        ResetPlant();
    }
}