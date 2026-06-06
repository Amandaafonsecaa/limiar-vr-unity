using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HouseAnchorPickup : MonoBehaviour
{
    [System.Serializable]
    public class CaptionLine
    {
        [TextArea(2, 4)]
        public string text;

        public float duration = 2.5f;
    }

    [Header("XR Interaction")]
    [SerializeField] private XRSimpleInteractable interactable;

    [Header("Visual")]
    [SerializeField] private GameObject visualRoot;

    [Header("Caption UI")]
    [SerializeField] private GameObject captionCanvas;
    [SerializeField] private CanvasGroup captionCanvasGroup;
    [SerializeField] private TMP_Text captionText;

    [Header("Caption Lines")]
    [SerializeField] private CaptionLine[] captionLines;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float delayBetweenLines = 0.2f;
    [SerializeField] private float delayBeforeDisappear = 0.3f;

    [Header("Events")]
    public UnityEvent onAnchorCollected;

    private bool hasCollected;

    private void Awake()
    {
        if (interactable == null)
            interactable = GetComponent<XRSimpleInteractable>();

        if (visualRoot == null)
            visualRoot = gameObject;

        if (captionCanvas != null)
            captionCanvas.SetActive(false);

        if (captionCanvasGroup != null)
            captionCanvasGroup.alpha = 0f;

        if (captionText != null)
            captionText.text = "";
    }

    private void OnEnable()
    {
        if (interactable != null)
            interactable.selectEntered.AddListener(OnAnchorSelected);
    }

    private void OnDisable()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnAnchorSelected);
    }

    private void OnAnchorSelected(SelectEnterEventArgs args)
    {
        if (hasCollected)
            return;

        hasCollected = true;
        StartCoroutine(CollectAnchorSequence());
    }

    private IEnumerator CollectAnchorSequence()
    {
        Debug.Log("Âncora da casa encontrada.");

        if (captionCanvas != null)
            captionCanvas.SetActive(true);

        foreach (CaptionLine line in captionLines)
        {
            if (captionText != null)
                captionText.text = line.text;

            yield return FadeCaption(1f);
            yield return new WaitForSeconds(line.duration);
            yield return FadeCaption(0f);

            if (captionText != null)
                captionText.text = "";

            yield return new WaitForSeconds(delayBetweenLines);
        }

        if (captionCanvas != null)
            captionCanvas.SetActive(false);

        onAnchorCollected?.Invoke();

        yield return new WaitForSeconds(delayBeforeDisappear);

        if (visualRoot != null)
            visualRoot.SetActive(false);

        Debug.Log("Âncora da casa coletada.");
    }

    private IEnumerator FadeCaption(float targetAlpha)
    {
        if (captionCanvasGroup == null)
            yield break;

        float startAlpha = captionCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            captionCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        captionCanvasGroup.alpha = targetAlpha;
    }
}