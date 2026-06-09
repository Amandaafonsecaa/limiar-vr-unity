using UnityEngine;

public class SleepParalysisTrigger : MonoBehaviour
{
    [SerializeField] private SleepParalysisEffect effect;
    [SerializeField] private string playerTag = "Player";

    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        if (!other.CompareTag(playerTag))
            return;

        hasTriggered = true;

        if (effect != null)
            effect.StartEffect();

        Debug.Log("Efeito de paralisia do sono iniciado.");
    }
}