using UnityEngine;
using System.Collections;

public class PickupTB : MonoBehaviour
{
    [Header("Configuração da Legenda")]
    [SerializeField] private LegendaVR sistemaLegenda;
    [SerializeField] private bool dispararApenasUmaVez = true;

    [Header("Passo 1 - Legenda Inicial")]
    [Tooltip("A primeira frase que aparece assim que o jogador pega o urso.")]
    public SubtitleTrigger.LinhaLegenda[] legendaInicial;

    [Header("Passo 2 - Áudio Intermediário (Com Corte e Fade)")]
    [Tooltip("O áudio de suspense ou memória que vai tocar após a primeira fala.")]
    [SerializeField] private AudioClip audioMemoria;
    [SerializeField] private float tempoMaximoAudio = 5.0f; // Corta cravado em 5 segundos

    [Header("Passo 3 - Legenda Final")]
    [Tooltip("A frase de reação que aparece DEPOIS que o áudio de 5 segundos termina.")]
    public SubtitleTrigger.LinhaLegenda[] legendaFinal;

    private bool jaDisparou = false;
    private AudioSource audioSource;

    private void Awake()
    {
        // Cria o componente de áudio focado em som de pensamento/memória (2D)
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0.0f; // 2D para dar a sensação de voz interna
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<Camera>() != null || other.name.Contains("Origin"))
        {
            if (jaDisparou && dispararApenasUmaVez) return;

            if (sistemaLegenda != null)
            {
                jaDisparou = true;
                StartCoroutine(ExecutarSequenciaUrso());
            }
        }
    }

    private IEnumerator ExecutarSequenciaUrso()
    {
        // ==========================================
        // PASSO 1: Mostra a primeira legenda e espera ela sumir
        // ==========================================
        if (legendaInicial != null && legendaInicial.Length > 0)
        {
            sistemaLegenda.MostrarSequencia(legendaInicial);
            foreach (var linha in legendaInicial)
            {
                yield return new WaitForSeconds(linha.duracao + 0.2f);
            }
        }

        // ==========================================
        // PASSO 2: Toca o áudio com Fade In, espera 5s e dá Fade Out
        // ==========================================
        if (audioMemoria != null && audioSource != null)
        {
            audioSource.clip = audioMemoria;
            
            // Fade In ultra rápido de 0.3 segundos para não estalar o som
            StartCoroutine(FadeVolume(audioSource, 0f, 1.0f, 0.3f));
            audioSource.Play();

            // Espera o tempo seguro rodar (5 segundos) tirando o tempo do Fade Out final
            float tempoDeEsperaEfetivo = Mathf.Min(tempoMaximoAudio, audioMemoria.length);
            yield return new WaitForSeconds(tempoDeEsperaEfetivo - 0.5f);

            // Fade Out de 0.5 segundos para sumir suavemente antes da reação
            yield return StartCoroutine(FadeVolume(audioSource, audioSource.volume, 0f, 0.5f));
            audioSource.Stop();
        }

        // ==========================================
        // PASSO 3: Solta a legenda de reação final
        // ==========================================
        if (legendaFinal != null && legendaFinal.Length > 0)
        {
            sistemaLegenda.MostrarSequencia(legendaFinal);
            Debug.Log("[Urso] Sequência finalizada com sucesso.");
        }
    }

    // Corrotina utilitária matemática para fazer o volume subir e descer suavemente
    private IEnumerator FadeVolume(AudioSource source, float volumeInicial, float volumeAlvo, float duracao)
    {
        if (source == null) yield break;
        float tempoMapeado = 0f;
        source.volume = volumeInicial;

        while (tempoMapeado < duracao)
        {
            if (source == null) yield break;
            tempoMapeado += Time.deltaTime;
            source.volume = Mathf.Lerp(volumeInicial, volumeAlvo, tempoMapeado / duracao);
            yield return null;
        }
        source.volume = volumeAlvo;
    }

    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            Gizmos.color = new Color(0f, 0.5f, 1f, 0.4f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
        }
    }
}