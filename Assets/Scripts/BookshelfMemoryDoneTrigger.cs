using UnityEngine;

public class BookshelfMemoryDoneTrigger : MonoBehaviour
{
    [Header("Progression")]
    [SerializeField] private HouseProgressionManager progressionManager;

    [Header("Optional Visual Feedback")]
    [SerializeField] private Light highlightLight;
    [SerializeField] private GameObject objectToEnable;

    [Header("Detection")]
    [SerializeField] private string playerTag = "Player";

    private bool hasTriggered;

    private void Awake()
    {
        if (highlightLight != null)
            highlightLight.enabled = false;

        if (objectToEnable != null)
            objectToEnable.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        if (!other.CompareTag(playerTag))
            return;

        hasTriggered = true;

        if (highlightLight != null)
            highlightLight.enabled = true;

        if (objectToEnable != null)
            objectToEnable.SetActive(true);

        if (progressionManager != null)
            progressionManager.MarkSculpturesMemoryDone();

        Debug.Log("Memória da estante/livros concluída.");
    }
}