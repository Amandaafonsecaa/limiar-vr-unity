using UnityEngine;

public class InteractableHighlight : MonoBehaviour
{
    [Header("Visual")]
    public Renderer targetRenderer;
    public Color highlightColor = Color.cyan;
    public float emissionStrength = 1.5f;

    [Header("Glitch")]
    public bool useJitter = true;
    public float jitterAmount = 0.003f;
    public float flickerSpeed = 18f;

    private Material materialInstance;
    private Color originalEmissionColor = Color.black;
    private Vector3 originalLocalPosition;
    private bool isHighlighted = false;

    private void Start()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer != null)
        {
            materialInstance = targetRenderer.material;
            originalEmissionColor = materialInstance.GetColor("_EmissionColor");
        }

        originalLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (!isHighlighted || materialInstance == null)
            return;

        float flicker = Mathf.Lerp(0.4f, emissionStrength, (Mathf.Sin(Time.time * flickerSpeed) + 1f) * 0.5f);
        materialInstance.EnableKeyword("_EMISSION");
        materialInstance.SetColor("_EmissionColor", highlightColor * flicker);

        if (useJitter)
        {
            Vector3 offset = Random.insideUnitSphere * jitterAmount;
            transform.localPosition = originalLocalPosition + offset;
        }
    }

    public void SetHighlight(bool state)
    {
        isHighlighted = state;

        if (!state)
        {
            if (materialInstance != null)
            {
                materialInstance.SetColor("_EmissionColor", originalEmissionColor);
            }

            transform.localPosition = originalLocalPosition;
        }
    }
}