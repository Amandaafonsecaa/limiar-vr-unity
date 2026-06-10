using System.Collections;
using UnityEngine;

public class SceneSpawnManager : MonoBehaviour
{
    public Transform playerSpawn;
    public Transform xrOrigin;

    void Start()
    {
        StartCoroutine(MoverDepoisDeCarregar());
    }

    IEnumerator MoverDepoisDeCarregar()
    {
        yield return new WaitForSeconds(0.2f);

        xrOrigin.position = playerSpawn.position;
        xrOrigin.rotation = playerSpawn.rotation;

        yield return new WaitForSeconds(0.2f);

        xrOrigin.position = playerSpawn.position;
        xrOrigin.rotation = playerSpawn.rotation;

        Debug.Log("XR Origin forçado para PlayerSpawnCasa.");
    }
}