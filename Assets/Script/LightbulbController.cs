using UnityEngine;

public class LightbulbController : MonoBehaviour
{
    [Header("¿Oªwªº Mesh Renderer")]
    public Renderer bulbRenderer;

    [Header("Ãö³¬ª¬ºA§÷½è")]
    public Material offMaterial;

    [Header("µo«Gª¬ºA§÷½è")]
    public Material onMaterial;

    [Header("¿Oªw¥ú·½")]
    public Light bulbLight;

    void Start()
    {
        TurnOff();
    }

    public void TurnOn()
    {
        if (bulbRenderer != null && onMaterial != null)
        {
            bulbRenderer.material = onMaterial;
        }

        if (bulbLight != null)
        {
            bulbLight.enabled = true;
        }
    }

    public void TurnOff()
    {
        if (bulbRenderer != null && offMaterial != null)
        {
            bulbRenderer.material = offMaterial;
        }

        if (bulbLight != null)
        {
            bulbLight.enabled = false;
        }
    }
}