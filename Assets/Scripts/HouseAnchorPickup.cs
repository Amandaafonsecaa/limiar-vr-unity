using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [Header("Progression")]
    [SerializeField] private MotherRoomProgressionManager progressionManager;

    [Header("Sleep Paralysis Effect")]
    [SerializeField] private SleepParalysisEffect sleepParalysisEffect;

    [Header("Visual")]
    [SerializeField] private GameObject visualRoot;
    [SerializeField] private Light anchorLight;

    [Header("Caption UI")]
    [SerializeField] private GameObject captionCanvas;
    [SerializeField] private CanvasGroup captionCanvasGroup;
    [SerializeField] private TMP_Text captionText;

    [Header("Locked Caption Lines")]
    [SerializeField] private CaptionLine[] lockedCaptionLines;

    [Header("Collect Caption Lines")]
    [SerializeField] private CaptionLine[] collectCaptionLines;

    [Header("Fade To Hub")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeToBlackDuration = 1.5f;
    [SerializeField] private float delayBeforeSceneLoad = 0.8f;
    [SerializeField] private bool loadHubAfterCollect = false;
    [SerializeField] private string hubSceneName = "HubBranco";

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float delayBetweenLines = 0.2f;

    private bool collected;
    private bool captionPlaying;

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

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.gameObject.SetActive(false);
        }
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
        if (collected || captionPlaying)
            return;

        if (progressionManager != null && !progressionManager.CanCollectAnchor())
        {
            StartCoroutine(PlayLockedSequence());
            return;
        }

        collected = true;
        StartCoroutine(CollectAnchorSequence());
    }

    private IEnumerator PlayLockedSequence()
    {
        captionPlaying = true;

        yield return PlayCaptions(lockedCaptionLines);

        captionPlaying = false;
    }

    private IEnumerator CollectAnchorSequence()
    {
        captionPlaying = true;

        Debug.Log("Âncora da casa coletada.");

        yield return PlayCaptions(collectCaptionLines);

        if (anchorLight != null)
            anchorLight.enabled = false;

        if (visualRoot != null)
            visualRoot.SetActive(false);

        if (sleepParalysisEffect != null)
            sleepParalysisEffect.StopEffect();

        Debug.Log("Fim da fase da casa. Iniciando fade para preto.");

        yield return FadeToBlack();

        yield return new WaitForSeconds(delayBeforeSceneLoad);

        if (loadHubAfterCollect)
        {
            SceneManager.LoadScene(hubSceneName);
        }
        else
        {
            Debug.Log("Hub ainda não carregado. Fase da casa concluída por enquanto.");
        }

        captionPlaying = false;
    }

    private IEnumerator PlayCaptions(CaptionLine[] lines)
    {
        if (lines == null || lines.Length == 0)
            yield break;

        if (captionCanvas != null)
            captionCanvas.SetActive(true);

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

    private IEnumerator FadeToBlack()
    {
        if (fadeCanvasGroup == null)
        {
            Debug.LogWarning("Fade Canvas Group não conectado. Sem fade para preto.");
            yield break;
        }

        fadeCanvasGroup.gameObject.SetActive(true);
        fadeCanvasGroup.alpha = 0f;

        Debug.Log("Fade começou.");

        float elapsed = 0f;

        while (elapsed < fadeToBlackDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeToBlackDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;

        Debug.Log("Fade terminou. Tela deveria estar preta.");
    }
}