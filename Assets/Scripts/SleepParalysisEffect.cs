using System.Collections;
using UnityEngine;

public class SleepParalysisEffect : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource breathingAudio;
    [SerializeField] private AudioSource heartbeatAudio;

    [Header("Volume")]
    [SerializeField] private float breathingMaxVolume = 0.75f;
    [SerializeField] private float heartbeatMaxVolume = 0.35f;
    [SerializeField] private float fadeInDuration = 3f;
    [SerializeField] private float fadeOutDuration = 2f;

    [Header("Optional Light")]
    [SerializeField] private Light roomLight;
    [SerializeField] private float tenseLightIntensity = 0.15f;

    private Coroutine effectRoutine;
    private float originalLightIntensity;

    private void Awake()
    {
        if (breathingAudio != null)
        {
            breathingAudio.loop = true;
            breathingAudio.playOnAwake = false;
            breathingAudio.volume = 0f;
        }

        if (heartbeatAudio != null)
        {
            heartbeatAudio.loop = true;
            heartbeatAudio.playOnAwake = false;
            heartbeatAudio.volume = 0f;
        }

        if (roomLight != null)
            originalLightIntensity = roomLight.intensity;
    }

    public void StartEffect()
    {
        if (effectRoutine != null)
            StopCoroutine(effectRoutine);

        effectRoutine = StartCoroutine(StartEffectRoutine());
    }

    public void StopEffect()
    {
        if (effectRoutine != null)
            StopCoroutine(effectRoutine);

        effectRoutine = StartCoroutine(StopEffectRoutine());
    }

    private IEnumerator StartEffectRoutine()
    {
        if (breathingAudio != null && !breathingAudio.isPlaying)
            breathingAudio.Play();

        if (heartbeatAudio != null && !heartbeatAudio.isPlaying)
            heartbeatAudio.Play();

        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeInDuration;

            if (breathingAudio != null)
                breathingAudio.volume = Mathf.Lerp(0f, breathingMaxVolume, t);

            if (heartbeatAudio != null)
                heartbeatAudio.volume = Mathf.Lerp(0f, heartbeatMaxVolume, t);

            if (roomLight != null)
                roomLight.intensity = Mathf.Lerp(originalLightIntensity, tenseLightIntensity, t);

            yield return null;
        }

        if (breathingAudio != null)
            breathingAudio.volume = breathingMaxVolume;

        if (heartbeatAudio != null)
            heartbeatAudio.volume = heartbeatMaxVolume;

        if (roomLight != null)
            roomLight.intensity = tenseLightIntensity;
    }

    private IEnumerator StopEffectRoutine()
    {
        float startBreathingVolume = breathingAudio != null ? breathingAudio.volume : 0f;
        float startHeartbeatVolume = heartbeatAudio != null ? heartbeatAudio.volume : 0f;
        float startLightIntensity = roomLight != null ? roomLight.intensity : 0f;

        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutDuration;

            if (breathingAudio != null)
                breathingAudio.volume = Mathf.Lerp(startBreathingVolume, 0f, t);

            if (heartbeatAudio != null)
                heartbeatAudio.volume = Mathf.Lerp(startHeartbeatVolume, 0f, t);

            if (roomLight != null)
                roomLight.intensity = Mathf.Lerp(startLightIntensity, originalLightIntensity, t);

            yield return null;
        }

        if (breathingAudio != null)
        {
            breathingAudio.volume = 0f;
            breathingAudio.Stop();
        }

        if (heartbeatAudio != null)
        {
            heartbeatAudio.volume = 0f;
            heartbeatAudio.Stop();
        }

        if (roomLight != null)
            roomLight.intensity = originalLightIntensity;
    }
}