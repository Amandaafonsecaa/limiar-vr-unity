using UnityEngine;
using System.Collections;

public class SubtitleTrigger : MonoBehaviour
{
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
    [SerializeField] private bool tocarAoIniciar = false;

    [Header("Áudio após a legenda")]
    [SerializeField] private AudioSource musicaDepois;

    [Header("Linhas de Legenda Deste Gatilho")]
    public LinhaLegenda[] linhas;

    private bool jaDisparou = false;

    void Start()
    {
        if (tocarAoIniciar)
        {
            TocarLegenda();
        }
    }

    public void TocarLegenda()
    {
        if (jaDisparou && dispararApenasUmaVez) return;

        if (sistemaLegenda != null && linhas != null && linhas.Length > 0)
        {
            jaDisparou = true;
            sistemaLegenda.MostrarSequencia(linhas);

            if (musicaDepois != null)
                StartCoroutine(TocarMusicaDepois());
        }
        else
        {
            Debug.LogError("SistemaLegenda não atribuído ou linhas vazias em " + gameObject.name);
        }
    }

    private IEnumerator TocarMusicaDepois()
    {
        float tempoTotal = 0f;

        foreach (LinhaLegenda linha in linhas)
        {
            tempoTotal += linha.duracao + 0.2f;
        }

        yield return new WaitForSeconds(tempoTotal);

        musicaDepois.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<Camera>() != null || other.name.Contains("Origin"))
        {
            TocarLegenda();
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