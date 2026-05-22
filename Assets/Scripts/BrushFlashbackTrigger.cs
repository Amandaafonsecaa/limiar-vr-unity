using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;

public class BrushFlashbackTrigger : MonoBehaviour
{
    [Header("XR Interaction")]
    [SerializeField] private XRGrabInteractable grabInteractable;

    [Header("Flashback Objects")]
    [SerializeField] private GameObject flashbackRoot;

    [Header("UI")]
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private TMP_Text subtitleText;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 0.6f;
    [SerializeField] private float flashbackDuration = 5f;

    [Header("Optional Audio")]
    [SerializeField] private AudioSource flashbackAudio;

    private bool hasPlayed;

    private void Awake()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        if (flashbackRoot != null)
            flashbackRoot.SetActive(false);

        if (fadeCanvas != null)
            fadeCanvas.alpha = 0f;

        if (subtitleText != null)
            subtitleText.text = "";
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
        StartCoroutine(PlayFlashback());
    }

    private IEnumerator PlayFlashback()
    {
        yield return Fade(1f);

        if (flashbackRoot != null)
            flashbackRoot.SetActive(true);

        if (subtitleText != null)
            subtitleText.text = "A voz da mãe ecoa pela casa...";

        if (flashbackAudio != null)
            flashbackAudio.Play();

        yield return Fade(0f);

        yield return new WaitForSeconds(2f);

        if (subtitleText != null)
            subtitleText.text = "Flora lembra da mãe penteando seu cabelo.";

        yield return new WaitForSeconds(flashbackDuration);

        yield return Fade(1f);

        if (flashbackRoot != null)
            flashbackRoot.SetActive(false);

        if (subtitleText != null)
            subtitleText.text = "";

        yield return Fade(0f);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeCanvas == null)
            yield break;

        float startAlpha = fadeCanvas.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        fadeCanvas.alpha = targetAlpha;
    }
}