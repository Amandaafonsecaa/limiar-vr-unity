using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ReadablePaper : MonoBehaviour
{
    [Header("XR Interaction")]
    [SerializeField] private XRSimpleInteractable interactable;

    [Header("Document View")]
    [SerializeField] private GameObject documentCanvas;

    private bool isOpen;

    private void Awake()
    {
        if (interactable == null)
            interactable = GetComponent<XRSimpleInteractable>();

        if (documentCanvas != null)
            documentCanvas.SetActive(false);
    }

    private void OnEnable()
    {
        if (interactable != null)
            interactable.selectEntered.AddListener(OnDocumentSelected);
    }

    private void OnDisable()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnDocumentSelected);
    }

    private void OnDocumentSelected(SelectEnterEventArgs args)
    {
        isOpen = !isOpen;

        if (documentCanvas != null)
            documentCanvas.SetActive(isOpen);

        Debug.Log(isOpen ? "Registro de visitas aberto." : "Registro de visitas fechado.");
    }
}