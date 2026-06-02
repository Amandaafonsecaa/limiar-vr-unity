using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Pega o componente Animator no DoorPivot
        animator = GetComponent<Animator>();
    }

    // Quando algo ENTRAR na área verde (Trigger)
    void OnTriggerEnter(Collider other)
    {
        // Verifica se quem entrou tem a etiqueta "Player" (Jogador)
        if (other.CompareTag("Player"))
        {
            animator.SetBool("isOpen", true); // Abre a porta
        }
    }

    // Quando algo SAIR da área verde (Trigger)
    void OnTriggerExit(Collider other)
    {
        // Verifica se quem saiu foi o jogador
        if (other.CompareTag("Player"))
        {
            animator.SetBool("isOpen", false); // Fecha a porta
        }
    }
}