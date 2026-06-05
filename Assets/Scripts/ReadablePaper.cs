using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ReadablePaper : MonoBehaviour
{
    [Header("XR Interaction")]
    [SerializeField] private XRSimpleInteractable interactable;

    [Header("Read View")]
    [SerializeField] private GameObject documentCanvas;

    [Header("Progression")]
    [SerializeField] private MotherRoomProgressionManager progressionManager;

    private bool isOpen;
    private bool hasBeenRead;

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
            interactable.selectEntered.AddListener(OnPaperSelected);
    }

    private void OnDisable()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnPaperSelected);
    }

    private void OnPaperSelected(SelectEnterEventArgs args)
    {
        ToggleDocument();
    }

    private void ToggleDocument()
    {
        isOpen = !isOpen;

        if (documentCanvas != null)
            documentCanvas.SetActive(isOpen);

        if (!hasBeenRead)
        {
            hasBeenRead = true;

            if (progressionManager != null)
                progressionManager.MarkPrescriptionRead();

            Debug.Log("Receita/atestado lido.");
        }
    }
}