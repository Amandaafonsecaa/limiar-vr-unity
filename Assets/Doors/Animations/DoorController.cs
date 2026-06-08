using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        Debug.Log("AWAKE -> " + gameObject.name);

        if (animator != null)
            Debug.Log("Animator encontrado em: " + animator.gameObject.name);
        else
            Debug.LogError("Animator não encontrado!");
    }

    public void OpenDoor()
    {
        Debug.Log("OPEN DOOR -> " + gameObject.name);

        if (animator == null)
        {
            Debug.LogError("Animator é NULL!");
            return;
        }

        animator.SetBool("isOpen", true);
    }
}