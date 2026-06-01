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
        Debug.Log("Memória das esculturas concluída.");
    }

    public void TryTriggerNotebookEvent()
    {
        if (notebookEventTriggered)
            return;

        if (!CanOpenMotherRoomDoor())
        {
            Debug.Log("Caderno encontrado, mas ainda faltam memórias antes da porta abrir.");
            return;
        }

        notebookEventTriggered = true;
        StartCoroutine(NotebookEventSequence());
    }

    private bool CanOpenMotherRoomDoor()
    {
        return brushMemoryDone && tvMemoryDone && sculpturesMemoryDone;
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