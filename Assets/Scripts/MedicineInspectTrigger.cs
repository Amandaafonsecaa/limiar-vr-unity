using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MedicineInspectTrigger : MonoBehaviour
{
    [Header("XR Interaction")]
    [SerializeField] private XRGrabInteractable grabInteractable;

    [Header("Progression")]
    [SerializeField] private MotherRoomProgressionManager progressionManager;

    [Header("Optional Visual Feedback")]
    [SerializeField] private GameObject objectToEnableAfterInspect;
    [SerializeField] private Light highlightLight;

    private bool hasTriggered;

    private void Awake()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        if (objectToEnableAfterInspect != null)
            objectToEnableAfterInspect.SetActive(false);

        if (highlightLight != null)
            highlightLight.enabled = false;
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnMedicineGrabbed);
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnMedicineGrabbed);
    }

    private void OnMedicineGrabbed(SelectEnterEventArgs args)
    {
        if (hasTriggered)
            return;

        hasTriggered = true;

        if (objectToEnableAfterInspect != null)
            objectToEnableAfterInspect.SetActive(true);

        if (highlightLight != null)
            highlightLight.enabled = true;

        if (progressionManager != null)
            progressionManager.MarkMedicineSeen();

        Debug.Log("Remédios inspecionados.");
    }
}