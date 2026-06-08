using UnityEngine;

public class GatilhoFlashbackMãe : MonoBehaviour
{
    [Header("Sistema de Legendas (Igual à Porta)")]
    [SerializeField] private LegendaVR sistemaLegenda;
    
    [Tooltip("As linhas de fala que a mãe/mente vão dizer durante o flashback.")]
    public SubtitleTrigger.LinhaLegenda[] linhasDoFlashback;

    [Header("Canal 1: Furadeira (Trepanação)")]
    public AudioSource somFuradeira;
    [Range(0f, 1f)] public float volumeFuradeira = 0.8f;

    [Header("Canal 2: Choro (Desespero Físico)")]
    public AudioSource somChoro;
    [Range(0f, 1f)] public float volumeChoro = 0.7f;

    [Header("Canal 3: Sussurro (Mente Perturbada)")]
    public AudioSource somSussurro;
    [Range(0f, 1f)] public float volumeSussurro = 0.5f;

    private bool jaDisparou = false;

    void Start()
    {
        // Garante que nenhum som saia tocando sozinho antes da hora ao iniciar o jogo
        ConfigurarAudioSource(somFuradeira);
        ConfigurarAudioSource(somChoro);
        ConfigurarAudioSource(somSussurro);
    }

    private void ConfigurarAudioSource(AudioSource source)
    {
        if (source != null)
        {
            source.playOnAwake = false;
        }
    }

    // Dispara quando o jogador passa pela área/porta com o colisor
    private void OnTriggerEnter(Collider other)
    {
        // Mesma verificação que você usa na porta para o jogador VR
        if (other.CompareTag("Player") || other.GetComponent<Camera>() != null || other.name.Contains("Origin"))
        {
            // Garante que o flashback e o trauma só aconteçam uma vez
            if (jaDisparou) return;
            jaDisparou = true;

            Debug.Log("[Flashback] Jogador cruzou o gatilho! Iniciando áudios e legendas combinados.");

            // 1. Aplica os volumes definidos por você no Inspector e toca os 3 áudios juntos
            DispararSom(somFuradeira, volumeFuradeira);
            DispararSom(somChoro, volumeChoro);
            DispararSom(somSussurro, volumeSussurro);

            // 2. Dispara a sequência de legendas exatamente como na porta
            if (sistemaLegenda != null && linhasDoFlashback != null && linhasDoFlashback.Length > 0)
            {
                sistemaLegenda.MostrarSequencia(linhasDoFlashback);
            }
            else
            {
                Debug.LogWarning("[Flashback] Falta arrastar o sistema de legendas ou configurar as frases.");
            }
        }
    }

    private void DispararSom(AudioSource source, float volumeAlvo)
    {
        if (source != null)
        {
            source.volume = volumeAlvo;
            source.Play();
        }
    }
}