using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MedicineTouchTrigger : MonoBehaviour
{
    [Header("XR Interaction")]
    [SerializeField] private XRSimpleInteractable interactable;

    [Header("Progression")]
    [SerializeField] private MotherRoomProgressionManager progressionManager;

    [Header("Flora Voice")]
    [SerializeField] private AudioSource floraLineAudio;

    [Header("Optional Visual Feedback")]
    [SerializeField] private Light highlightLight;
    [SerializeField] private GameObject objectToEnableAfterTouch;

    private bool hasTriggered;

    private void Awake()
    {
        if (interactable == null)
            interactable = GetComponent<XRSimpleInteractable>();

        if (highlightLight != null)
            highlightLight.enabled = false;

        if (objectToEnableAfterTouch != null)
            objectToEnableAfterTouch.SetActive(false);
    }

    private void OnEnable()
    {
        if (interactable != null)
            interactable.selectEntered.AddListener(OnMedicineTouched);
    }

    private void OnDisable()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnMedicineTouched);
    }

    private void OnMedicineTouched(SelectEnterEventArgs args)
    {
        if (hasTriggered)
            return;

        hasTriggered = true;

        if (floraLineAudio != null)
            floraLineAudio.Play();

        if (highlightLight != null)
            highlightLight.enabled = true;

        if (objectToEnableAfterTouch != null)
            objectToEnableAfterTouch.SetActive(true);

        if (progressionManager != null)
            progressionManager.MarkMedicineSeen();

        Debug.Log("Remédios tocados. Fala da Flora ativada.");
    }
}