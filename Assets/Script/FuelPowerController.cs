// 控制天然氣滑桿數值，並用滑桿控制天然氣流量、HRSG火焰、各段蒸氣/煙霧流量，以及燈泡亮度

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FuelPowerController : MonoBehaviour
{
    [System.Serializable]
    public class ParticleRateControl
    {
        [Header("方便辨識用名稱")]
        public string name;

        [Header("要控制的 Particle System")]
        public ParticleSystem particle;

        [Header("Gas = 0% 時的 Emission Rate")]
        public float minRate = 0f;

        [Header("Gas = 100% 時的 Emission Rate")]
        public float maxRate = 100f;
    }

    [System.Serializable]
    public class ElectricLineControl
    {
        [Header("方便辨識用名稱")]
        public string name;

        [Header("要控制的 Line Renderer")]
        public LineRenderer lineRenderer;

        [Header("Gas = 0% 時的線寬")]
        public float minWidth = 0.02f;

        [Header("Gas = 100% 時的線寬")]
        public float maxWidth = 0.08f;

        [Header("電線發光材質")]
        public Material lineMaterial;

        [Header("Gas = 100% 時的 Emission 強度")]
        public float maxEmissionIntensity = 3f;

        [Header("發光顏色")]
        public Color emissionColor = new Color(1f, 0.85f, 0.3f);
    }

    [Header("UI 元件")]
    public Slider gasSlider;
    public TextMeshProUGUI gasValueText;

    [Header("目前天然氣輸入百分比")]
    [Range(0f, 100f)]
    public float gasPercent = 50f;

    [Header("燈泡控制")]
    public Renderer bulbRenderer;
    public Material glassMaterial;
    public Material lightOnMaterial;
    public Light bulbLight;

    [Header("燈泡亮度範圍")]
    public float minLightIntensity = 0f;
    public float maxLightIntensity = 5f;
    public float maxEmissionIntensity = 5f;

    [Header("燈泡發光顏色")]
    public Color emissionColor = new Color(1f, 0.85f, 0.4f);

    [Header("天然氣流動 Particle")]
    public ParticleSystem naturalGasFlow;

    [Header("天然氣流量範圍")]
    public float minGasEmissionRate = 0f;
    public float maxGasEmissionRate = 60f;

    [Header("HRSG 火焰 Particle")]
    public ParticleSystem hrsgFire;

    [Header("HRSG 火焰 Rate over Time 範圍")]
    public float minFireEmissionRate = 0f;
    public float maxFireEmissionRate = 300f;

    [Header("各段蒸氣 / 煙霧 / 水氣 Emission 控制")]
    public ParticleRateControl[] particleRateControls;

    [Header("電流 LineRenderer 控制")]
    public ElectricLineControl[] electricLineControls;

    private bool lightbulbActivated = false;

    void Start()
    {
        if (gasSlider != null)
        {
            gasSlider.minValue = 0f;
            gasSlider.maxValue = 100f;
            gasSlider.value = gasPercent;
            gasSlider.onValueChanged.AddListener(OnGasSliderChanged);
        }

        UpdateGasUI();
        UpdateNaturalGasFlow();
        UpdateHRSGFire();
        UpdateControlledParticles();
        DeactivateLightbulb();
        UpdateElectricLines();
    }

    public void OnGasSliderChanged(float value)
    {
        gasPercent = value;

        UpdateGasUI();
        UpdateNaturalGasFlow();
        UpdateHRSGFire();
        UpdateControlledParticles();
        UpdateElectricLines();

        if (lightbulbActivated)
        {
            UpdateLightbulbBrightness();
        }
    }

    public void ActivateLightbulb()
    {
        lightbulbActivated = true;
        UpdateLightbulbBrightness();
    }

    public void DeactivateLightbulb()
    {
        lightbulbActivated = false;
        TurnOffLightbulbVisual();
    }

    private void UpdateGasUI()
    {
        if (gasValueText != null)
        {
            gasValueText.text = "Gas Input: " + Mathf.RoundToInt(gasPercent) + "%";
        }
    }

    private void UpdateNaturalGasFlow()
    {
        if (naturalGasFlow == null)
            return;

        var emission = naturalGasFlow.emission;

        emission.rateOverTime = Mathf.Lerp(
            minGasEmissionRate,
            maxGasEmissionRate,
            gasPercent / 100f
        );
    }

    private void UpdateHRSGFire()
    {
        if (hrsgFire == null)
            return;

        var emission = hrsgFire.emission;

        emission.rateOverTime = Mathf.Lerp(
            minFireEmissionRate,
            maxFireEmissionRate,
            gasPercent / 100f
        );
    }

    private void UpdateControlledParticles()
    {
        if (particleRateControls == null)
            return;

        float t = gasPercent / 100f;

        foreach (ParticleRateControl item in particleRateControls)
        {
            if (item == null || item.particle == null)
                continue;

            var emission = item.particle.emission;

            emission.rateOverTime = Mathf.Lerp(
                item.minRate,
                item.maxRate,
                t
            );
        }
    }

    private void UpdateLightbulbBrightness()
    {
        float normalizedGas = gasPercent / 100f;

        if (gasPercent <= 0.01f)
        {
            TurnOffLightbulbVisual();
            return;
        }

        if (bulbRenderer != null && lightOnMaterial != null)
        {
            bulbRenderer.material = lightOnMaterial;
        }

        if (bulbLight != null)
        {
            bulbLight.intensity = Mathf.Lerp(
                minLightIntensity,
                maxLightIntensity,
                normalizedGas
            );
        }

        if (lightOnMaterial != null)
        {
            float emissionIntensity = Mathf.Lerp(
                0f,
                maxEmissionIntensity,
                normalizedGas
            );

            lightOnMaterial.SetColor(
                "_EmissionColor",
                emissionColor * emissionIntensity
            );
        }
    }

    private void TurnOffLightbulbVisual()
    {
        if (bulbRenderer != null && glassMaterial != null)
        {
            bulbRenderer.material = glassMaterial;
        }

        if (bulbLight != null)
        {
            bulbLight.intensity = 0f;
        }

        if (lightOnMaterial != null)
        {
            lightOnMaterial.SetColor("_EmissionColor", Color.black);
        }
    }

    private void UpdateElectricLines()
    {
        if (electricLineControls == null)
            return;

        float t = gasPercent / 100f;

        foreach (ElectricLineControl item in electricLineControls)
        {
            if (item == null || item.lineRenderer == null)
                continue;

            float width = Mathf.Lerp(item.minWidth, item.maxWidth, t);

            item.lineRenderer.startWidth = width;
            item.lineRenderer.endWidth = width;

            if (item.lineMaterial != null)
            {
                Color finalEmission = item.emissionColor * Mathf.Lerp(0f, item.maxEmissionIntensity, t);
                item.lineMaterial.SetColor("_EmissionColor", finalEmission);
            }
        }
    }
}