using UnityEngine;

public class InterruptorSimples : MonoBehaviour
{
    [Header("Elementos do Quarto")]
    public GameObject luzDoQuarto;    
    public GameObject volumeSonho; 
    public AudioSource somAmbiente;  

    [Header("Elementos do Flashback (Visual e Som Local)")]
    public GameObject fotoFlashback; 
    public ParticleSystem particulasFlashback; 
    
    [Tooltip("Arraste aqui o AudioSource que está anexado JUNTO ao objeto da Foto do Flashback.")]
    [SerializeField] private AudioSource audioSourceDaFoto;

    [Header("Sistema de Legendas")]
    [SerializeField] private LegendaVR sistemaLegenda;
    
    [Tooltip("A sequência de legendas que acompanha o flashback.")]
    public SubtitleTrigger.LinhaLegenda[] linhasFlashback;

    private bool estaLigado = false;
    private bool jaDisparouFlashback = false; // Garante que a história só toca na primeira vez que liga a luz

    void Start()
    {
        // GARANTE O ESTADO INICIAL: Desliga tudo no primeiro frame do jogo
        if(luzDoQuarto != null) luzDoQuarto.SetActive(false);
        if(fotoFlashback != null) fotoFlashback.SetActive(false);
        if(volumeSonho != null) volumeSonho.SetActive(false);
        
        if(particulasFlashback != null) particulasFlashback.Stop();
        if(somAmbiente != null) somAmbiente.Stop();
        
        // Garante que o som vindo da foto comece desligado
        if(audioSourceDaFoto != null) audioSourceDaFoto.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Garante que só o jogador ou controles VR ativem o interruptor por proximidade/toque
        if (other.CompareTag("Player") || other.GetComponent<Camera>() != null || other.name.Contains("Origin"))
        {
            AlternarEstado();
        }
    }

    private void OnMouseDown()
    {
        AlternarEstado();
    }

    void AlternarEstado()
    {
        estaLigado = !estaLigado;

        // Ativa/Desativa elementos comuns
        if(luzDoQuarto != null) luzDoQuarto.SetActive(estaLigado);
        if(volumeSonho != null) volumeSonho.SetActive(estaLigado);

        // Ativa/Desativa o visual do Flashback
        if(fotoFlashback != null) fotoFlashback.SetActive(estaLigado);

        if(particulasFlashback != null)
        {
            if (estaLigado) particulasFlashback.Play(); else particulasFlashback.Stop(); 
        }

        if(somAmbiente != null)
        {
            if (estaLigado) somAmbiente.Play(); else somAmbiente.Stop(); 
        }

        // ==========================================================
        // DISPARO DO SOM DA FOTO + LEGENDA (APENAS QUANDO LIGA A LUZ)
        // ==========================================================
        if (estaLigado)
        {
            // 1. Toca o som vindo DIRETAMENTE do objeto da foto (se houver um configurado)
            if (audioSourceDaFoto != null)
            {
                audioSourceDaFoto.Play();
                Debug.Log($"[Interruptor] Ativando áudio 3D vindo de: {audioSourceDaFoto.gameObject.name}");
            }

            // 2. Dispara as legendas (Apenas na primeira vez que o flashback acontece para não virar bagunça)
            if (!jaDisparouFlashback && sistemaLegenda != null && linhasFlashback != null && linhasFlashback.Length > 0)
            {
                jaDisparouFlashback = true;
                sistemaLegenda.MostrarSequencia(linhasFlashback);
                Debug.Log("[Interruptor] Legendas do Flashback iniciadas.");
            }
        }
        else
        {
            // Se o jogador desligar o interruptor no meio do evento, corta o áudio da foto imediatamente
            if (audioSourceDaFoto != null)
            {
                audioSourceDaFoto.Stop();
            }
        }
    }
}