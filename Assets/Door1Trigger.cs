using UnityEngine;
using UnityEngine.SceneManagement;

public class Door1Trigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Fase1");
        }
    }
}