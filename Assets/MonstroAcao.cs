using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonstroPerseguidorQuarto : MonoBehaviour
{
    [Header("Alvo (Arraste o XROrigin ou Main Camera aqui)")]
    public Transform jogador;

    [Header("Configurações")]
    public float distanciaParaAtacar = 1.8f; 

    [Header("Áudio de Perseguição")]
    public AudioSource audioFalasLongas; 

    private NavMeshAgent agente;
    private Animator animator;
    private bool jaComecouFalar = false;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (jogador == null && Camera.main != null)
        {
            jogador = Camera.main.transform;
        }

        // O PULO DO GATO: Desliga a rotação automática do agente para não dar conflito com o LookAt
        if (agente != null)
        {
            agente.updateRotation = false;
        }

        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }

        if (audioFalasLongas != null && !jaComecouFalar)
        {
            audioFalasLongas.loop = true;
            audioFalasLongas.Play();
            jaComecouFalar = true;
        }
    }

    void Update()
    {
        if (jogador == null) return;

        float distanciaAtual = Vector3.Distance(transform.position, jogador.position);

        if (distanciaAtual <= distanciaParaAtacar)
        {
            agente.isStopped = true; 

            if (animator != null)
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", true); 
            }
        }
        else
        {
            agente.isStopped = false;
            agente.SetDestination(jogador.position); 

            if (animator != null)
            {
                animator.SetBool("isAttacking", false);
                animator.SetBool("isWalking", true);
            }
        }

        // Agora o LookAt roda livre sem travar as pernas do monstro!
        Vector3 posicaoOlhar = new Vector3(jogador.position.x, transform.position.y, jogador.position.z);
        transform.LookAt(posicaoOlhar);
    }
}