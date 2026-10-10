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
            case "NEXT":
                if (tourController != null)
                    tourController.NextStep();
                break;

            case "PREVIOUS":
                if (tourController != null)
                    tourController.PreviousStep();
                break;

            case "RESET":
                if (tourController != null)
                    tourController.ResetTour();
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