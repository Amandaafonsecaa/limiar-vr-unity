using System.Collections;
using TMPro;
using UnityEngine;

public class BookshelfMemoryDoneTrigger : MonoBehaviour
{
    [System.Serializable]
    public class CaptionLine
    {
        [TextArea(2, 4)]
        public string text;

        public float duration = 2.5f;
    }

    [Header("Progression")]
    [SerializeField] private HouseProgressionManager progressionManager;

    [Header("Caption UI")]
    [SerializeField] private GameObject captionCanvas;
    [SerializeField] private CanvasGroup captionCanvasGroup;
    [SerializeField] private TMP_Text captionText;
    [SerializeField] private CaptionLine[] captionLines;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float delayBetweenLines = 0.2f;

    [Header("Detection")]
    [SerializeField] private string playerTag = "Player";

    private bool hasTriggered;

    private void Awake()
    {
        if (captionCanvas != null)
            captionCanvas.SetActive(false);

        if (captionCanvasGroup != null)
            captionCanvasGroup.alpha = 0f;

        if (captionText != null)
            captionText.text = "";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        bool isPlayer = other.CompareTag(playerTag) || other.transform.root.CompareTag(playerTag);

        if (!isPlayer)
            return;

        hasTriggered = true;
        StartCoroutine(PlayBookshelfMemory());
    }

    private IEnumerator PlayBookshelfMemory()
    {
        if (captionCanvas != null)
            captionCanvas.SetActive(true);

        foreach (CaptionLine line in captionLines)
        {
            if (captionText != null)
                captionText.text = line.text;

            yield return FadeCaption(1f);
            yield return new WaitForSeconds(line.duration);
            yield return FadeCaption(0f);

            if (captionText != null)
                captionText.text = "";

            yield return new WaitForSeconds(delayBetweenLines);
        }

        if (captionCanvas != null)
            captionCanvas.SetActive(false);

        if (progressionManager != null)
            progressionManager.MarkSculpturesMemoryDone();

        Debug.Log("Memória da estante/livros concluída.");
    }

    private IEnumerator FadeCaption(float targetAlpha)
    {
        if (captionCanvasGroup == null)
            yield break;

        float startAlpha = captionCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            captionCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        captionCanvasGroup.alpha = targetAlpha;
    }
}