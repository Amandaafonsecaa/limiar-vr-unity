using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonstroPerseguidorQuarto : MonoBehaviour
{
    [Header("Alvo (Arraste o XROrigin ou o objeto CameraOffset aqui)")]
    public Transform jogador;

    [Header("Configurações")]
    public float distanciaParaParar = 1.0f; 
    public float velocidadeSemNavMesh = 2.5f; // Velocidade dele caso o NavMesh trave na porta

    [Header("Áudio de Perseguição")]
    public AudioSource audioFalasLongas; 

    private NavMeshAgent agente;
    private Animator animator;
    private bool jaComecouFalar = false;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agente != null)
        {
            agente.updateRotation = false;
            agente.stoppingDistance = distanciaParaParar; 
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
        Debug.Log("Distância atual: " + distanciaAtual);

        if (distanciaAtual <= distanciaParaParar)
        {
            if (agente != null && agente.isOnNavMesh) agente.isStopped = true; 

            if (animator != null)
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", false); 
            }
        }
        else
        {
            // O TRUQUE PARA ATRAVESSAR A PORTA SEM TRAVAR:
            // Se o agente estiver ativo mas travado na porta (pathStatus parcial ou sem conseguir andar)
            if (agente != null && agente.isOnNavMesh && (agente.pathStatus == NavMeshPathStatus.PathPartial || !agente.hasPath))
            {
                // Ele ignora o cálculo do NavMesh e caminha direto na sua direção usando o Transform!
                transform.position = Vector3.MoveTowards(transform.position, jogador.position, velocidadeSemNavMesh * Time.deltaTime);
            }
            else if (agente != null && agente.isOnNavMesh)
            {
                // Se o caminho estiver livre, ele usa o NavMesh normal
                agente.isStopped = false;
                agente.SetDestination(jogador.position); 
            }

            if (animator != null)
            {
                animator.SetBool("isWalking", true);
            }
        }

        // Mantém o seu código original de rotação idêntico
        Vector3 posicaoOlhar = new Vector3(jogador.position.x, transform.position.y, jogador.position.z);
        transform.LookAt(posicaoOlhar);
    }
}