using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PickupTB : MonoBehaviour
{
    [Header("XR Grab")]
    [SerializeField] private XRGrabInteractable grabInteractable;

    [Header("Configuração da Legenda")]
    [SerializeField] private LegendaVR sistemaLegenda;
    [SerializeField] private bool dispararApenasUmaVez = true;

    [Header("Passo 1 - Legenda Inicial")]
    [Tooltip("A primeira frase que aparece assim que o jogador pega o urso.")]
    public SubtitleTrigger.LinhaLegenda[] legendaInicial;

    [Header("Passo 2 - Áudio Intermediário")]
    [Tooltip("O áudio de suspense ou memória que vai tocar após a primeira fala.")]
    [SerializeField] private AudioClip audioMemoria;
    [SerializeField] private float tempoMaximoAudio = 5.0f;

    [Header("Passo 3 - Legenda Final")]
    [Tooltip("A frase de reação que aparece depois que o áudio termina.")]
    public SubtitleTrigger.LinhaLegenda[] legendaFinal;

    private bool jaDisparou;
    private bool sequenciaRodando;
    private AudioSource audioSource;

    private void Awake()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 0.0f;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnBearGrabbed);
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnBearGrabbed);
    }

    private void OnBearGrabbed(SelectEnterEventArgs args)
    {
        if (sequenciaRodando)
            return;

        if (jaDisparou && dispararApenasUmaVez)
            return;

        if (sistemaLegenda == null)
        {
            Debug.LogWarning("[Urso] Sistema de legenda não configurado.");
            return;
        }

        jaDisparou = true;
        StartCoroutine(ExecutarSequenciaUrso());
    }

    private IEnumerator ExecutarSequenciaUrso()
    {
        sequenciaRodando = true;

        if (legendaInicial != null && legendaInicial.Length > 0)
        {
            sistemaLegenda.MostrarSequencia(legendaInicial);

            foreach (var linha in legendaInicial)
            {
                yield return new WaitForSeconds(linha.duracao + 0.2f);
            }
        }

        if (audioMemoria != null && audioSource != null)
        {
            audioSource.clip = audioMemoria;
            audioSource.volume = 0f;
            audioSource.Play();

            yield return StartCoroutine(FadeVolume(audioSource, 0f, 1f, 0.3f));

            float tempoDeEspera = Mathf.Min(tempoMaximoAudio, audioMemoria.length);
            tempoDeEspera = Mathf.Max(tempoDeEspera - 0.5f, 0f);

            yield return new WaitForSeconds(tempoDeEspera);

            yield return StartCoroutine(FadeVolume(audioSource, audioSource.volume, 0f, 0.5f));
            audioSource.Stop();
        }

        if (legendaFinal != null && legendaFinal.Length > 0)
        {
            sistemaLegenda.MostrarSequencia(legendaFinal);
            Debug.Log("[Urso] Sequência finalizada com sucesso.");
        }

        sequenciaRodando = false;
    }

    private IEnumerator FadeVolume(AudioSource source, float volumeInicial, float volumeAlvo, float duracao)
    {
        if (source == null)
            yield break;

        float tempo = 0f;
        source.volume = volumeInicial;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            source.volume = Mathf.Lerp(volumeInicial, volumeAlvo, tempo / duracao);
            yield return null;
        }

        source.volume = volumeAlvo;
    }
}