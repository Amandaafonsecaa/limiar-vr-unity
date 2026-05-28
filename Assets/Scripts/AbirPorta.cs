using UnityEngine;
using DG.Tweening;

public class AbirPorta : MonoBehaviour
{
[SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float duration = 0.5f;

    private Vector3 closedRotation;
    private bool isOpen;
    private Tween doorTween;
    private void Start()
    {
        ToggleDoor();
    }
    private void Awake()
    {
        if (doorPivot == null)
            doorPivot = transform;

        closedRotation = doorPivot.localEulerAngles;
    }

    public void ToggleDoor()
    {
        doorTween?.Kill();

        Vector3 targetRotation = isOpen
            ? closedRotation
            : closedRotation + new Vector3(0f, 0f, openAngle);

        doorTween = doorPivot
            .DOLocalRotate(targetRotation, duration)
            .SetEase(Ease.OutCubic);

        isOpen = !isOpen;
    }
}
