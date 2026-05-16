// 引入 Unity 常用功能
using System.Collections;

// 引入 TextMeshPro UI 功能
using TMPro;

// 引入 Unity 核心功能
using UnityEngine;

// 引入 UI 功能（Button、Slider 等）
using UnityEngine.UI;

public class GuidedTourController : MonoBehaviour
{
    // =========================
    // 每一個導覽步驟的資料結構
    // =========================
    [System.Serializable]
    public class TourStep
    {
        [Header("步驟標題")]
        public string title;

        [Header("介紹文字")]
        [TextArea(3, 8)]
        public string description;

        [Header("相機移動目標位置")]
        public Transform cameraPoint;

        [Header("這一步要啟動的系統")]
        public string actionName;
    }

    // =========================
    // 主相機
    // =========================
    [Header("主相機")]
    public Camera mainCamera;

    // =========================
    // 相機滑鼠控制腳本
    // =========================
    [Header("相機滑鼠控制")]
    public TourCameraLook cameraLook;

    // =========================
    // 發電廠流程控制器
    // =========================
    [Header("電廠流程控制器")]
    public PowerPlantSequenceController powerPlantController;

    // =========================
    // 導覽步驟陣列
    // =========================
    [Header("導覽步驟")]
    public TourStep[] steps;

    // =========================
    // UI 文字
    // =========================
    [Header("UI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public TextMeshProUGUI nextButtonText;

    // =========================
    // UI 按鈕
    // =========================
    public Button previousButton;
    public Button nextButton;

    // =========================
    // 相機移動時間
    // =========================
    [Header("相機移動時間")]
    public float moveDuration = 2f;

    // =========================
    // 目前導覽索引
    // -1 代表尚未開始
    // =========================
    private int currentIndex = -1;

    // =========================
    // Coroutine 參考
    // 用來停止舊的相機移動
    // =========================
    private Coroutine moveRoutine;

    // =========================
    // 初始相機位置
    // Reset 時回到這裡
    // =========================
    [Header("初始相機位置")]
    public Transform startCameraPoint;

    // =========================
    // 控制滑桿
    // Reset 時回到 50%
    // =========================
    [Header("控制滑桿")]
    public Slider controlSlider;

    // =========================
    // 遊戲開始時初始化
    // =========================
    void Start()
    {
        ResetTour();
    }

    // =========================
    // 下一步
    // =========================
    public void NextStep()
    {
        // 如果沒有步驟則不執行
        if (steps.Length == 0)
            return;

        // 切換到下一步
        currentIndex++;

        // 超出範圍則停止
        if (currentIndex >= steps.Length)
        {
            currentIndex = steps.Length - 1;
            return;
        }

        // 顯示目前步驟
        ShowStep(currentIndex);
    }

    // =========================
    // 上一步
    // =========================
    public void PreviousStep()
    {
        if (steps.Length == 0)
            return;

        currentIndex--;

        // 避免小於 0
        if (currentIndex < 0)
        {
            currentIndex = 0;
        }

        ShowStep(currentIndex);
    }

    // =========================
    // 跳到指定步驟
    // =========================
    public void GoToStep(int index)
    {
        // 防止超出範圍
        if (index < 0 || index >= steps.Length)
            return;

        currentIndex = index;

        ShowStep(currentIndex);
    }

    // =========================
    // 重置導覽
    // =========================
    public void ResetTour()
    {
        // 回到初始狀態
        currentIndex = -1;

        // 重置發電廠流程
        if (powerPlantController != null)
        {
            powerPlantController.ResetPlant();
        }

        // 重置 UI 文字
        titleText.text = "Natural Gas Power Plant";

        bodyText.text =
            "Press Start to begin the guided tour.";

        nextButtonText.text = "Start";

        previousButton.interactable = false;

        // 滑桿回到 50%
        if (controlSlider != null)
        {
            controlSlider.value = 50f;
        }

        // 回到初始相機位置
        if (startCameraPoint != null)
        {
            // 停止舊 coroutine
            if (moveRoutine != null)
            {
                StopCoroutine(moveRoutine);
            }

            // 開始新的相機移動
            moveRoutine =
                StartCoroutine(
                    MoveCamera(startCameraPoint)
                );
        }
    }

    // =========================
    // 顯示指定步驟
    // =========================
    public void ShowStep(int index)
    {
        // 取得步驟資料
        TourStep step = steps[index];

        // 更新 UI
        titleText.text = step.title;
        bodyText.text = step.description;

        // 上一步按鈕控制
        previousButton.interactable = index > 0;

        // 最後一步顯示 Finished
        nextButtonText.text =
            index >= steps.Length - 1
            ? "Finished"
            : "Next";

        // 啟動對應設備動畫
        RunStepAction(step.actionName);

        // 停止舊相機移動
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }

        // 開始新的相機平滑移動
        moveRoutine =
            StartCoroutine(
                MoveCamera(step.cameraPoint)
            );
    }

    // =========================
    // 平滑移動相機 Coroutine
    // =========================
    private IEnumerator MoveCamera(Transform targetPoint)
    {
        // 起始位置
        Vector3 startPos =
            mainCamera.transform.position;

        // 起始旋轉
        Quaternion startRot =
            mainCamera.transform.rotation;

        // 目標位置
        Vector3 targetPos =
            targetPoint.position;

        // 目標旋轉
        Quaternion targetRot =
            targetPoint.rotation;

        float timer = 0f;

        // 在指定時間內平滑移動
        while (timer < moveDuration)
        {
            timer += Time.deltaTime;

            float t = timer / moveDuration;

            // 使用 SmoothStep 讓動畫更平滑
            t = Mathf.SmoothStep(0f, 1f, t);

            // 平滑移動位置
            mainCamera.transform.position =
                Vector3.Lerp(
                    startPos,
                    targetPos,
                    t
                );

            // 平滑旋轉
            mainCamera.transform.rotation =
                Quaternion.Slerp(
                    startRot,
                    targetRot,
                    t
                );

            yield return null;
        }

        // 最終位置修正
        mainCamera.transform.position =
            targetPos;

        mainCamera.transform.rotation =
            targetRot;

        // 同步滑鼠視角
        if (cameraLook != null)
        {
            cameraLook.SyncRotation();
        }
    }

    // =========================
    // 啟動對應設備動畫
    // =========================
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