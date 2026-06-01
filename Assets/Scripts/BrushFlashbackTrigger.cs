using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BrushFlashbackTrigger : MonoBehaviour
{
    [Header("XR Interaction")]
    [SerializeField] private XRGrabInteractable grabInteractable;

    [Header("Progression")]
    [SerializeField] private HouseProgressionManager progressionManager;

    [Header("Flashback Objects Optional")]
    [SerializeField] private GameObject flashbackRoot;

    [Header("Timing")]
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

        if (flashbackAudio != null)
            flashbackAudio.Play();

        yield return new WaitForSeconds(flashbackDuration);

        if (flashbackRoot != null)
            flashbackRoot.SetActive(false);

        if (progressionManager != null)
            progressionManager.MarkBrushMemoryDone();

        Debug.Log("Memória da escova concluída.");
    }
}