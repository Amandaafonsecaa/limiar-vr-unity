using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
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

    [Header("Video Cutscene")]
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private float fallbackVideoDuration = 8f;

    [Header("Disable During Cutscene")]
    [SerializeField] private GameObject[] objectsToDisableDuringCutscene;

    [Header("Caption UI")]
    [SerializeField] private GameObject captionCanvas;
    [SerializeField] private CanvasGroup captionCanvasGroup;
    [SerializeField] private TMP_Text captionText;

    [Header("Caption Lines")]
    [SerializeField] private CaptionLine[] captionLines;

    [Header("Timing")]
    [SerializeField] private float delayBeforeCaptions = 0.3f;
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float delayBetweenLines = 0.2f;

    private bool hasPlayed;

    private void Awake()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        if (videoPanel != null)
            videoPanel.SetActive(false);

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = false;
            videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        }

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
        StartCoroutine(PlayBrushCutscene());
    }

    private IEnumerator PlayBrushCutscene()
    {
        SetCutsceneObjects(false);

        if (videoPanel != null)
            videoPanel.SetActive(true);

        if (videoPlayer != null)
        {
            videoPlayer.time = 0;
            videoPlayer.Play();
        }

        yield return new WaitForSeconds(delayBeforeCaptions);

        Coroutine captionRoutine = StartCoroutine(PlayCaptions());

        if (videoPlayer != null)
        {
            while (videoPlayer.isPlaying)
                yield return null;
        }
        else
        {
            yield return new WaitForSeconds(fallbackVideoDuration);
        }

        if (captionRoutine != null)
            yield return captionRoutine;

        if (videoPlayer != null)
            videoPlayer.Stop();

        if (videoPanel != null)
            videoPanel.SetActive(false);

        SetCutsceneObjects(true);

        if (progressionManager != null)
            progressionManager.MarkBrushMemoryDone();

        Debug.Log("Cutscene da escova concluída.");
    }

    private void SetCutsceneObjects(bool active)
    {
        if (objectsToDisableDuringCutscene == null)
            return;

        foreach (GameObject obj in objectsToDisableDuringCutscene)
        {
            if (obj != null)
                obj.SetActive(active);
        }
    }

    private IEnumerator PlayCaptions()
    {
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