using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleBallDragVR : MonoBehaviour, IDragHandler
{
    public RectTransform mazeArea;
    public RectTransform finishArea;

    public BloodyMaryMonitor monitor;
    public AudioSource whisperAudio;
    public GameObject door1;
    public DoorController doorController;
    public SubtitleTrigger afterPuzzleTrigger;

    private RectTransform rectTransform;
    private bool completed = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (completed) return;

        rectTransform.anchoredPosition += eventData.delta;

        LimitarDentroDoMapa();
        VerificarFinal();
    }

    void LimitarDentroDoMapa()
    {
        if (mazeArea == null) return;

        Vector2 pos = rectTransform.anchoredPosition;

        Rect mazeRect = mazeArea.rect;
        Rect ballRect = rectTransform.rect;

        float minX = mazeRect.xMin + ballRect.width / 2f;
        float maxX = mazeRect.xMax - ballRect.width / 2f;

        float minY = mazeRect.yMin + ballRect.height / 2f;
        float maxY = mazeRect.yMax - ballRect.height / 2f;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        rectTransform.anchoredPosition = pos;
    }

    void VerificarFinal()
    {
        float distance = Vector2.Distance(
            rectTransform.anchoredPosition,
            finishArea.anchoredPosition
        );

        if (distance < 30f)
        {
            completed = true;

            Debug.Log("Puzzle Concluído!");

            if (monitor != null)
                monitor.ShowPuzzleCompleted();

            if (whisperAudio != null)
                whisperAudio.Play();

            if (afterPuzzleTrigger != null)
                afterPuzzleTrigger.Invoke("TocarLegenda", 2.5f);

            if (door1 != null)
                door1.SetActive(true);

            if (doorController != null)
                doorController.OpenDoor();

            transform.parent.gameObject.SetActive(false);
        }
    }
}