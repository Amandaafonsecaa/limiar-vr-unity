using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonstroAcao : MonoBehaviour
{
    [Header("Alvo (Arraste o XROrigin ou o objeto CameraOffset aqui)")]
    public Transform jogador;

    [Header("Configurações")]
    public float distanciaParaParar = 1.0f; 
    public float velocidadeSemNavMesh = 2.5f; 

    [Header("Áudio de Perseguição")]
    public AudioSource audioFalasLongas; 

    private NavMeshAgent agente;
    private Animator animator;
    private bool jaComecouFalar = false;

    // 1. O Awake roda ANTES de tudo, garantindo que a Unity ache o NavMesh e o Animator
    void Awake()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agente != null)
        {
            agente.updateRotation = false;
            agente.stoppingDistance = distanciaParaParar; 
        }
    }

    // 2. O OnEnable roda NO EXATO SEGUNDO em que o interruptor dá SetActive(true)
    void OnEnable()
    {
        // Força o agente a se situar no mapa para não bugar
        if (agente != null && agente.isOnNavMesh)
        {
            agente.isStopped = false;
        }

        // Liga o áudio no momento do susto
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
            // Se o agente sumir do NavMesh por um frame ao spawnar, o MoveTowards força ele a andar
            if (agente == null || !agente.isOnNavMesh || agente.pathStatus == NavMeshPathStatus.PathPartial || !agente.hasPath)
            {
                transform.position = Vector3.MoveTowards(transform.position, jogador.position, velocidadeSemNavMesh * Time.deltaTime);
            }
            else
            {
                agente.isStopped = false;
                agente.SetDestination(jogador.position); 
            }

            if (animator != null)
            {
                animator.SetBool("isWalking", true);
            }
        }

        Vector3 posicaoOlhar = new Vector3(jogador.position.x, transform.position.y, jogador.position.z);
        transform.LookAt(posicaoOlhar);
    }
}