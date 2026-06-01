using UnityEngine;
using DG.Tweening;

public class AbirPorta : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private Transform doorPivot;
    [SerializeField] private Vector3 openRotationOffset = new Vector3(0f, 0f, 90f);
    [SerializeField] private float duration = 0.5f;

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

        closedRotation = doorPivot.localEulerAngles;
        isLocked = startsLocked;
    }

    public void ToggleDoor()
    {
        if (isLocked)
        {
            Debug.Log("Porta da mãe está trancada.");
            return;
        }

        if (isOpen)
            CloseDoor();
        else
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

        Vector3 targetRotation = closedRotation + openRotationOffset;

        doorTween = doorPivot
            .DOLocalRotate(targetRotation, duration)
            .SetEase(Ease.OutCubic);

        isOpen = true;
    }

    public void CloseDoor()
    {
        if (!isOpen)
            return;

        doorTween?.Kill();

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