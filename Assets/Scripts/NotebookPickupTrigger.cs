using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class NotebookPickupTrigger : MonoBehaviour
{
    [Header("XR")]
    [SerializeField] private XRGrabInteractable grabInteractable;

    [Header("Progression")]
    [SerializeField] private HouseProgressionManager progressionManager;

    private void Awake()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnNotebookGrabbed);
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnNotebookGrabbed);
    }

    private void OnNotebookGrabbed(SelectEnterEventArgs args)
    {
        if (progressionManager != null)
            progressionManager.TryTriggerNotebookEvent();
    }
}