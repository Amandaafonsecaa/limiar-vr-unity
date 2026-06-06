using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ReadablePaper : MonoBehaviour
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

    [Header("Document Read View")]
    [SerializeField] private GameObject documentCanvas;

    [Header("Progression")]
    [SerializeField] private MotherRoomProgressionManager progressionManager;

    [Header("Caption UI")]
    [SerializeField] private GameObject captionCanvas;
    [SerializeField] private CanvasGroup captionCanvasGroup;
    [SerializeField] private TMP_Text captionText;

    [Header("Caption Lines After Reading")]
    [SerializeField] private CaptionLine[] captionLines;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float delayBetweenLines = 0.2f;

    private bool isDocumentOpen;
    private bool hasTriggeredReaction;
    private bool isPlayingCaptions;
    private Coroutine captionRoutine;

    private void Awake()
    {
        if (interactable == null)
            interactable = GetComponent<XRSimpleInteractable>();

        if (documentCanvas != null)
            documentCanvas.SetActive(false);

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
            interactable.selectEntered.AddListener(OnPaperSelected);
    }

    private void OnDisable()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnPaperSelected);
    }

    private void OnPaperSelected(SelectEnterEventArgs args)
    {
        if (isPlayingCaptions)
            return;

        ToggleDocument();
    }

    private void ToggleDocument()
    {
        isDocumentOpen = !isDocumentOpen;

        if (documentCanvas != null)
            documentCanvas.SetActive(isDocumentOpen);

        // Só toca a reação depois que o jogador fecha o documento pela primeira vez.
        if (!isDocumentOpen && !hasTriggeredReaction)
        {
            hasTriggeredReaction = true;
            PlayCaptionSequence();
        }
    }

    private void PlayCaptionSequence()
    {
        if (captionRoutine != null)
            StopCoroutine(captionRoutine);

        captionRoutine = StartCoroutine(PlayCaptions());
    }

    private IEnumerator PlayCaptions()
    {
        isPlayingCaptions = true;

        if (captionCanvas != null)
            captionCanvas.SetActive(true);

        if (captionLines != null)
        {
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
        }

        if (captionText != null)
            captionText.text = "";

        if (captionCanvas != null)
            captionCanvas.SetActive(false);

        if (progressionManager != null)
            progressionManager.MarkPrescriptionRead();

        Debug.Log("Receita/atestado lido e reação da Flora concluída.");

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