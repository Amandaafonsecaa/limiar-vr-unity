using UnityEngine;

public class ItemPickupTrigger : MonoBehaviour
{
    [Header("Configuração da Legenda")]
    [SerializeField] private LegendaVR sistemaLegenda;
    [SerializeField] private bool dispararApenasUmaVez = true;

    [Header("Linhas de Legenda do Item")]
    public SubtitleTrigger.LinhaLegenda[] linhasDaMemoria;

    [Header("Áudio do Item")]
    [Tooltip("O áudio da fala ou memória que toca ao pegar o urso.")]
    [SerializeField] private AudioClip audioDoItem;

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
        // Detecta se quem encostou foi o jogador ou a câmera VR
        if (other.CompareTag("Player") || other.GetComponent<Camera>() != null || other.name.Contains("Origin"))
        {
            if (jaDisparou && dispararApenasUmaVez) return;

            if (sistemaLegenda != null && linhasDaMemoria != null && linhasDaMemoria.Length > 0)
            {
                jaDisparou = true;

                // 1. Toca o Áudio se ele tiver sido colocado no Inspector
                if (audioDoItem != null && audioSource != null)
                {
                    audioSource.clip = audioDoItem;
                    audioSource.volume = 1.0f;
                    audioSource.Play();
                    Debug.Log($"[Item] Tocando áudio do item: {gameObject.name}");
                }

                // 2. Manda as legendas para a tela no mesmo instante
                sistemaLegenda.MostrarSequencia(linhasDaMemoria);
                Debug.Log($"[Item] Mostrando legendas do item: {gameObject.name}");
            }
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            // Desenha em AZUL na Scene para você saber que é um gatilho de item colecionável
            Gizmos.color = new Color(0f, 0.5f, 1f, 0.4f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
        }
    }
}