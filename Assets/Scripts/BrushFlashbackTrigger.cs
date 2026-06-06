using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BrushFlashbackTrigger : MonoBehaviour
{
    [System.Serializable]
    public class CaptionLine
    {
        [TextArea(2, 4)]
        public string text;

        public float duration = 2.5f;
    }

    [Header("XR Interaction")]
    [SerializeField] private XRGrabInteractable grabInteractable;

    [Header("Progression")]
    [SerializeField] private HouseProgressionManager progressionManager;

    [Header("Flashback Objects Optional")]
    [SerializeField] private GameObject flashbackRoot;

    [Header("Caption UI")]
    [SerializeField] private GameObject captionCanvas;
    [SerializeField] private CanvasGroup captionCanvasGroup;
    [SerializeField] private TMP_Text captionText;

    [Header("Caption Lines")]
    [SerializeField] private CaptionLine[] captionLines;

    [Header("Timing")]
    [SerializeField] private float delayBeforeCaptions = 0.2f;
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float delayBetweenLines = 0.2f;

    private bool hasPlayed;

    private void Awake()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        if (flashbackRoot != null)
            flashbackRoot.SetActive(false);

        if (captionCanvas != null)
            captionCanvas.SetActive(false);

        if (captionCanvasGroup != null)
            captionCanvasGroup.alpha = 0f;

        if (captionText != null)
            captionText.text = "";
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnBrushGrabbed);
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnBrushGrabbed);
    }

    private void OnBrushGrabbed(SelectEnterEventArgs args)
    {
        if (hasPlayed)
            return;

        hasPlayed = true;
        StartCoroutine(PlayBrushMemory());
    }

    private IEnumerator PlayBrushMemory()
    {
        if (flashbackRoot != null)
            flashbackRoot.SetActive(true);

        yield return new WaitForSeconds(delayBeforeCaptions);

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

        if (flashbackRoot != null)
            flashbackRoot.SetActive(false);

        if (progressionManager != null)
            progressionManager.MarkBrushMemoryDone();

        Debug.Log("Memória da escova concluída.");
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