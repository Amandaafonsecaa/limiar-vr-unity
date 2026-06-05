using UnityEngine;
using DG.Tweening;

public class AbirPorta : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private Transform doorPivot;
    [SerializeField] private Vector3 openRotationOffset = new Vector3(0f, 90f, 0f); // Ajustado para rotacionar no eixo Y comum de portas
    [SerializeField] private float duration = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource openDoorAudio;
    [SerializeField] private AudioSource closeDoorAudio; // Novo campo para o som de fechar

    [Header("State")]
    [SerializeField] private bool startsLocked = false; // Mudei para false para facilitar seus testes iniciais!

    private Vector3 closedRotation;
    private bool isOpen;
    private bool isLocked;
    private Tween doorTween;

    private void Awake()
    {
        if (doorPivot == null)
            doorPivot = transform;

        if (openDoorAudio == null)
            openDoorAudio = GetComponent<AudioSource>();

        closedRotation = doorPivot.localEulerAngles;
        isLocked = startsLocked;
    }

    // Função inteligente: se estiver aberta, fecha. Se estiver fechada, tenta abrir.
    public void InteragirComAPorta()
    {
        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            TryOpenDoor();
        }
    }

    public void TryOpenDoor()
    {
        if (isLocked)
        {
            Debug.Log("Porta da mãe está trancada.");
            return;
        }

        OpenDoor();
    }

    public void UnlockDoor()
    {
        isLocked = false;
        Debug.Log("Porta da mãe destrancada.");
    }

    public void OpenDoor()
    {
        if (isOpen) return;

        doorTween?.Kill();

        if (openDoorAudio != null)
            openDoorAudio.Play();

        Vector3 targetRotation = closedRotation + openRotationOffset;

        doorTween = doorPivot
            .DOLocalRotate(targetRotation, duration)
            .SetEase(Ease.OutCubic);

        isOpen = true;
    }

    // ✅ NOVA FUNÇÃO: Faz a porta voltar para a rotação original fechada
    public void CloseDoor()
    {
        if (!isOpen) return;

        doorTween?.Kill();

        if (closeDoorAudio != null)
            closeDoorAudio.Play();

        doorTween = doorPivot
            .DOLocalRotate(closedRotation, duration)
            .SetEase(Ease.OutCubic);

        isOpen = false;
    }

    public void UnlockAndOpenDoor()
    {
        UnlockDoor();
        OpenDoor();
    }
}