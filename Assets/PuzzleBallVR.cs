using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleBallVR : MonoBehaviour
{
    public float speed = 300f;
    public RectTransform finishArea;

    public AudioSource whisperAudio;
    public BloodyMaryMonitor monitor;
    public GameObject door1;
    private RectTransform rectTransform;
    private bool completed = false;
    public DoorController doorController;
void Start()
{
    rectTransform = GetComponent<RectTransform>();

    monitor = FindFirstObjectByType<BloodyMaryMonitor>();

    GameObject audioObj = GameObject.Find("WhisperAudio");

    Debug.Log("Audio encontrado: " + audioObj);

    if (audioObj != null)
    {
        whisperAudio = audioObj.GetComponent<AudioSource>();

        Debug.Log("AudioSource encontrada: " + whisperAudio);
    }
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

        if (!completed)
        {
            float distance = Vector2.Distance(
                rectTransform.anchoredPosition,
                finishArea.anchoredPosition
            );

            if (distance < 30f)
            {
                completed = true;

                Debug.Log("Puzzle Concluído!");

                // Fecha o puzzle
                transform.parent.gameObject.SetActive(false);

                // Volta para o terminal
                monitor.ShowPuzzleCompleted();

                // Toca o sussurro
                if (whisperAudio != null)
                    whisperAudio.Play();

                // Faz a Porta 1 aparecer
                if (door1 != null)
                    door1.SetActive(true);

                // Abre a porta
                if (doorController != null)
                    doorController.OpenDoor();
            }
        }
    }
}