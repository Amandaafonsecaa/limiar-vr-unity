using UnityEngine;

public class InterruptorSimples : MonoBehaviour
{
    public GameObject luzDoQuarto;    
    public GameObject fotoFlashback; 
    public GameObject volumeSonho; 
    public AudioSource somAmbiente;  
    public ParticleSystem particulasFlashback; 

    private bool estaLigado = false;

    void Start()
    {
        // GANTE O ESTADO INICIAL: Desliga tudo no primeiro frame do jogo
        if(luzDoQuarto != null) luzDoQuarto.SetActive(false);
        if(fotoFlashback != null) fotoFlashback.SetActive(false);
        if(volumeSonho != null) volumeSonho.SetActive(false);
        
        if(particulasFlashback != null) particulasFlashback.Stop();
        if(somAmbiente != null) somAmbiente.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Se a colisão estiver certa, isso vai disparar
        AlternarEstado();
    }

    private void OnMouseDown()
    {
        AlternarEstado();
    }

    void AlternarEstado()
    {
        estaLigado = !estaLigado;

        if(luzDoQuarto != null) 
            luzDoQuarto.SetActive(estaLigado);

        if(fotoFlashback != null) 
            fotoFlashback.SetActive(estaLigado);

        if(volumeSonho != null) 
            volumeSonho.SetActive(estaLigado);

        if(particulasFlashback != null)
        {
            if (estaLigado)
                particulasFlashback.Play(); 
            else
                particulasFlashback.Stop(); 
        }

        if(somAmbiente != null)
        {
            if (estaLigado)
                somAmbiente.Play(); 
            else
                somAmbiente.Stop(); 
        }
    }
}