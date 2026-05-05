using UnityEngine;

public class LuzPiscando : MonoBehaviour
{
    private Light luz;

    [Header("Configurações de Tempo")]
    public float minTempo = 0.05f; 
    public float maxTempo = 0.4f;  

    [Header("Visual Opcional (Pode deixar vazio)")]
    public MeshRenderer rendererLampada; 
    public Material materialLigado;      
    public Material materialDesligado;   

    void Start()
    {
        luz = GetComponent<Light>();
        
        // Se a luz não existir no objeto, desativa o script para não dar erro
        if (luz == null)
        {
            Debug.LogWarning("Script LuzPiscando em " + gameObject.name + " sem componente Light!");
            enabled = false;
            return;
        }

        StartCoroutine(Flicker());
    }

    System.Collections.IEnumerator Flicker()
    {
        while (true)
        {
            bool estadoAtual = !luz.enabled;
            luz.enabled = estadoAtual;
            
            // SÓ EXECUTA A TROCA SE TODOS OS CAMPOS FOREM PREENCHIDOS
            // Se um deles estiver vazio, ele ignora essa parte e a luz continua piscando normal
            if (rendererLampada != null && materialLigado != null && materialDesligado != null)
            {
                rendererLampada.material = estadoAtual ? materialLigado : materialDesligado;
            }

            yield return new WaitForSeconds(Random.Range(minTempo, maxTempo));
        }
    }
}