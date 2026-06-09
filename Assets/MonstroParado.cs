using UnityEngine;

public class MonstroParado : MonoBehaviour
{
    [Header("Som de Visão (Olhou pro Monstro)")]
    public AudioSource somAoOlhar; 

    [Header("Som do Susto (Ao Sumir)")]
    public AudioSource somAoSumir; 

    [Header("Configurações de Coordenadas e Distância")]
    [Tooltip("Distância máxima em metros para ativar o som do olhar.")]
    public float distanciaParaOlhar = 8f;
    [Tooltip("Distância de colisão em metros (substitui o Trigger se necessário).")]
    public float distanciaParaColidir = 2f;

    private Animator animator;
    private Camera cameraJogador;
    private bool jaSumiu = false;
    private bool jaTocouSomVisao = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        cameraJogador = Camera.main;

        // Força os áudios a iniciarem limpos e desligados
        if (somAoOlhar != null) { somAoOlhar.playOnAwake = false; somAoOlhar.Stop(); }
        if (somAoSumir != null) { somAoSumir.playOnAwake = false; somAoSumir.Stop(); }

        if (animator != null)
        {
            animator.Play("Idle", 0); 
        }
    }

    void Update()
    {
        if (jaSumiu || cameraJogador == null) return;

        // 1. PEGA AS COORDENADAS: Calcula a distância real em metros entre o jogador e o monstro
        float distanciaReal = Vector3.Distance(cameraJogador.transform.position, transform.position);

        // CHECAGEM 1: Se o jogador chegar muito perto (colisão por coordenadas), o monstro some
        if (distanciaReal <= distanciaParaColidir)
        {
            SumirEFecharObjeto();
            return;
        }

        // CHECAGEM 2: Se o jogador estiver no raio de visão e ainda não ouviu o som do olhar
        if (!jaTocouSomVisao && distanciaReal <= distanciaParaOlhar)
        {
            VerificarOlharDoJogador();
        }
    }

    void VerificarOlharDoJogador()
    {
        // Calcula a direção exata da câmera até a coordenada do monstro
        Vector3 direcaoParaMonstro = (transform.position - cameraJogador.transform.position).normalized;
        
        // Compara com a direção para onde a lente do VR está apontada
        float olhandoParaMonstro = Vector3.Dot(cameraJogador.transform.forward, direcaoParaMonstro);

        // 0.75 significa que o monstro está enquadrado no campo visual do jogador
        if (olhandoParaMonstro > 0.75f)
        {
            // Raio laser (Raycast) de segurança para checar se não há paredes no caminho
            RaycastHit hit;
            Vector3 inicioRaio = cameraJogador.transform.position;
            Vector3 direcaoRaio = transform.position - inicioRaio;

            if (Physics.Raycast(inicioRaio, direcaoRaio, out hit, distanciaParaOlhar))
            {
                // Se o raio atingir o próprio monstro, o susto é disparado!
                if (hit.transform == transform || hit.transform.IsChildOf(transform))
                {
                    DispararSomVisao();
                }
            }
        }
    }

    void DispararSomVisao()
    {
        jaTocouSomVisao = true;
        if (somAoOlhar != null)
        {
            somAoOlhar.Play();
            Debug.Log($"[Susto] Jogador avistou o monstro via coordenadas mundiais!");
        }
    }

    // Mantido por segurança caso você ainda queira usar colisores físicos na cena
    private void OnTriggerEnter(Collider other)
    {
        if (!jaSumiu)
        {
            SumirEFecharObjeto();
        }
    }

    void SumirEFecharObjeto()
    {
        jaSumiu = true;

        if (somAoSumir != null)
        {
            somAoSumir.Play();
            Debug.Log("[Susto] Monstro destruído por proximidade física.");
        }

        float tempoSomSumir = somAoSumir != null ? somAoSumir.clip.length : 0f;
        float tempoSomVisao = (somAoOlhar != null && somAoOlhar.isPlaying) ? (somAoOlhar.clip.length - somAoOlhar.time) : 0f;
        float tempoTotalDeVida = Mathf.Max(tempoSomSumir, tempoSomVisao);

        DesativarVisualDoMonstro();
        Destroy(gameObject, tempoTotalDeVida);
    }

    void DesativarVisualDoMonstro()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers) r.enabled = false;

        Collider[] colisores = GetComponentsInChildren<Collider>();
        foreach (Collider c in colisores) c.enabled = false;
    }
}