using UnityEngine;
using DG.Tweening;

public class AbirPorta : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private Transform doorPivot;
    [SerializeField] private Vector3 openRotationOffset = new Vector3(0f, 0f, 90f);
    [SerializeField] private float duration = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource openDoorAudio;

    [Header("State")]
    [SerializeField] private bool startsLocked = true;

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
        if (isOpen)
            return;

        doorTween?.Kill();

        if (openDoorAudio != null)
            openDoorAudio.Play();

        Vector3 targetRotation = closedRotation + openRotationOffset;

        doorTween = doorPivot
            .DOLocalRotate(targetRotation, duration)
            .SetEase(Ease.OutCubic);

        isOpen = true;
    }

    public void UnlockAndOpenDoor()
    {
        UnlockDoor();
        OpenDoor();
    }
}