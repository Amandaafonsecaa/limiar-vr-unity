using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class PanicoPersonagem : MonoBehaviour
{
    [Header("Áudios")]
    public AudioClip clipeZumbido;
    public AudioClip clipeCoracao;
    public AudioClip clipeRespiracao;

    [Header("Sistema de Legenda VR")]
    public GameObject sistemaLegenda;
    public float tempoExibicaoLegenda = 4.0f;

    [Header("Referências de Sistema")]
    public Volume volumePostProcess;
    public Light luzAura;

    [Header("Ajustes de Intensidade")]
    [Range(0, 1)] public float volumeMaximo = 0.7f;
    public float intensidadeLuzNormal = 0.5f;
    public float intensidadeLuzPanico = 1.2f;

    private AudioSource sZumbido, sCoracao, sRespiracao;

    private ChromaticAberration chromatic;
    private LensDistortion lens;
    private ColorAdjustments colorAdj;
    private Vignette vignette;

    private float tempoEfeito;
    private bool efeitoAtivo = false;

    void Awake()
    {
        sZumbido    = gameObject.AddComponent<AudioSource>();
        sCoracao    = gameObject.AddComponent<AudioSource>();
        sRespiracao = gameObject.AddComponent<AudioSource>();

        ConfigurarFonte(sZumbido,    clipeZumbido,    true);
        ConfigurarFonte(sCoracao,    clipeCoracao,    true);
        ConfigurarFonte(sRespiracao, clipeRespiracao, false);
    }

    void Start()
    {
        if (volumePostProcess != null && volumePostProcess.profile != null)
        {
            volumePostProcess.profile.TryGet(out chromatic);
            volumePostProcess.profile.TryGet(out lens);
            volumePostProcess.profile.TryGet(out colorAdj);
            volumePostProcess.profile.TryGet(out vignette);
        }

        ResetarEfeitos();
        if (luzAura != null) luzAura.intensity = intensidadeLuzNormal;

        StartCoroutine(SequenciaCustomizada());
    }

    void Update()
    {
        if (efeitoAtivo && sCoracao != null && sCoracao.isPlaying)
        {
            tempoEfeito += Time.deltaTime;

            float balancoZ = Mathf.Sin(tempoEfeito * 1.5f) * 3.0f;
            transform.localRotation = Quaternion.Euler(0, 0, balancoZ);

            if (luzAura != null)
            {
                float pulso = Mathf.PingPong(tempoEfeito * 2f, 0.5f);
                luzAura.intensity = intensidadeLuzPanico + pulso;
            }
        }
    }

    IEnumerator SequenciaCustomizada()
    {
        // FASE 1 — Só o zumbido por 1.3 segundos
        sZumbido.volume = 0f;
        sZumbido.Play();
        StartCoroutine(FadeSom(sZumbido, volumeMaximo, 0.5f));
        yield return new WaitForSeconds(1.3f);

        // FASE 2 — Coração entra + efeitos visuais (1.0 s)
        sCoracao.volume = 0f;
        sCoracao.Play();
        StartCoroutine(FadeSom(sCoracao, volumeMaximo, 0.4f));
        StartCoroutine(FadeVisual(1.0f, 1.5f, 0.45f, 1.0f));
        efeitoAtivo = true;
        tempoEfeito = 0f;
        yield return new WaitForSeconds(1.0f);

        // FASE 3 — Respiração (2x) + Sequência de legendas
        StartCoroutine(TocarRespiracaoDoisLoops());

        if (sistemaLegenda != null)
        {
            LegendaVR scriptLegenda = sistemaLegenda.GetComponent<LegendaVR>();
            if (scriptLegenda != null)
                scriptLegenda.MostrarSequencia();
        }

        // Espera o tempo total definido no Inspector
        yield return new WaitForSeconds(tempoExibicaoLegenda);

        // FASE 4 — Fade out de tudo
        if (sistemaLegenda != null)
        {
            LegendaVR scriptLegenda = sistemaLegenda.GetComponent<LegendaVR>();
            if (scriptLegenda != null) scriptLegenda.LimparLegenda();
        }

        efeitoAtivo = false;
        StartCoroutine(FadeVisual(0f, 0f, 0f, 2.5f));
        StartCoroutine(FadeSom(sZumbido,    0f, 2.5f));
        StartCoroutine(FadeSom(sCoracao,    0f, 2.5f));
        StartCoroutine(FadeSom(sRespiracao, 0f, 2.5f));

        yield return new WaitForSeconds(2.6f);

        transform.localRotation = Quaternion.identity;
        if (luzAura != null) luzAura.intensity = intensidadeLuzNormal;
        PararTudo();
    }

    IEnumerator TocarRespiracaoDoisLoops()
    {
        if (sRespiracao == null || clipeRespiracao == null) yield break;
        sRespiracao.volume = 0f;
        for (int i = 0; i < 2; i++)
        {
            sRespiracao.Play();
            if (i == 0) StartCoroutine(FadeSom(sRespiracao, volumeMaximo, 0.3f));
            yield return new WaitForSeconds(clipeRespiracao.length);
        }
    }

    IEnumerator FadeVisual(float alvoCromatica, float alvoExp, float alvoVignette, float tempo)
    {
        float cI = chromatic ? chromatic.intensity.value   : 0;
        float eI = colorAdj  ? colorAdj.postExposure.value : 0;
        float vI = vignette  ? vignette.intensity.value    : 0;
        float lI = lens      ? lens.intensity.value        : 0;
        float t  = 0;
        while (t < tempo)
        {
            t += Time.deltaTime;
            float p = t / tempo;
            if (chromatic) chromatic.intensity.value   = Mathf.Lerp(cI, alvoCromatica, p);
            if (colorAdj)  colorAdj.postExposure.value = Mathf.Lerp(eI, alvoExp, p);
            if (vignette)  vignette.intensity.value    = Mathf.Lerp(vI, alvoVignette, p);
            if (lens)      lens.intensity.value        = Mathf.Lerp(lI, -0.35f * alvoCromatica, p);
            yield return null;
        }
    }

    IEnumerator FadeSom(AudioSource source, float alvo, float tempo)
    {
        if (source == null) yield break;
        float inicial = source.volume;
        for (float t = 0; t < tempo; t += Time.deltaTime)
        {
            if (source == null) yield break;
            source.volume = Mathf.Lerp(inicial, alvo, t / tempo);
            yield return null;
        }
        if (source != null) source.volume = alvo;
    }

    void ConfigurarFonte(AudioSource s, AudioClip c, bool l)
    {
        s.clip = c; s.loop = l; s.spatialBlend = 0; s.playOnAwake = false; s.volume = 0;
    }

    void PararTudo()
    {
        if (sZumbido    != null) sZumbido.Stop();
        if (sCoracao    != null) sCoracao.Stop();
        if (sRespiracao != null) sRespiracao.Stop();
    }

    void ResetarEfeitos()
    {
        if (chromatic) chromatic.intensity.value   = 0;
        if (colorAdj)  colorAdj.postExposure.value = 0;
        if (vignette)  vignette.intensity.value    = 0;
        if (lens)      lens.intensity.value        = 0;
    }
}