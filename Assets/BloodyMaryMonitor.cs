using UnityEngine;
using TMPro;
using System.Collections;

public class BloodyMaryMonitor : MonoBehaviour
{
    public GameObject screenUI;
    public TextMeshProUGUI terminalText;

    private CanvasGroup puzzlePanel;
    private bool activated = false;

    public AudioSource whisperAudio;
    public GameObject blackLiquid;

    void Start()
    {
        GameObject obj = GameObject.Find("PuzzlePanel");

        if (obj != null)
        {
            puzzlePanel = obj.GetComponent<CanvasGroup>();

            puzzlePanel.alpha = 0;
            puzzlePanel.interactable = false;
            puzzlePanel.blocksRaycasts = false;
        }
    }

    public void ActivateMonitor()
    {
        if (activated) return;

        activated = true;

        screenUI.SetActive(true);

        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        terminalText.text = "> EXECUTAR: bloody_mary.exe";

        yield return new WaitForSeconds(2f);

        terminalText.text += "\n> STATUS: AGUARDANDO REFLEXO...";

        yield return new WaitForSeconds(3f);

        puzzlePanel.alpha = 1;
        puzzlePanel.interactable = true;
        puzzlePanel.blocksRaycasts = true;
    }
    public void ShowPuzzleCompleted()
{
    screenUI.SetActive(true);

    terminalText.text =
        "> EXECUTAR: bloody_mary.exe\n" +
        "> STATUS: AGUARDANDO REFLEXO...\n\n" +
        "> PUZZLE CONCLUÍDO";
}
    public void StartFinalSequence()
{
    StartCoroutine(FinalSequence());
}

IEnumerator FinalSequence()
{
    ShowPuzzleCompleted();

    yield return new WaitForSeconds(1f);

    whisperAudio.Play();

    yield return new WaitForSeconds(2f);

    blackLiquid.SetActive(true);
}
}