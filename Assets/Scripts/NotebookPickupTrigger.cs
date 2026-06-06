using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class NotebookPickupTrigger : MonoBehaviour
{
    [System.Serializable]
    public class CaptionLine
    {
        [TextArea(2, 4)]
        public string text;

        public float duration = 2.5f;
    }

    [Header("XR")]
    [SerializeField] private XRGrabInteractable grabInteractable;

    [Header("Progression")]
    [SerializeField] private HouseProgressionManager progressionManager;

    [Header("Caption UI")]
    [SerializeField] private GameObject captionCanvas;
    [SerializeField] private CanvasGroup captionCanvasGroup;
    [SerializeField] private TMP_Text captionText;

    [Header("Success Captions")]
    [SerializeField] private CaptionLine[] successLines;

    [Header("Locked Captions")]
    [SerializeField] private CaptionLine[] lockedLines;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float delayBetweenLines = 0.2f;

    private bool notebookEventCompleted;
    private bool isPlayingCaptions;
    private Coroutine captionRoutine;

    private void Awake()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

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
            grabInteractable.selectEntered.AddListener(OnNotebookGrabbed);
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnNotebookGrabbed);
    }

    private void OnNotebookGrabbed(SelectEnterEventArgs args)
    {
        if (isPlayingCaptions)
            return;

        if (progressionManager == null)
        {
            Debug.LogWarning("NotebookPickupTrigger está sem Progression Manager.");
            return;
        }

        bool canTriggerNotebookEvent = progressionManager.TryTriggerNotebookEvent();

        if (canTriggerNotebookEvent)
        {
            if (notebookEventCompleted)
                return;

            notebookEventCompleted = true;
            PlayCaptionSequence(successLines);
        }
        else
        {
            PlayCaptionSequence(lockedLines);
        }
    }

    private void PlayCaptionSequence(CaptionLine[] lines)
    {
        if (captionRoutine != null)
            StopCoroutine(captionRoutine);

        captionRoutine = StartCoroutine(PlayCaptions(lines));
    }

    private IEnumerator PlayCaptions(CaptionLine[] lines)
    {
        isPlayingCaptions = true;

        if (captionCanvas != null)
            captionCanvas.SetActive(true);

        if (lines != null)
        {
            foreach (CaptionLine line in lines)
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
        }

        if (captionText != null)
            captionText.text = "";

        if (captionCanvas != null)
            captionCanvas.SetActive(false);

        isPlayingCaptions = false;
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