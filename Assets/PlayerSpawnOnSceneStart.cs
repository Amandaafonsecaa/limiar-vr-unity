using UnityEngine;

public class PlayerSpawnOnSceneStart : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;

    private void Start()
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("Spawn Point não conectado.");
            return;
        }

        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        Debug.Log("Player reposicionado no spawn da casa.");
    }
}