using UnityEngine;
using System.Collections;

public class SequentialEventTrigger : MonoBehaviour
{
    [Header("Configuração Geral")]
    [SerializeField] private LegendaVR sistemaLegenda;
    [SerializeField] private bool dispararApenasUmaVez = true;

    [Header("Passo 1 - Primeiras Falas")]
    public SubtitleTrigger.LinhaLegenda[] primeirasLinhas;

    [Header("Passo 2 - O Susto + Coração Inicial (IMEDIATOS)")]
    [Tooltip("Arraste aqui o AudioSource do Objeto 3D se quiser passos/sussurros vindos do cenário.")]
    [SerializeField] private AudioSource audioSourceSustoExterno;
    [SerializeField] private AudioClip somDoSustoBackup;
    [SerializeField] private float tempoMaximoSusto = 3.0f; 
    [SerializeField] private float delayAntesDoSusto = 0.5f;
    
    [Tooltip("O som do coração/respiração que vai disparar JUNTO com o susto, sem delay.")]
    [SerializeField] private AudioClip somFundoReacao; 

    [Header("Passo 3 - Falas de Reação (Pós-Susto)")]
    public SubtitleTrigger.LinhaLegenda[] segundasLinhas;
    [SerializeField] private float delayAntesDasSegundasLinhas = 0.5f;

    private bool jaDisparou = false;
    private AudioSource audioSourceLocal;

    private void Awake()
    {
        audioSourceLocal = gameObject.AddComponent<AudioSource>();
        audioSourceLocal.spatialBlend = 0.0f; 
        audioSourceLocal.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<Camera>() != null || other.name.Contains("Origin"))
        {
            if (jaDisparou && dispararApenasUmaVez) return;

            if (sistemaLegenda != null)
            {
                jaDisparou = true;

                PanicoPersonagem panico = other.GetComponentInChildren<PanicoPersonagem>();
                if (panico == null) panico = Object.FindFirstObjectByType<PanicoPersonagem>();

                if (panico != null)
                {
                    panico.StopAllCoroutines();
                    panico.Invoke("PararTudo", 0f); 
                    Debug.Log("[Gatilho Mesa] Parando os áudios do pânico inicial.");
                }

                StartCoroutine(ExecutarSequenciaCine());
            }
        }
    }

    private IEnumerator ExecutarSequenciaCine()
    {
        // ==========================================
        // PASSO 1: O personagem fala as primeiras linhas
        // ==========================================
        if (primeirasLinhas != null && primeirasLinhas.Length > 0)
        {
            sistemaLegenda.MostrarSequencia(primeirasLinhas);
            foreach (var linha in primeirasLinhas)
            {
                yield return new WaitForSeconds(linha.duracao + 0.2f);
            }
        }

        // ==========================================
        // PASSO 2: Susto + Coração entram no MESMO MILISSEGUNDO
        // ==========================================
        yield return new WaitForSeconds(delayAntesDoSusto);

        // 1. Liga o coração de fundo IMEDIATAMENTE (Local e em loop)
        if (somFundoReacao != null)
        {
            audioSourceLocal.clip = somFundoReacao;
            audioSourceLocal.volume = 0f;
            audioSourceLocal.loop = true;
            audioSourceLocal.Play();
            StartCoroutine(FadeVolume(audioSourceLocal, 0f, 0.8f, 0.2f)); // Fade in ultra rápido
        }

        // 2. Dispara o som do susto no mesmo instante
        AudioSource sourceSusto = (audioSourceSustoExterno != null) ? audioSourceSustoExterno : audioSourceLocal;
        
        if (somDoSustoBackup != null && audioSourceSustoExterno == null)
        {
            sourceSusto.clip = somDoSustoBackup;
        }

        if (sourceSusto != null && sourceSusto.clip != null)
        {
            // Se o susto for local, usamos o PlayOneShot para não cortar o coração que acabou de dar Play
            if (sourceSusto == audioSourceLocal)
            {
                audioSourceLocal.PlayOneShot(somDoSustoBackup);
            }
            else
            {
                StartCoroutine(FadeVolume(sourceSusto, 0f, 1.0f, 0.1f));
                sourceSusto.Play();
            }

            // Espera o susto acontecer enquanto o coração já está bombando de fundo
            float tempoDeEsperaEfetivo = Mathf.Min(tempoMaximoSusto, sourceSusto.clip.length);
            yield return new WaitForSeconds(tempoDeEsperaEfetivo);

            // Se for áudio externo, para ele com fade
            if (audioSourceSustoExterno != null)
            {
                yield return StartCoroutine(FadeVolume(sourceSusto, sourceSusto.volume, 0f, 0.4f));
                sourceSusto.Stop();
            }
        }

        // ==========================================
        // PASSO 3: Legendas de Reação aparecem (Coração continua tocando)
        // ==========================================
        yield return new WaitForSeconds(delayAntesDasSegundasLinhas);

        if (segundasLinhas != null && segundasLinhas.Length > 0)
        {
            sistemaLegenda.MostrarSequencia(segundasLinhas);
        }
    }

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
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
        }
    }
}