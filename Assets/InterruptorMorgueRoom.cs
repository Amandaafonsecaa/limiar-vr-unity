using UnityEngine;

public class InterruptorMorgueRoom : MonoBehaviour
{
    [Header("Iluminação Geral")]
    public GameObject luzDoQuarto;    

    [Header("Configuração do Monstro e Susto")]
    public GameObject monstroPerseguidor; 
    public AudioSource somDoMonstro;  

    private bool jaAtivou = false;

    void Start()
    {
        // Garante que o monstro comece totalmente sumido do mapa
        if (monstroPerseguidor != null)
            monstroPerseguidor.SetActive(false);

        // Garante que a luz comece apagada
        if (luzDoQuarto != null) 
            luzDoQuarto.SetActive(false);

        // Garante que o som do monstro não toque sozinho no início
        if (somDoMonstro != null) 
            somDoMonstro.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se quem entrou na área foi o jogador VR
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
        // Se já ativou uma vez, ignora para a luz continuar ligada direto
        if (jaAtivou) return;
        jaAtivou = true;

        Debug.Log("[Morgue] Interruptor acionado! Invocando o monstro.");

        // 1. Liga a luz geral do quarto e ela NÃO desliga mais
        if (luzDoQuarto != null) 
            luzDoQuarto.SetActive(true);

        // 2. Faz o monstro surgir instantaneamente na cena
        if (monstroPerseguidor != null)
        {
            monstroPerseguidor.SetActive(true);
        }

        // 3. Toca o áudio de terror/grito do monstro
        if (somDoMonstro != null)
        {
            somDoMonstro.Play(); 
        }
    }
}