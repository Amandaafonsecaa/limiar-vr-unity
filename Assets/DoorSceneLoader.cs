using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSceneLoader : MonoBehaviour
{
    [Header("Configuração de Transição")]
    public string sceneName = "Casa";
    
    [Tooltip("O nome exato do GameObject de Spawn para onde o jogador deve ir ao carregar a cena")]
    public string nomeDoSpawnAlvo = "SpawnInicial";

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se quem entrou na porta foi o jogador
        if (other.CompareTag("Player"))
        {
            // Guarda na memória qual é o spawn correto ANTES de mudar de cena
            GameManagerData.pontoDeSpawnAlvo = nomeDoSpawnAlvo;
            
            // Carrega a nova cena
            SceneManager.LoadScene(sceneName);
        }
    }
}