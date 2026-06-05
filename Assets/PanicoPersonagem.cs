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
    public AudioClip clipeDublagem;

    [Header("Referências de Sistema")]
    public Volume volumePostProcess;
    public Light luzAura; // Arraste a Point Light que está na câmera para cá

    [Header("Ajustes de Intensidade")]
    [Range(0, 1)] public float volumeMaximo = 0.7f;
    public float intensidadeLuzNormal = 0.5f;
    public float intensidadeLuzPanico = 1.2f;

    private AudioSource sZumbido, sCoracao, sRespiracao, sDublagem;
    
    // Variáveis internas para manipulação direta dos componentes do Volume
    private ChromaticAberration chromatic;
    private LensDistortion lens;
    private ColorAdjustments colorAdj;
    private Vignette vignette;

    private float tempoEfeito;

    void Start()
    {
        // Força a busca direta no perfil do Volume atribuído
        if (volumePostProcess != null && volumePostProcess.profile != null)
        {
            volumePostProcess.profile.TryGet(out chromatic);
            volumePostProcess.profile.TryGet(out lens);
            volumePostProcess.profile.TryGet(out colorAdj);
            volumePostProcess.profile.TryGet(out vignette);
            
            // Alerta visual no Console para você ter certeza se ele achou os efeitos
            Debug.Log($"[Volume] Chromatic: {chromatic != null} | Lens: {lens != null} | Color: {colorAdj != null} | Vignette: {vignette != null}");
        }
        else
        {
            Debug.LogError("🚨 [Volume] O componente Global Volume não foi arrastado para o Inspector!");
        }

        ResetarEfeitos();

        if (luzAura != null) luzAura.intensity = intensidadeLuzNormal;

        // Cria os AudioSources dinamicamente
        sZumbido = gameObject.AddComponent<AudioSource>();
        sCoracao = gameObject.AddComponent<AudioSource>();
        sRespiracao = gameObject.AddComponent<AudioSource>();
        sDublagem = gameObject.AddComponent<AudioSource>();

        ConfigurarFonte(sZumbido, clipeZumbido, true);
        ConfigurarFonte(sCoracao, clipeCoracao, true);
        ConfigurarFonte(sRespiracao, clipeRespiracao, true);
        ConfigurarFonte(sDublagem, clipeDublagem, false);

        StartCoroutine(SequenciaImersiva());
    }

    void Update()
    {
        if (sCoracao != null && sCoracao.isPlaying && sCoracao.volume > 0.1f)
        {
            tempoEfeito += Time.deltaTime;

            // Balanço de Câmera (Efeito estonteante no VR)
            float balancoZ = Mathf.Sin(tempoEfeito * 1.5f) * 3.0f; 
            transform.localRotation = Quaternion.Euler(0, 0, balancoZ);

            // Luz Pulsante na visão do jogador
            if (luzAura != null)
            {
                float pulso = Mathf.PingPong(tempoEfeito * 2f, 0.5f);
                luzAura.intensity = intensidadeLuzPanico + pulso;
            }
        }
    }

    IEnumerator SequenciaImersiva()
    {
        // --- ETAPA 1: ZUMBIDO ---
        sZumbido.Play();
        StartCoroutine(FadeSom(sZumbido, volumeMaximo, 1.5f)); 
        yield return new WaitForSeconds(1.5f);

        // --- ETAPA 2: CORAÇÃO + EFEITOS VISUAIS EM AÇÃO ---
        sCoracao.Play();
        StartCoroutine(FadeSom(sCoracao, volumeMaximo, 1.5f));
        
        // Ativa os efeitos visuais progressivamente em 3 segundos
        StartCoroutine(FadeVisual(1.0f, 1.5f, 0.45f, 3.0f)); 

        yield return new WaitForSeconds(1.5f);
        
        // --- ETAPA 3: RESPIRAÇÃO ---
        sRespiracao.Play();
        StartCoroutine(FadeSom(sRespiracao, volumeMaximo, 1.0f));

        yield return new WaitForSeconds(0.2f);
        
        // --- ETAPA 4: DUBLAGEM ---
        if (clipeDublagem != null)
        {
            sDublagem.volume = 1.0f;
            sDublagem.Play();
            yield return new WaitForSeconds(clipeDublagem.length);
        }

        // --- ETAPA 5: FINALIZAÇÃO (FADE OUT TOTAL) ---
        StartCoroutine(FadeVisual(0f, 0f, 0f, 3.0f)); 
        StartCoroutine(FadeSom(sZumbido, 0, 3.0f));
        StartCoroutine(FadeSom(sCoracao, 0, 3.0f));
        StartCoroutine(FadeSom(sRespiracao, 0, 3.0f));

        yield return new WaitForSeconds(3.1f);
        
        // Limpeza e estabilização do VR
        transform.localRotation = Quaternion.identity;
        if (luzAura != null) luzAura.intensity = intensidadeLuzNormal;
        PararTudo();
    }

    IEnumerator FadeVisual(float alvoCromatica, float alvoExp, float alvoVignette, float tempo)
    {
        float cI = chromatic ? chromatic.intensity.value : 0;
        float eI = colorAdj ? colorAdj.postExposure.value : 0;
        float vI = vignette ? vignette.intensity.value : 0;
        float lI = lens ? lens.intensity.value : 0;
        float t = 0;

        while (t < tempo)
        {
            t += Time.deltaTime;
            float p = t / tempo;

            if (chromatic) chromatic.intensity.value = Mathf.Lerp(cI, alvoCromatica, p);
            if (colorAdj) colorAdj.postExposure.value = Mathf.Lerp(eI, alvoExp, p);
            if (vignette) vignette.intensity.value = Mathf.Lerp(vI, alvoVignette, p);
            if (lens) lens.intensity.value = Mathf.Lerp(lI, -0.35f * (alvoCromatica), p);

            yield return null;
        }
    }

    IEnumerator FadeSom(AudioSource source, float alvo, float tempo)
    {
        float inicial = source.volume;
        for (float t = 0; t < tempo; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(inicial, alvo, t / tempo);
            yield return null;
        }
        source.volume = alvo;
    }

    void ConfigurarFonte(AudioSource s, AudioClip c, bool l) {
        s.clip = c; s.loop = l; s.spatialBlend = 0; s.playOnAwake = false; s.volume = 0;
    }

    void PararTudo() { 
        if (sZumbido != null) sZumbido.Stop(); 
        if (sCoracao != null) sCoracao.Stop(); 
        if (sRespiracao != null) sRespiracao.Stop(); 
    }

    void ResetarEfeitos() {
        if (chromatic) chromatic.intensity.value = 0;
        if (colorAdj) colorAdj.postExposure.value = 0;
        if (vignette) vignette.intensity.value = 0;
        if (lens) lens.intensity.value = 0;
    }
}