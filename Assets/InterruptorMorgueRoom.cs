using UnityEngine;

public class InterruptorMorgueRoom : MonoBehaviour
{
    public GameObject luzDoQuarto;    
    public GameObject fotoFlashback; 
    public GameObject volumeSonho; 
    public AudioSource somAmbiente;  

    [Header("Configuração do Monstro")]
    public GameObject monstroPerseguidor; // Arraste o monstro do quarto aqui

    private bool estaLigado = false;

    void Start()
    {
        // SEGURANÇA ABSOLUTA: Força o monstro a sumir e não existir no mapa no início do jogo
        if (monstroPerseguidor != null)
            monstroPerseguidor.SetActive(false);

        // Garante que os outros elementos visuais e sonoros também comecem desligados
        if (luzDoQuarto != null) luzDoQuarto.SetActive(false);
        if (fotoFlashback != null) fotoFlashback.SetActive(false);
        if (volumeSonho != null) volumeSonho.SetActive(false);
        if (somAmbiente != null) somAmbiente.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        AlternarEstado();
    }

    private void OnMouseDown()
    {
        AlternarEstado();
    }

    void AlternarEstado()
    {
        estaLigado = !estaLigado;

        // Ativa ou desativa a luz do quarto
        if(luzDoQuarto != null) 
            luzDoQuarto.SetActive(estaLigado);

        // O monstro é invocado apenas quando a luz liga. Depois disso, ele não some mais.
        if (estaLigado && monstroPerseguidor != null)
        {
            monstroPerseguidor.SetActive(true);
        }

        if(fotoFlashback != null) 
            fotoFlashback.SetActive(estaLigado);

        if(volumeSonho != null) 
            volumeSonho.SetActive(estaLigado);

        // Toca o som do flashback (som ambiente configurado)
        if(somAmbiente != null)
        {
            if (estaLigado)
                somAmbiente.Play(); 
            else
                somAmbiente.Stop(); 
        }
    }
}