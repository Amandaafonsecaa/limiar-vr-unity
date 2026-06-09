using UnityEngine;
using System.Collections;

public class AbrirPortaComLoop : MonoBehaviour
{
    [Header("Configurações da Porta")]
    [Tooltip("Arraste a própria porta para cá.")]
    public Transform objetoPorta;
    
    [Tooltip("O ângulo que a porta vai girar quando abrir. Geralmente 90 no Y.")]
    public Vector3 rotacaoBotaoAberto = new Vector3(0f, 90f, 0f);

    [Header("Áudio Único (Ação da Porta)")]
    [Tooltip("Arraste o AudioSource do ranger/tranca da porta.")]
    public AudioSource somPortaAcao;

    [Header("Áudio Externo (Loop do Quarto)")]
    [Tooltip("O som de ressonância/mistério que o jogador vai ficar ouvindo lá dentro.")]
    public AudioSource somExternoLoop;

    [Header("Sistema de Legendas")]
    [SerializeField] private LegendaVR sistemaLegenda;
    public SubtitleTrigger.LinhaLegenda[] linhasAoAbrir;

    private Quaternion rstAlvo;
    private Quaternion rstOriginal;
    
    // Controle de estado limpo e sem avisos
    private bool jogadorEstaDentroDoQuarto = false;
    private bool primeiroTrancoFeito = false;

    void Start()
    {
        if (objetoPorta == null) objetoPorta = transform;
        rstOriginal = objetoPorta.localRotation;
        rstAlvo = rstOriginal;

        if (somPortaAcao != null) somPortaAcao.playOnAwake = false;

        if (somExternoLoop != null) 
        {
            somExternoLoop.playOnAwake = false;
            somExternoLoop.loop = true; 
        }
    }

    void Update()
    {
        objetoPorta.localRotation = Quaternion.Slerp(objetoPorta.localRotation, rstAlvo, Time.deltaTime * 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<Camera>() != null || other.name.Contains("Origin"))
        {
            // CENA 1: O jogador está entrando no quarto pela primeira vez
            if (!jogadorEstaDentroDoQuarto && !primeiroTrancoFeito)
            {
                jogadorEstaDentroDoQuarto = true;
                primeiroTrancoFeito = true;

                // Abre a porta
                rstAlvo = rstOriginal * Quaternion.Euler(rotacaoBotaoAberto);
                if (somPortaAcao != null) somPortaAcao.Play();
                
                // Liga o looping ensurdecedor do quarto
                if (somExternoLoop != null) somExternoLoop.Play();
                
                // Mostra a legenda revoltada da personagem
                if (sistemaLegenda != null && linhasAoAbrir != null && linesAoAbrirValidas())
                {
                    sistemaLegenda.MostrarSequencia(linhasAoAbrir);
                }

                // Fecha a porta atrás dele após 1.5 segundos
                StartCoroutine(FecharPortaAtrasDoJogador());
            }
            // CENA 2: O jogador já explorou o quarto e encostou na porta de novo para SAIR
            else if (jogadorEstaDentroDoQuarto && primeiroTrancoFeito)
            {
                // Abre a porta para ele escapar
                rstAlvo = rstOriginal * Quaternion.Euler(rotacaoBotaoAberto);
                if (somPortaAcao != null) somPortaAcao.Play();

                // DESLIGA O SOM DO QUARTO IMEDIATAMENTE!
                if (somExternoLoop != null)
                {
                    somExternoLoop.Stop();
                    Debug.Log("[Porta] Jogador solicitou a saída. Som em loop desligado.");
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<Camera>() != null || other.name.Contains("Origin"))
        {
            // Se o jogador de fato cruzou o portal para ir embora (SAIU do colisor vindo de dentro)
            if (jogadorEstaDentroDoQuarto && rstAlvo != rstOriginal)
            {
                jogadorEstaDentroDoQuarto = false;
                StartCoroutine(FecharPortaAoAfastar());
            }
        }
    }

    private IEnumerator FecharPortaAtrasDoJogador()
    {
        yield return new WaitForSeconds(1.5f);
        rstAlvo = rstOriginal; 
        if (somPortaAcao != null) somPortaAcao.Play();
        Debug.Log("[Porta] Trancado! O som do quarto vai continuar tocando em loop.");
    }

    private IEnumerator FecharPortaAoAfastar()
    {
        yield return new WaitForSeconds(1.2f);
        rstAlvo = rstOriginal;
        if (somPortaAcao != null) somPortaAcao.Play();
        
        // Reseta o gatilho para caso ela precise voltar aqui no futuro do jogo
        primeiroTrancoFeito = false; 
        Debug.Log("[Porta] Jogador saiu do quarto com sucesso e a porta fechou.");
    }

    private bool linesAoAbrirValidas()
    {
        return linhasAoAbrir != null && linhasAoAbrir.Length > 0;
    }
}