using UnityEngine;

public class InterruptorSimples : MonoBehaviour
{
    // Removi os [Header] para evitar conflitos de atributos no Inspector
    public GameObject luzDoQuarto;    
    public GameObject fotoFlashback; 
    public GameObject volumeSonho; 
    public AudioSource somAmbiente;  
    public ParticleSystem particulasFlashback; 

    private bool estaLigado = false;

    private void OnTriggerEnter(Collider other)
    {
        // VR Trigger
        AlternarEstado();
    }

    private void OnMouseDown()
    {
        // Mouse Test
        AlternarEstado();
    }

    void AlternarEstado()
    {
        estaLigado = !estaLigado;

        // Verifica cada objeto antes de mudar para evitar erros de 'Null Reference'
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