using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Namespace obrigatório na Unity 6

public class ReadablePaper : MonoBehaviour
{
    [Header("XR Interaction")]
    [SerializeField] private XRSimpleInteractable interactable; // Tipo direto e seguro para Unity 6

    [Header("Read View")]
    [SerializeField] private GameObject documentCanvas;

    private bool isOpen = false;

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
        {
            // Na Unity 6, o evento de ativação por raio usa selectEntered ou activated dependendo do Profile.
            // Vamos escutar o 'activated' que é o gatilho padrão do simulador.
            interactable.activated.AddListener(OnPaperActivated);
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.activated.RemoveListener(OnPaperActivated);
        }
    }

    private void OnPaperActivated(ActivateEventArgs args)
    {
        ToggleDocument();
    }

    public void ToggleDocument()
    {
        isOpen = !isOpen;

        if (documentCanvas != null)
        {
            documentCanvas.SetActive(isOpen);
            Debug.Log("Estado do papel alterado para: " + isOpen);
        }
    }
}