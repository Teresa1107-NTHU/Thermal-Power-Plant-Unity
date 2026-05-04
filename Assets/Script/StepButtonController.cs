using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StepButtonController : MonoBehaviour
{
    [Header("主控制器")]
    public PowerPlantSequenceController controller;

    [Header("按鈕文字")]
    public TextMeshProUGUI buttonText;

    [Header("按鈕本身")]
    public Button button;

    [Header("每一步 Loading 時間")]
    public float fuelLoadingTime = 1.5f;
    public float boilerLoadingTime = 2.0f;
    public float steamLoadingTime = 2.0f;
    public float coolingLoadingTime = 1.5f;
    public float electricityLoadingTime = 2.0f;
    public float lightbulbLoadingTime = 1.0f;

    private int currentStep = 0;
    private bool isRunning = false;

    void Start()
    {
        UpdateButtonText();
    }

    // UI Button 的 OnClick 呼叫這個
    public void OnButtonClick()
    {
        if (isRunning) return;

        StartCoroutine(RunCurrentStep());
    }

    private IEnumerator RunCurrentStep()
    {
        isRunning = true;

        if (button != null)
        {
            button.interactable = false;
        }

        if (buttonText != null)
        {
            buttonText.text = "Loading...";
        }

        float waitTime = 1f;

        switch (currentStep)
        {
            case 0:
                controller.StartFuel();
                waitTime = fuelLoadingTime;
                break;

            case 1:
                controller.StartBoiler();
                waitTime = boilerLoadingTime;
                break;

            case 2:
                controller.StartSteamCycle();
                waitTime = steamLoadingTime;
                break;

            case 3:
                controller.StartCoolingSystem();
                waitTime = coolingLoadingTime;
                break;

            case 4:
                controller.StartElectricity();
                waitTime = electricityLoadingTime;
                break;

            case 5:
                controller.TurnOnLightbulb();
                waitTime = lightbulbLoadingTime;
                break;

            default:
                if (buttonText != null)
                {
                    buttonText.text = "Finished";
                }

                isRunning = false;

                if (button != null)
                {
                    button.interactable = false;
                }

                yield break;
        }

        yield return new WaitForSeconds(waitTime);

        currentStep++;

        isRunning = false;

        if (button != null)
        {
            button.interactable = true;
        }

        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        if (buttonText == null) return;

        switch (currentStep)
        {
            case 0:
                buttonText.text = "Start Fuel";
                break;

            case 1:
                buttonText.text = "Start Boiler";
                break;

            case 2:
                buttonText.text = "Start Steam Cycle";
                break;

            case 3:
                buttonText.text = "Start Cooling System";
                break;

            case 4:
                buttonText.text = "Start Electricity";
                break;

            case 5:
                buttonText.text = "Turn On Lightbulb";
                break;

            default:
                buttonText.text = "Finished";
                break;
        }
    }

    // 如果你之後想加 Reset 按鈕，可以呼叫這個
    public void ResetSteps()
    {
        currentStep = 0;
        isRunning = false;

        if (controller != null)
        {
            controller.ResetPlant();
        }

        if (button != null)
        {
            button.interactable = true;
        }

        UpdateButtonText();
    }
}