using UnityEngine;

public class AbrirPorta : MonoBehaviour
{
    [Header("Configurações da Porta")]
    [Tooltip("Arraste a própria porta para cá.")]
    public Transform objetoPorta;
    
    [Tooltip("O ângulo que a porta vai girar quando abrir. Geralmente 90 no Y.")]
    public Vector3 rotacaoBotaoAberto = new Vector3(0f, 90f, 0f);

    [Header("Áudio")]
    public AudioSource somPorta;

    [Header("Sistema de Legendas da Porta")]
    [SerializeField] private LegendaVR sistemaLegenda;
    
    [Tooltip("As linhas de fala que o personagem vai dizer assim que a porta abrir.")]
    public SubtitleTrigger.LinhaLegenda[] linhasAoAbrir;

    private Quaternion rstAlvo;
    private Quaternion rstOriginal;
    private bool abriu = false;

    void Start()
    {
        if (objetoPorta == null) objetoPorta = transform;
        rstOriginal = objetoPorta.localRotation;
        rstAlvo = rstOriginal;

        if (somPorta != null) somPorta.playOnAwake = false;
    }

    void Update()
    {
        // Faz a porta girar suavemente frame a frame sem precisar de DOTween
        objetoPorta.localRotation = Quaternion.Slerp(objetoPorta.localRotation, rstAlvo, Time.deltaTime * 5f);
    }

    // Abre a porta e dispara as falas quando o jogador entra na área
    private void OnTriggerEnter(Collider other)
    {
        // Garante que só vai responder ao comando do jogador/câmera VR
        if (other.CompareTag("Player") || other.GetComponent<Camera>() != null || other.name.Contains("Origin"))
        {
            if (abriu) return;
            
            rstAlvo = rstOriginal * Quaternion.Euler(rotacaoBotaoAberto);
            abriu = true;

            // 1. Toca o som físico da porta destrancando/abrindo
            if (somPorta != null) somPorta.Play();
            
            // 2. DISPARA AS FALAS (Se você configurou alguma no Inspector)
            if (sistemaLegenda != null && linhasAoAbrir != null && linhasAoAbrir.Length > 0)
            {
                sistemaLegenda.MostrarSequencia(linhasAoAbrir);
                Debug.Log($"[Porta] Abrindo e iniciando {linhasAoAbrir.Length} falas de legenda.");
            }

            Debug.Log("Porta abriu por aproximação!");
        }
    }

    // Fecha a porta quando o jogador sai da área
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<Camera>() != null || other.name.Contains("Origin"))
        {
            if (!abriu) return;

            rstAlvo = rstOriginal;
            abriu = false;

            if (somPorta != null) somPorta.Play();
            Debug.Log("Porta fechou quando o jogador se afastou!");
        }
    }
}