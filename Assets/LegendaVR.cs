using UnityEngine;
using TMPro;
using System.Collections;

public class LegendaVR : MonoBehaviour
{
    [System.Serializable]
    public class LinhaLegenda
    {
        [TextArea(2, 4)]
        public string texto;
        public float duracao = 2.5f;
    }

    [Header("Referências UI")]
    [SerializeField] private CanvasGroup captionCanvasGroup;
    [SerializeField] private TMP_Text captionText;

    [Header("Linhas de Legenda")]
    public LinhaLegenda[] linhas;

    [Header("Posicionamento VR")]
    public Transform cameraJogador;
    public float distanciaDoRosto = 2.0f;
    public float alturaLegenda = -0.3f;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float delayEntreLinhas = 0.2f;

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

        // Posiciona exatamente na frente da câmera, centralizado
        Vector3 posicaoAlvo = cameraJogador.position
                            + cameraJogador.forward * distanciaDoRosto
                            + Vector3.up * alturaLegenda;

        transform.position = Vector3.Lerp(transform.position, posicaoAlvo, Time.deltaTime * 10f);

        // Fica paralelo ao rosto do jogador (não aponta para ele, fica de frente)
        transform.rotation = Quaternion.Lerp(transform.rotation, cameraJogador.rotation, Time.deltaTime * 10f);
    }

    public void MostrarSequencia()
    {
        StartCoroutine(TocarSequencia());
    }

    public void LimparLegenda()
    {
        StartCoroutine(EsconderComFade());
    }

    private IEnumerator TocarSequencia()
    {
        foreach (LinhaLegenda linha in linhas)
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