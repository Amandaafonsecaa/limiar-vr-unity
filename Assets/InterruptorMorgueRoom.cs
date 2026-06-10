using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InterruptorMorgueRoom : MonoBehaviour
{
    [Header("Iluminação Geral")]
    public GameObject luzDoQuarto;

    [Header("Configuração do Monstro e Susto")]
    public GameObject monstroPerseguidor;
    public AudioSource somDoMonstro;

    [Header("Cena da Cutscene Final")]
    [SerializeField] private bool carregarCutsceneDepois = true;
    [SerializeField] private string nomeCenaCutscene = "Cutscene final";
    [SerializeField] private float tempoAntesDaCutscene = 6f;

    private bool jaAtivou = false;

    void Start()
    {
        if (monstroPerseguidor != null)
            monstroPerseguidor.SetActive(false);

        if (luzDoQuarto != null)
            luzDoQuarto.SetActive(false);

        if (somDoMonstro != null)
            somDoMonstro.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<Camera>() != null || other.name.Contains("Origin"))
        {
            AtivarSustoMorgue();
        }
    }

    private void OnMouseDown()
    {
        AtivarSustoMorgue();
    }

    void AtivarSustoMorgue()
    {
        if (jaAtivou)
            return;

        jaAtivou = true;

        Debug.Log("[Morgue] Interruptor acionado! Invocando o monstro.");

        if (luzDoQuarto != null)
            luzDoQuarto.SetActive(true);

        if (monstroPerseguidor != null)
            monstroPerseguidor.SetActive(true);

        if (somDoMonstro != null)
            somDoMonstro.Play();

        if (carregarCutsceneDepois)
            StartCoroutine(CarregarCenaCutsceneDepoisDoSusto());
    }

    private IEnumerator CarregarCenaCutsceneDepoisDoSusto()
    {
        Debug.Log("[Morgue] Aguardando fim da sequência antes da cutscene.");

        yield return new WaitForSeconds(tempoAntesDaCutscene);

        Debug.Log("[Morgue] Carregando cena da cutscene final.");

        SceneManager.LoadScene(nomeCenaCutscene);
    }
}