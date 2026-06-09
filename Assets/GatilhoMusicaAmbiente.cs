using UnityEngine;
using System.Collections;

public class GatilhoMusicaAmbiente : MonoBehaviour
{
    [Header("Configuração Geral")]
    [SerializeField] private LegendaVR sistemaLegenda;
    [SerializeField] private bool dispararApenasUmaVez = true;

    [Header("Configuração do Áudio (Música)")]
    [Tooltip("Arraste o arquivo de música de fundo aqui.")]
    [SerializeField] private AudioClip musicaAmbiente;
    [SerializeField] private float duracaoFadeIn = 2.0f;
    [SerializeField] private float duracaoFadeOut = 2.0f;
    [Range(0f, 1f)] [SerializeField] private float volumeMaximoMusica = 0.6f;

    [Header("Configuração da Narrativa")]
    [Tooltip("O tempo que o script espera após a música começar para soltar a legenda.")]
    [SerializeField] private float delayParaLegenda = 1.4f;
    public SubtitleTrigger.LinhaLegenda[] linhasDaFrase;

    private AudioSource audioSource;
    private bool jaDisparou = false;
    private Coroutine coroutineFade;

    private void Awake()
    {
        // Cria o componente de áudio local configurado para música ambiente (2D)
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0.0f; // 2D para envelopar o jogador na música
        
        // MUDANÇA AQUI: Agora está DESLIGADO o loop. O som toca uma vez e para.
        audioSource.loop = false; 
        
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // LINHA DE TESTE: Avisa no Console exatamente QUEM pisou no gatilho
        Debug.Log($"[Gatilho Chão] Algo entrou na área! Objeto: {other.name} | Tag: {other.tag}");

        // Detecta se quem pisou no chão foi o jogador ou a câmera VR
        if (other.CompareTag("Player") || other.GetComponent<Camera>() != null || other.name.Contains("Origin"))
        {
            if (jaDisparou && dispararApenasUmaVez) return;

            if (musicaAmbiente != null)
            {
                jaDisparou = true;
                Debug.Log("[Gatilho Chão] Alvo validado! Iniciando som de disparo único...");
                
                audioSource.clip = musicaAmbiente;
                audioSource.Play();
                
                if (coroutineFade != null) StopCoroutine(coroutineFade);
                coroutineFade = StartCoroutine(FadeVolume(audioSource, audioSource.volume, volumeMaximoMusica, duracaoFadeIn));

                StartCoroutine(AgendarLegenda());
            }
            else
            {
                Debug.LogWarning("[Gatilho Chão] ERRO: Você esqueceu de arrastar o arquivo de Música no Inspector!");
            }
        }
    }

    private IEnumerator AgendarLegenda()
    {
        yield return new WaitForSeconds(delayParaLegenda);

        if (sistemaLegenda != null && linhasDaFrase != null && linhasDaFrase.Length > 0)
        {
            sistemaLegenda.MostrarSequencia(linhasDaFrase);
            Debug.Log("[Gatilho Chão] Legenda enviada para o Canvas.");
        }
        else
        {
            Debug.LogWarning("[Gatilho Chão] ERRO: Sistema de Legenda ou as Linhas estão vazios no Inspector!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Se a opção "Disparar Apenas Uma Vez" estiver desmarcada e você sair correndo do quarto antes do som acabar, ele faz o fade out
        if (!dispararApenasUmaVez && (other.CompareTag("Player") || other.GetComponent<Camera>() != null || other.name.Contains("Origin")))
        {
            if (coroutineFade != null) StopCoroutine(coroutineFade);
            coroutineFade = StartCoroutine(FadeVolume(audioSource, audioSource.volume, 0f, duracaoFadeOut));
        }
    }

    private IEnumerator FadeVolume(AudioSource source, float volumeInicial, float volumeAlvo, float duracao)
    {
        float tempoMapeado = 0f;
        source.volume = volumeInicial;

        while (tempoMapeado < duracao)
        {
            tempoMapeado += Time.deltaTime;
            source.volume = Mathf.Lerp(volumeInicial, volumeAlvo, tempoMapeado / duracao);
            yield return null;
        }
        
        source.volume = volumeAlvo;
        if (volumeAlvo == 0f) source.Stop();
    }

    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            Gizmos.color = new Color(1f, 0f, 1f, 0.3f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
        }
    }
}