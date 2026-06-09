using UnityEngine;

public class HubProgressManager : MonoBehaviour
{
    public GameObject door2;
    public SubtitleTrigger door2SubtitleTrigger;

    void Start()
{
    if (GameProgress.casaConcluida)
    {
        door2.SetActive(true);

        if (door2SubtitleTrigger != null)
            door2SubtitleTrigger.TocarLegenda();
    }
}
}