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
            staticVideoPlayer.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        if (!other.CompareTag(playerTag))
            return;

        hasTriggered = true;
        ActivateTVMemory();
    }

    private void ActivateTVMemory()
    {
        if (staticScreen != null)
            staticScreen.SetActive(true);

        if (staticVideoPlayer != null)
            staticVideoPlayer.Play();

        if (tvGlowLight != null)
            tvGlowLight.intensity = tvGlowIntensity;

        if (progressionManager != null)
            progressionManager.MarkTvMemoryDone();

        Debug.Log("Memória da TV concluída.");
    }
}