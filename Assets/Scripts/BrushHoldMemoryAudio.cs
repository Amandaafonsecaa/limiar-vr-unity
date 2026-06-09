using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class BrushHoldMemoryAudio : MonoBehaviour
{
    [Header("XR Grab")]
    [SerializeField] private XRGrabInteractable grabInteractable;

    [Header("Memory Audio Sources")]
    [SerializeField] private AudioSource[] memoryAudioSources;

    [Header("Audio Settings")]
    [SerializeField] private bool restartWhenGrabbed = true;
    [SerializeField] private bool fadeAudio = true;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float targetVolume = 0.35f;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        foreach (AudioSource source in memoryAudioSources)
        {
            if (source == null)
                continue;

            source.playOnAwake = false;
            source.loop = true;
            source.volume = 0f;
        }
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }

        StopMemoryAudioImmediate();
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        foreach (AudioSource source in memoryAudioSources)
        {
            if (source == null)
                continue;

            if (restartWhenGrabbed)
                source.time = 0f;

            if (!source.isPlaying)
                source.Play();

            if (!fadeAudio)
                source.volume = targetVolume;
        }

        if (fadeAudio)
            fadeRoutine = StartCoroutine(FadeAllVolumes(targetVolume, fadeDuration));
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        if (fadeAudio)
            fadeRoutine = StartCoroutine(FadeOutAndStop());
        else
            StopMemoryAudioImmediate();
    }

    private IEnumerator FadeOutAndStop()
    {
        yield return FadeAllVolumes(0f, fadeDuration);
        StopMemoryAudioImmediate();
    }

    private IEnumerator FadeAllVolumes(float target, float duration)
    {
        float elapsed = 0f;

        float[] startVolumes = new float[memoryAudioSources.Length];

        for (int i = 0; i < memoryAudioSources.Length; i++)
        {
            if (memoryAudioSources[i] != null)
                startVolumes[i] = memoryAudioSources[i].volume;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            for (int i = 0; i < memoryAudioSources.Length; i++)
            {
                if (memoryAudioSources[i] != null)
                    memoryAudioSources[i].volume = Mathf.Lerp(startVolumes[i], target, t);
            }

            yield return null;
        }

        foreach (AudioSource source in memoryAudioSources)
        {
            if (source != null)
                source.volume = target;
        }
    }

    private void StopMemoryAudioImmediate()
    {
        foreach (AudioSource source in memoryAudioSources)
        {
            if (source == null)
                continue;

            source.Stop();
            source.volume = 0f;
            source.time = 0f;
        }
    }
}