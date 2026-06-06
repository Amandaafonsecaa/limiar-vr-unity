using UnityEngine;
using TMPro;
using System.Collections;

public class LegendaVR : MonoBehaviour
{
    [Header("Referências UI")]
    [SerializeField] private CanvasGroup captionCanvasGroup;
    [SerializeField] private TMP_Text captionText;

    [Header("Posicionamento VR")]
    public Transform cameraJogador;
    public float distanciaDoRosto = 2.0f;
    public float alturaLegenda = -0.3f;

    [Header("Timing Padrão")]
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float delayEntreLinhas = 0.2f;

    private Coroutine sequenciaAtiva;

    void Awake()
    {
        if (captionCanvasGroup != null) captionCanvasGroup.alpha = 0f;
        if (captionText != null) captionText.text = "";
    }

    void Start()
    {
        if (cameraJogador == null) cameraJogador = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (cameraJogador == null) return;

        // Posição: Mantém um deslize suave e rápido (30f) para evitar enjoo de movimento (Motion Sickness)
        Vector3 posicaoAlvo = cameraJogador.position
                            + cameraJogador.forward * distanciaDoRosto
                            + Vector3.up * alturaLegenda;

        transform.position = Vector3.Lerp(transform.position, posicaoAlvo, Time.deltaTime * 30f);

        // Rotação: 100% travada com os olhos do jogador de forma instantânea
        transform.rotation = cameraJogador.rotation;
    }

    public void MostrarSequencia(SubtitleTrigger.LinhaLegenda[] novasLinhas)
    {
        if (sequenciaAtiva != null) StopCoroutine(sequenciaAtiva);
        sequenciaAtiva = StartCoroutine(TocarSequencia(novasLinhas));
    }

    public void LimparLegenda()
    {
        if (sequenciaAtiva != null) StopCoroutine(sequenciaAtiva);
        StartCoroutine(EsconderComFade());
    }

    private IEnumerator TocarSequencia(SubtitleTrigger.LinhaLegenda[] linhasParaTocar)
    {
        foreach (SubtitleTrigger.LinhaLegenda linha in linhasParaTocar)
        {
            if (captionText != null) captionText.text = linha.texto;
            yield return FadeAlpha(0f, 1f);
            yield return new WaitForSeconds(linha.duracao);
            yield return FadeAlpha(1f, 0f);
            if (captionText != null) captionText.text = "";
            yield return new WaitForSeconds(delayEntreLinhas);
        }
    }

    private IEnumerator FadeAlpha(float de, float para)
    {
        if (captionCanvasGroup == null) yield break;
        float elapsed = 0f;
        captionCanvasGroup.alpha = de;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            captionCanvasGroup.alpha = Mathf.Lerp(de, para, elapsed / fadeDuration);
            yield return null;
        }
        captionCanvasGroup.alpha = para;
    }

    private IEnumerator EsconderComFade()
    {
        if (captionCanvasGroup != null)
            yield return FadeAlpha(captionCanvasGroup.alpha, 0f);
        if (captionText != null) captionText.text = "";
    }
}