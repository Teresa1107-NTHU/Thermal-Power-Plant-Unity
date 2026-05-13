// 控制天然氣輸入滑桿、燈泡亮度、Emission 發光、天然氣流動、蒸氣流動與 HRSG 火焰效果

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FuelPowerController : MonoBehaviour
{
    // =========================
    // UI 元件
    // =========================

    [Header("天然氣滑桿")]
    public Slider gasSlider;

    [Header("顯示天然氣百分比文字")]
    public TextMeshProUGUI gasValueText;

    // =========================
    // 天然氣輸入百分比
    // =========================

    [Header("目前天然氣輸入百分比")]
    [Range(0f, 100f)]
    public float gasPercent = 50f;

    // =========================
    // 燈泡控制
    // =========================

    [Header("燈泡 Renderer")]
    public Renderer bulbRenderer;

    [Header("燈泡未通電時的玻璃材質")]
    public Material glassMaterial;

    [Header("燈泡通電時的發光材質")]
    public Material lightOnMaterial;

    [Header("燈泡 Point Light")]
    public Light bulbLight;

    // 執行時複製出來的材質
    // 避免直接修改原本 Material Asset
    private Material runtimeLightMaterial;

    // =========================
    // 燈泡亮度設定
    // =========================

    [Header("燈泡最小亮度")]
    public float minLightIntensity = 0f;

    [Header("燈泡最大亮度")]
    public float maxLightIntensity = 5f;

    [Header("Emission 最亮強度")]
    public float maxEmissionIntensity = 5f;

    // =========================
    // 燈泡發光顏色
    // =========================

    [Header("燈泡發光顏色")]
    public Color emissionColor = new Color(1f, 0.85f, 0.4f);

    // =========================
    // 天然氣流動 Particle
    // =========================

    [Header("天然氣流動 Particle")]
    public ParticleSystem naturalGasFlow;

    [Header("天然氣最小流量")]
    public float minGasEmissionRate = 0f;

    [Header("天然氣最大流量")]
    public float maxGasEmissionRate = 60f;

    // =========================
    // 蒸氣流動 Particle
    // =========================

    [Header("蒸氣 Particle")]
    public ParticleSystem[] steamFlows;

    [Header("蒸氣最小流量")]
    public float minSteamEmissionRate = 0f;

    [Header("蒸氣最大流量")]
    public float maxSteamEmissionRate = 150f;

    // =========================
    // 冷卻水循環 Particle
    // =========================

    [Header("冷卻水 Particle")]
    public ParticleSystem[] coolingWaterFlows;

    [Header("冷卻水最小流量")]
    public float minCoolingEmissionRate = 0f;

    [Header("冷卻水最大流量")]
    public float maxCoolingEmissionRate = 120f;

    // =========================
    // HRSG 火焰 Particle
    // =========================

    [Header("HRSG 火焰 Particle")]
    public ParticleSystem hrsgFire;

    [Header("火焰最小噴發量")]
    public float minFireEmissionRate = 0f;

    [Header("火焰最大噴發量")]
    public float maxFireEmissionRate = 300f;

    // =========================
    // 是否允許燈泡亮起
    // 只有按下 Turn On Lightbulb 後才會變 true
    // =========================

    private bool lightbulbActivated = false;

    // =========================
    // Start
    // 遊戲開始時初始化
    // =========================

    void Start()
    {
        // 複製一份燈泡材質
        // 避免直接修改原本 Material Asset
        if (lightOnMaterial != null)
        {
            runtimeLightMaterial = new Material(lightOnMaterial);
        }

        // 初始化 Slider
        if (gasSlider != null)
        {
            gasSlider.minValue = 0f;
            gasSlider.maxValue = 100f;

            // 初始值
            gasSlider.value = gasPercent;

            // 監聽 Slider 變化
            gasSlider.onValueChanged.AddListener(OnGasSliderChanged);
        }

        // 更新 UI
        UpdateGasUI();

        // 關閉燈泡
        TurnOffLightbulbVisual();

        // 更新天然氣流量
        UpdateNaturalGasFlow();

        // 更新蒸氣流量
        UpdateSteamFlow();

        // 更新冷卻水流量
        UpdateCoolingWaterFlow();

        // 更新火焰流量
        UpdateHRSGFire();
    }

    // =========================
    // Slider 被拖曳時
    // =========================

    public void OnGasSliderChanged(float value)
    {
        // 更新天然氣百分比
        gasPercent = value;

        // 更新 UI
        UpdateGasUI();

        // 如果燈泡已經啟動
        // 才允許調整亮度
        if (lightbulbActivated)
        {
            UpdateLightbulbBrightness();
        }

        // 更新天然氣流量
        UpdateNaturalGasFlow();

        // 更新蒸氣流量
        UpdateSteamFlow();

        // 更新冷卻水流量
        UpdateCoolingWaterFlow();

        // 更新火焰
        UpdateHRSGFire();
    }

    // =========================
    // 更新天然氣百分比文字
    // =========================

    private void UpdateGasUI()
    {
        if (gasValueText != null)
        {
            gasValueText.text =
                "Gas Input: " +
                Mathf.RoundToInt(gasPercent) +
                "%";
        }
    }

    // =========================
    // 外部呼叫
    // 啟動燈泡
    // =========================

    public void ActivateLightbulb()
    {
        // 允許燈泡亮起
        lightbulbActivated = true;

        // 更新燈泡亮度
        UpdateLightbulbBrightness();
    }

    // =========================
    // 外部呼叫
    // 關閉燈泡
    // =========================
    public void DeactivateLightbulb()
    {
        // 禁止燈泡亮起
        lightbulbActivated = false;

        // 關閉燈泡外觀
        TurnOffLightbulbVisual();
    }

    // =========================
    // 更新燈泡亮度
    // =========================

    private void UpdateLightbulbBrightness()
    {
        // 正規化天然氣百分比
        // 例如 50% -> 0.5
        float normalizedGas = gasPercent / 100f;

        // 如果天然氣接近 0
        // 關燈
        if (gasPercent <= 0.01f)
        {
            TurnOffLightbulbVisual();
            return;
        }

        // 切換成發光材質
        if (bulbRenderer != null && runtimeLightMaterial != null)
        {
            bulbRenderer.material = runtimeLightMaterial;
        }

        // 控制 Point Light 強度
        if (bulbLight != null)
        {
            bulbLight.intensity = Mathf.Lerp(
                minLightIntensity,
                maxLightIntensity,
                normalizedGas
            );
        }

        // 控制 Emission 強度
        if (runtimeLightMaterial != null)
        {
            float emissionIntensity = Mathf.Lerp(
                0f,
                maxEmissionIntensity,
                normalizedGas
            );

            runtimeLightMaterial.SetColor(
                "_EmissionColor",
                emissionColor * emissionIntensity
            );
        }
    }

    // =========================
    // 關閉燈泡
    // =========================

    private void TurnOffLightbulbVisual()
    {
        // 切回玻璃材質
        if (bulbRenderer != null && glassMaterial != null)
        {
            bulbRenderer.material = glassMaterial;
        }

        // 關閉 Point Light
        if (bulbLight != null)
        {
            bulbLight.intensity = 0f;
        }
    }

    // =========================
    // 更新天然氣流量
    // =========================

    private void UpdateNaturalGasFlow()
    {
        if (naturalGasFlow == null)
            return;

        // 取得 Emission 模組
        var emission = naturalGasFlow.emission;

        // 根據 Slider 調整流量
        emission.rateOverTime = Mathf.Lerp(
            minGasEmissionRate,
            maxGasEmissionRate,
            gasPercent / 100f
        );
    }

    // =========================
    // 更新蒸氣流量
    // =========================
    private void UpdateSteamFlow()
    {
        // 如果沒有設定 Particle
        if (steamFlows == null)
            return;

        // 逐一控制每條蒸氣
        foreach (ParticleSystem steamFlow in steamFlows)
        {
            if (steamFlow == null)
                continue;

            // 取得 Emission 模組
            var emission = steamFlow.emission;

            // 根據 Slider 調整流量
            emission.rateOverTime = Mathf.Lerp(
                minSteamEmissionRate,
                maxSteamEmissionRate,
                gasPercent / 100f
            );
        }
    }

    // =========================
    // 更新冷卻水流量
    // =========================

    private void UpdateCoolingWaterFlow()
    {
        // 如果沒有設定 Particle
        if (coolingWaterFlows == null)
            return;

        // 逐一控制每條水流
        foreach (ParticleSystem waterFlow in coolingWaterFlows)
        {
            if (waterFlow == null)
                continue;

            // 取得 Emission 模組
            var emission = waterFlow.emission;

            // 根據 Slider 調整流量
            emission.rateOverTime = Mathf.Lerp(
                minCoolingEmissionRate,
                maxCoolingEmissionRate,
                gasPercent / 100f
            );
        }
    }

    // =========================
    // 更新 HRSG 火焰
    // =========================

    private void UpdateHRSGFire()
    {
        if (hrsgFire == null)
            return;

        // 取得 Emission 模組
        var emission = hrsgFire.emission;

        // 根據 Slider 調整火焰大小
        emission.rateOverTime = Mathf.Lerp(
            minFireEmissionRate,
            maxFireEmissionRate,
            gasPercent / 100f
        );
    }

    // =========================
    // 重置整個系統
    // =========================

    public void ResetFuelSystem()
    {
        // 關閉燈泡
        lightbulbActivated = false;

        // Slider 回到 0
        gasPercent = 0f;

        // 更新 Slider
        if (gasSlider != null)
        {
            gasSlider.value = 0f;
        }

        // 更新 UI
        UpdateGasUI();

        // 關燈
        TurnOffLightbulbVisual();

        // 更新天然氣
        UpdateNaturalGasFlow();

        // 更新蒸氣
        UpdateSteamFlow();

        // 更新冷卻水流量
        UpdateCoolingWaterFlow();

        // 更新火焰
        UpdateHRSGFire();
    }
}