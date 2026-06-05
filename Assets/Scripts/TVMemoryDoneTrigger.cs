using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class TVMemoryDoneTrigger : MonoBehaviour
{
    [Header("Progression")]
    [SerializeField] private HouseProgressionManager progressionManager;

    [Header("TV Visual")]
    [SerializeField] private GameObject staticScreen;
    [SerializeField] private VideoPlayer staticVideoPlayer;
    [SerializeField] private Light tvGlowLight;
    [SerializeField] private float tvGlowIntensity = 1.2f;

    [Header("Timing")]
    [SerializeField] private float videoDuration = 8f;
    [SerializeField] private bool turnOffAfterVideo = true;

    [Header("Detection")]
    [SerializeField] private string playerTag = "Player";

    private bool hasTriggered;

    private void Awake()
    {
        if (staticScreen != null)
            staticScreen.SetActive(false);

        if (tvGlowLight != null)
            tvGlowLight.intensity = 0f;

        if (staticVideoPlayer != null)
        {
            staticVideoPlayer.Stop();
            staticVideoPlayer.playOnAwake = false;
            staticVideoPlayer.isLooping = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        if (!other.CompareTag(playerTag))
            return;

        hasTriggered = true;
        StartCoroutine(PlayTVMemory());
    }

    private IEnumerator PlayTVMemory()
    {
        if (staticScreen != null)
            staticScreen.SetActive(true);

        if (tvGlowLight != null)
            tvGlowLight.intensity = tvGlowIntensity;

        if (staticVideoPlayer != null)
        {
            staticVideoPlayer.time = 0;
            staticVideoPlayer.Play();
        }

        if (progressionManager != null)
            progressionManager.MarkTvMemoryDone();

        Debug.Log("Memória da TV concluída.");

        yield return new WaitForSeconds(videoDuration);

        if (!turnOffAfterVideo)
            yield break;

        if (staticVideoPlayer != null)
            staticVideoPlayer.Stop();

        if (tvGlowLight != null)
            tvGlowLight.intensity = 0f;

        if (staticScreen != null)
            staticScreen.SetActive(false);
    }
}