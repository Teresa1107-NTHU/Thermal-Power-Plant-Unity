using UnityEngine;

public class ThermalWebMessageReceiver : MonoBehaviour
{
    [Header("¬J¦³±±¨î¾¹")]
    public GuidedTourController tourController;
    public PowerPlantSequenceController plantController;
    public FuelPowerController fuelPowerController;

    public void HandleWebCommand(string command)
    {
        if (string.IsNullOrEmpty(command))
            return;

        Debug.Log("[Web] Thermal command: " + command);

        switch (command)
        {
            case "RESET":
                ResetAll();
                break;

            case "STEP_0":
                GoToStep(0);
                break;

            case "STEP_1":
                GoToStep(1);
                break;

            case "STEP_2":
                GoToStep(2);
                break;

            case "STEP_3":
                GoToStep(3);
                break;

            case "STEP_4":
                GoToStep(4);
                break;

            case "STEP_5":
                GoToStep(5);
                break;

            case "GAS":
                GoToStep(0);
                break;

            case "BOILER":
                GoToStep(1);
                break;

            case "STEAM":
                GoToStep(2);
                break;

            case "COOLING":
                GoToStep(3);
                break;

            case "ELECTRICITY":
                GoToStep(4);
                break;

            case "LIGHTBULB":
                GoToStep(5);
                break;
        }
    }

    public void SetGasPercent(string value)
    {
        if (fuelPowerController == null)
            return;

        if (!float.TryParse(value, out float percent))
            return;

        percent = Mathf.Clamp(percent, 0f, 100f);

        fuelPowerController.SetGasPercent(percent);
    }

    private void GoToStep(int index)
    {
        if (tourController != null)
        {
            tourController.GoToStep(index);
        }
    }

    private void ResetAll()
    {
        if (tourController != null)
        {
            tourController.ResetTour();
        }
        else if (plantController != null)
        {
            plantController.ResetPlant();
        }
    }

    public void TestBoiler()
    {
        HandleWebCommand("BOILER");
    }
}