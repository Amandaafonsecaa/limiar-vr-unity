using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class PhysicsOnGrab : MonoBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private Transform originalParent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        originalParent = transform.parent;

        rb.useGravity = false;
        rb.isKinematic = true;
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.WakeUp();
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        StartCoroutine(ReleasePhysicsNextFrame());
    }

    private IEnumerator ReleasePhysicsNextFrame()
    {
        yield return null;

        transform.SetParent(originalParent, true);

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.WakeUp();

        Physics.SyncTransforms();

        Debug.Log("Urso solto e física ativada.");
    }
}