// 控制可點擊設備標籤，滑鼠移上去時背景變深，點擊後移動導覽相機並讓整個標籤放大

using UnityEngine;
using System.Collections;

public class ClickableTourLabel : MonoBehaviour
{
    [Header("導覽控制器")]
    public GuidedTourController tourController;

    [Header("對應導覽步驟編號")]
    public int stepIndex = 0;

    [Header("背景 Renderer")]
    public Renderer backgroundRenderer;

    [Header("一般背景顏色")]
    public Color normalColor = new Color(0f, 0f, 0f, 0.35f);

    [Header("滑鼠移上去背景顏色")]
    public Color hoverColor = new Color(0f, 0f, 0f, 0.65f);

    [Header("點擊放大倍率")]
    public float clickScale = 1.08f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;

        if (backgroundRenderer != null)
        {
            backgroundRenderer.material.color = normalColor;
        }
    }

    void OnMouseEnter()
    {
        if (backgroundRenderer != null)
        {
            backgroundRenderer.material.color = hoverColor;
        }
    }

    void OnMouseExit()
    {
        if (backgroundRenderer != null)
        {
            backgroundRenderer.material.color = normalColor;
        }
    }

    void OnMouseDown()
    {
        if (tourController != null)
        {
            tourController.ShowStep(stepIndex);
        }

        StopAllCoroutines();
        StartCoroutine(ClickEffect());
    }

    private IEnumerator ClickEffect()
    {
        transform.localScale = originalScale * clickScale;

        yield return new WaitForSeconds(0.12f);

        transform.localScale = originalScale;
    }
}