using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>(true);

        if (animator == null)
            Debug.LogError("Animator é NULL em: " + gameObject.name);
    }

    public void OpenDoor()
    {
        if (animator == null)
        {
            Debug.LogError("Animator é NULL!");
            return;
        }

        animator.SetBool("isOpen", true);
    }
}