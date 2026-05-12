// 控制天然氣發電廠導覽流程，讓相機依序移動到設備、更新介紹文字，並啟動對應發電效果

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GuidedTourController : MonoBehaviour
{
    [System.Serializable]
    public class TourStep
    {
        [Header("步驟標題")]
        public string title;

        [Header("介紹文字")]
        [TextArea(3, 8)]
        public string description;

        [Header("相機位置")]
        public Transform cameraPoint;

        [Header("這一步要啟動的系統：Gas / Boiler / Steam / Cooling / Electricity / Lightbulb")]
        public string actionName;
    }

    [Header("主相機")]
    public Camera mainCamera;

    [Header("電廠流程控制器")]
    public PowerPlantSequenceController powerPlantController;

    [Header("導覽步驟")]
    public TourStep[] steps;

    [Header("UI 文字")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public TextMeshProUGUI nextButtonText;

    [Header("UI 按鈕")]
    public Button previousButton;
    public Button nextButton;

    [Header("相機移動時間")]
    public float moveDuration = 1.5f;

    private int currentIndex = -1;
    private Coroutine moveRoutine;

    void Start()
    {
        ResetTour();
    }

    public void NextStep()
    {
        if (steps == null || steps.Length == 0)
            return;

        currentIndex++;

        if (currentIndex >= steps.Length)
        {
            currentIndex = steps.Length - 1;
            return;
        }

        ShowStep(currentIndex);
    }

    public void PreviousStep()
    {
        if (steps == null || steps.Length == 0)
            return;

        currentIndex--;

        if (currentIndex < 0)
        {
            currentIndex = 0;
        }

        ShowStep(currentIndex);
    }

    public void ResetTour()
    {
        currentIndex = -1;

        if (powerPlantController != null)
        {
            powerPlantController.ResetPlant();
        }

        if (titleText != null)
        {
            titleText.text = "Natural Gas Power Plant";
        }

        if (bodyText != null)
        {
            bodyText.text = "Press Start to begin the guided tour.";
        }

        if (nextButtonText != null)
        {
            nextButtonText.text = "Start";
        }

        if (previousButton != null)
        {
            previousButton.interactable = false;
        }

        if (nextButton != null)
        {
            nextButton.interactable = true;
        }
    }

    private void ShowStep(int index)
    {
        TourStep step = steps[index];

        if (titleText != null)
        {
            titleText.text = step.title;
        }

        if (bodyText != null)
        {
            bodyText.text = step.description;
        }

        if (previousButton != null)
        {
            previousButton.interactable = index > 0;
        }

        if (nextButtonText != null)
        {
            nextButtonText.text = index >= steps.Length - 1 ? "Finished" : "Next";
        }

        if (nextButton != null)
        {
            nextButton.interactable = index < steps.Length - 1;
        }

        RunStepAction(step.actionName);

        if (step.cameraPoint != null && mainCamera != null)
        {
            if (moveRoutine != null)
            {
                StopCoroutine(moveRoutine);
            }

            moveRoutine = StartCoroutine(MoveCamera(step.cameraPoint));
        }
    }

    private void RunStepAction(string actionName)
    {
        if (powerPlantController == null)
            return;

        switch (actionName)
        {
            case "Gas":
                powerPlantController.StartGas();
                break;

            case "Boiler":
                powerPlantController.StartBoiler();
                break;

            case "Steam":
                powerPlantController.StartSteamCycle();
                break;

            case "Cooling":
                powerPlantController.StartCoolingSystem();
                break;

            case "Electricity":
                powerPlantController.StartElectricity();
                break;

            case "Lightbulb":
                powerPlantController.TurnOnLightbulb();
                break;
        }
    }

    private IEnumerator MoveCamera(Transform targetPoint)
    {
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;

        Vector3 targetPosition = targetPoint.position;
        Quaternion targetRotation = targetPoint.rotation;

        float timer = 0f;

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;

            float t = timer / moveDuration;
            t = t * t * (3f - 2f * t);

            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;
    }
}