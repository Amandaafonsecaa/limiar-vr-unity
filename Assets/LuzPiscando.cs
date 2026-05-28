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

    [Header("Configurações do Terror (Monstro e Som)")]
    public GameObject monstroCorredor; // Arraste o Palhaço aqui
    public AudioSource somZumbido;     // Arraste o som de zumbido elétrico aqui

    void Start()
    {
        luz = GetComponent<Light>();
        
        if (luz == null)
        {
            Debug.LogWarning("Script LuzPiscando em " + gameObject.name + " sem componente Light!");
            enabled = false;
            return;
        }

        // GARANTE O ESTADO INICIAL NO COMEÇO DO JOGO:
        // O monstro começa invisível no corredor esperando a luz piscar
        if (monstroCorredor != null)
        {
            monstroCorredor.SetActive(false);
        }

        // Se o zumbido foi colocado, bota ele para tocar em Loop
        if (somZumbido != null)
        {
            somZumbido.loop = true;
            somZumbido.Play();
        }

        StartCoroutine(Flicker());
    }

    System.Collections.IEnumerator Flicker()
    {
        // PULO DO GATO: O monstro aparece assim que a lâmpada começa a falhar
        if (monstroCorredor != null)
        {
            monstroCorredor.SetActive(true);
        }

        while (true)
        {
            bool estadoAtual = !luz.enabled;
            luz.enabled = estadoAtual;
            
            // Controle do Material da Lâmpada
            if (rendererLampada != null && materialLigado != null && materialDesligado != null)
            {
                rendererLampada.material = estadoAtual ? materialLigado : materialDesligado;
            }

            // CONTROLE DO SOM: O zumbido acompanha o piscar da luz
            if (somZumbido != null)
            {
                // Se a luz apagou, o zumbido fica quase mudo (0.1). Se acendeu, volta ao volume normal (1.0)
                somZumbido.volume = estadoAtual ? 1.0f : 0.1f;
            }

            yield return new WaitForSeconds(Random.Range(minTempo, maxTempo));
        }
    }
}