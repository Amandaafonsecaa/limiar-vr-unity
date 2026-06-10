// PlayerSpawner.cs
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Configurações do Jogador")]
    [Tooltip("Arraste o objeto principal do seu Player VR aqui (ex: XR Origin)")]
    public GameObject vrPlayerRig; 

    [Header("Pontos de Spawn")]
    [Tooltip("Arraste os Empty GameObjects de spawn que você criou na cena")]
    public Transform[] spawnPoints;

    void Start()
    {
        PosicionarJogador();
    }

    void PosicionarJogador()
    {
        // Procura na lista qual é o ponto de spawn correto com base na memória do jogo
        foreach (Transform ponto in spawnPoints)
        {
            if (ponto.name == GameManagerData.pontoDeSpawnAlvo)
            {
                TeleportarPara(ponto);
                return;
            }
        }
        
        Debug.LogWarning("Ponto de spawn não encontrado. O jogador ficará na posição atual.");
    }

    void TeleportarPara(Transform alvo)
    {
        // IMPORTANTE PARA VR: Se você estiver usando um CharacterController 
        // no seu rig, você deve desativá-lo antes de mover, senão ele bloqueia o teleporte.
        CharacterController cc = vrPlayerRig.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Move a posição e a rotação do Rig para o ponto de spawn
        vrPlayerRig.transform.position = alvo.position;
        vrPlayerRig.transform.rotation = alvo.rotation;

        // Reativa o Character Controller
        if (cc != null) cc.enabled = true;
    }
}