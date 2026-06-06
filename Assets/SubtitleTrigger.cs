using UnityEngine;

public class SubtitleTrigger : MonoBehaviour
{
    // A estrutura foi mantida pública aqui para servir de modelo global para os outros scripts
    [System.Serializable]
    public class LinhaLegenda
    {
        [TextArea(2, 4)]
        public string texto;
        public float duracao = 2.5f;
    }

    [Header("Configuração da Legenda")]
    [SerializeField] private LegendaVR sistemaLegenda; 
    [SerializeField] private bool dispararApenasUmaVez = true;

    [Header("Linhas de Legenda Deste Gatilho")]
    public LinhaLegenda[] linhas;

    private bool jaDisparou = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<Camera>() != null || other.name.Contains("Origin"))
        {
            if (jaDisparou && dispararApenasUmaVez) return;

            if (sistemaLegenda != null && linhas != null && linhas.Length > 0)
            {
                jaDisparou = true;
                
                // Passa as falas exclusivas deste gatilho de proximidade para o tocador
                sistemaLegenda.MostrarSequencia(linhas); 
                
                Debug.Log($"[Gatilho] {gameObject.name} enviou {linhas.Length} linhas de legenda.");
            }
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
        }
    }
}