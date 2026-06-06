using System.Collections;
using UnityEngine;

public class HouseProgressionManager : MonoBehaviour
{
    [Header("Required Events")]
    [SerializeField] private bool brushMemoryDone;
    [SerializeField] private bool tvMemoryDone;
    [SerializeField] private bool sculpturesMemoryDone;

    [Header("Mother Room Door")]
    [SerializeField] private AbirPorta motherRoomDoor;

    [Header("Optional Audio")]
    [SerializeField] private AudioSource motherScreamAudio;

    [Header("Timing")]
    [SerializeField] private float delayBeforeDoorOpens = 1.2f;

    private bool notebookEventTriggered;

    public bool HasCompletedRequiredMemories()
    {
        return brushMemoryDone && tvMemoryDone && sculpturesMemoryDone;
    }

    public void MarkBrushMemoryDone()
    {
        brushMemoryDone = true;
        Debug.Log("Memória da escova concluída.");
    }

    public void MarkTvMemoryDone()
    {
        tvMemoryDone = true;
        Debug.Log("Memória da TV concluída.");
    }

    public void MarkSculpturesMemoryDone()
    {
        sculpturesMemoryDone = true;
        Debug.Log("Memória da estante/livros concluída.");
    }

    public bool TryTriggerNotebookEvent()
    {
        if (notebookEventTriggered)
            return true;

        if (!HasCompletedRequiredMemories())
        {
            Debug.Log("Caderno encontrado, mas ainda faltam memórias antes da porta abrir.");
            return false;
        }

        notebookEventTriggered = true;
        StartCoroutine(NotebookEventSequence());
        return true;
    }

    private IEnumerator NotebookEventSequence()
    {
        Debug.Log("Caderno encontrado depois das memórias. Evento do quarto da mãe iniciado.");

        if (motherScreamAudio != null)
            motherScreamAudio.Play();

        yield return new WaitForSeconds(delayBeforeDoorOpens);

        if (motherRoomDoor != null)
            motherRoomDoor.UnlockAndOpenDoor();
    }
}