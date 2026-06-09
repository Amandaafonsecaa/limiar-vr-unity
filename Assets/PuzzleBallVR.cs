using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleBallVR : MonoBehaviour
{
    public float speed = 300f;
    public RectTransform finishArea;

    public AudioSource whisperAudio;
    public BloodyMaryMonitor monitor;
    public GameObject door1;
    public DoorController doorController;
    public SubtitleTrigger afterPuzzleTrigger;

    private RectTransform rectTransform;
    private bool completed = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        monitor = FindFirstObjectByType<BloodyMaryMonitor>();

        GameObject audioObj = GameObject.Find("WhisperAudio");

        if (audioObj != null)
            whisperAudio = audioObj.GetComponent<AudioSource>();
    }

    void Update()
    {
        Vector2 movement = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            movement += Vector2.up;

        if (Keyboard.current.sKey.isPressed)
            movement += Vector2.down;

        if (Keyboard.current.aKey.isPressed)
            movement += Vector2.left;

        if (Keyboard.current.dKey.isPressed)
            movement += Vector2.right;

        rectTransform.anchoredPosition += movement * speed * Time.deltaTime;

        if (completed) return;

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