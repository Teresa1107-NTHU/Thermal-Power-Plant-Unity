using System.Collections;
using UnityEngine;

public class PowerPlantSequenceController : MonoBehaviour
{
    void Start()
    {
        ResetPlant();
    }

    [System.Serializable]
    
    public class EffectStep
    {
        [Header("顯示用名稱")]
        public string stepName;

        [Header("分類名稱，例如 Fuel / Boiler / Steam / Cooling")]
        public string group;

        [Header("要啟動的物件")]
        public GameObject target;

        [Header("按下該步驟後，延遲幾秒啟動")]
        public float delayTime;
    }

    [Header("所有效果物件")]
    public EffectStep[] effectSteps;

    [Header("會旋轉的物件，例如 Shaft")]
    public GameObject[] rotatingObjects;

    [Header("燈泡控制器")]
    public LightbulbController lightbulbController;

    // 啟動某一類效果：會依照 delayTime 分批打開
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

    // 延遲後開啟單一效果
    private IEnumerator EnableEffectAfterDelay(EffectStep step)
    {
        yield return new WaitForSeconds(step.delayTime);

        if (step.target != null)
        {
            step.target.SetActive(true);
        }
    }

    public void StartFuel()
    {
        StartGroup("Fuel");
    }

    public void StartBoiler()
    {
        StartGroup("Boiler");
        StartGroup("Exhaust");
    }

    public void StartSteamCycle()
    {
        StartGroup("Steam");
        SetRotatingObjects(true);
    }

    public void StartCoolingSystem()
    {
        StartGroup("Cooling");
    }

    public void StartElectricity()
    {
        StartGroup("Electricity");
    }

    public void TurnOnLightbulb()
    {
        if (lightbulbController != null)
        {
            lightbulbController.TurnOn();
        }
    }

    public void ResetPlant()
    {
        StopAllCoroutines();

        foreach (EffectStep step in effectSteps)
        {
            if (step.target != null)
            {
                step.target.SetActive(false);
            }
        }

        SetRotatingObjects(false);

        if (lightbulbController != null)
        {
            lightbulbController.TurnOff();
        }
    }

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
}