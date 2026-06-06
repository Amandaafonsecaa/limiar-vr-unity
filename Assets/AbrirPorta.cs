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

    private Quaternion rotacaoFechada;
    private Quaternion rstAlvo;
    private bool abriu = false;

    void Start()
    {
        // Salva a rotação inicial automática da porta
        if (objetoPorta == null) objetoPorta = transform;
        rotacaoFechada = objetoPorta.localRotation;
        rstAlvo = rotacaoFechada;

        if (somPorta != null) somPorta.playOnAwake = false;
    }

    void Update()
    {
        // Faz a porta girar suavemente frame a frame sem precisar de DOTween
        objetoPorta.localRotation = Quaternion.Slerp(objetoPorta.localRotation, rstAlvo, Time.deltaTime * 5f);
    }

    // Abre a porta quando o jogador entra na área
    private void OnTriggerEnter(Collider other)
    {
        if (abriu) return;
        
        rstAlvo = rotacaoFechada * Quaternion.Euler(rotacaoBotaoAberto);
        abriu = true;

        if (somPorta != null) somPorta.Play();
        Debug.Log("Porta abriu por aproximação!");
    }

    // Fecha a porta quando o jogador sai da área
    private void OnTriggerExit(Collider other)
    {
        if (!abriu) return;

        rstAlvo = rotacaoFechada;
        abriu = false;

        if (somPorta != null) somPorta.Play();
        Debug.Log("Porta fechou quando o jogador se afastou!");
    }
}