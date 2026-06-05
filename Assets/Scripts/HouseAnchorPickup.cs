using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HouseAnchorPickup : MonoBehaviour
{
    [Header("XR Interaction")]
    [SerializeField] private XRSimpleInteractable interactable;

    [Header("Visual")]
    [SerializeField] private GameObject visualRoot;

    [Header("Optional Audio")]
    [SerializeField] private AudioSource pickupAudio;

    [Header("Events")]
    public UnityEvent onAnchorCollected;

    private bool hasCollected;

    private void Awake()
    {
        if (interactable == null)
            interactable = GetComponent<XRSimpleInteractable>();

        if (visualRoot == null)
            visualRoot = gameObject;
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
        if (hasCollected)
            return;

        hasCollected = true;
        CollectAnchor();
    }

    private void CollectAnchor()
    {
        Debug.Log("Âncora da casa coletada.");

        if (pickupAudio != null)
            pickupAudio.Play();

        onAnchorCollected?.Invoke();

        if (visualRoot != null)
            visualRoot.SetActive(false);
    }
}