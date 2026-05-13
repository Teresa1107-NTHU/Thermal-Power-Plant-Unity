using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        [Header("這一步要啟動的系統")]
        public string actionName;
    }

    [Header("主相機")]
    public Camera mainCamera;

    [Header("相機滑鼠控制")]
    public TourCameraLook cameraLook;

    [Header("電廠流程控制器")]
    public PowerPlantSequenceController powerPlantController;

    [Header("導覽步驟")]
    public TourStep[] steps;

    [Header("UI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public TextMeshProUGUI nextButtonText;

    public Button previousButton;
    public Button nextButton;

    [Header("相機移動時間")]
    public float moveDuration = 2f;

    private int currentIndex = -1;

    private Coroutine moveRoutine;

    [Header("初始相機位置")]
    public Transform startCameraPoint;

    void Start()
    {
        ResetTour();
    }

    [Header("控制滑桿")]
    public Slider controlSlider;

    public void NextStep()
    {
        if (steps.Length == 0)
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
        if (steps.Length == 0)
            return;

        currentIndex--;

        if (currentIndex < 0)
        {
            currentIndex = 0;
        }

        ShowStep(currentIndex);
    }

    public void GoToStep(int index)
    {
        if (index < 0 || index >= steps.Length)
            return;

        currentIndex = index;

        ShowStep(currentIndex);
    }

    public void ResetTour()
    {
        currentIndex = -1;

        if (powerPlantController != null)
        {
            powerPlantController.ResetPlant();
        }

        titleText.text = "Natural Gas Power Plant";

        bodyText.text =
            "Press Start to begin the guided tour.";

        nextButtonText.text = "Start";

        previousButton.interactable = false;

        // Slider 回到 50%
        if (controlSlider != null)
        {
            controlSlider.value = 50f;
        }

        // 回到初始畫面
        if (startCameraPoint != null)
        {
            if (moveRoutine != null)
            {
                StopCoroutine(moveRoutine);
            }

            moveRoutine =
                StartCoroutine(
                    MoveCamera(startCameraPoint)
                );
        }
    }

    public void ShowStep(int index)
    {
        TourStep step = steps[index];

        // 更新 UI
        titleText.text = step.title;
        bodyText.text = step.description;

        previousButton.interactable = index > 0;

        nextButtonText.text =
            index >= steps.Length - 1
            ? "Finished"
            : "Next";

        // 執行設備動畫
        RunStepAction(step.actionName);

        // 停止舊移動
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }

        // 開始新的平滑移動
        moveRoutine =
            StartCoroutine(
                MoveCamera(step.cameraPoint)
            );
    }

    private IEnumerator MoveCamera(Transform targetPoint)
    {
        Vector3 startPos =
            mainCamera.transform.position;

        Quaternion startRot =
            mainCamera.transform.rotation;

        Vector3 targetPos =
            targetPoint.position;

        Quaternion targetRot =
            targetPoint.rotation;

        float timer = 0f;

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;

            float t = timer / moveDuration;

            // 更平滑
            t = Mathf.SmoothStep(0f, 1f, t);

            mainCamera.transform.position =
                Vector3.Lerp(
                    startPos,
                    targetPos,
                    t
                );

            mainCamera.transform.rotation =
                Quaternion.Slerp(
                    startRot,
                    targetRot,
                    t
                );

            yield return null;
        }

        // 最終位置
        mainCamera.transform.position =
            targetPos;

        mainCamera.transform.rotation =
            targetRot;

        // 同步滑鼠控制角度
        if (cameraLook != null)
        {
            cameraLook.SyncRotation();
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
}