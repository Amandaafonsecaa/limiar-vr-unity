using UnityEngine;

public class PlayerFootstepAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerRig;
    [SerializeField] private AudioSource audioSource;

    [Header("Footstep Audio")]
    [SerializeField] private AudioClip footstepClip;

    [Header("Movement Detection")]
    [SerializeField] private float minMoveSpeed = 0.03f;
    [SerializeField] private float stepInterval = 0.45f;

    [Header("Audio Settings")]
    [SerializeField] private float volume = 0.7f;
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.05f;

    private Vector3 lastPosition;
    private float stepTimer;

    private void Awake()
    {
        if (playerRig == null)
            playerRig = transform;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.volume = volume;
        }

        lastPosition = playerRig.position;
    }

    private void Update()
    {
        if (playerRig == null || audioSource == null || footstepClip == null)
            return;

        Vector3 currentPosition = playerRig.position;

        Vector3 movement = currentPosition - lastPosition;
        movement.y = 0f;

        float speed = movement.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        bool isMoving = speed >= minMoveSpeed;

        if (isMoving)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }

        lastPosition = currentPosition;
    }

    private void PlayFootstep()
    {
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(footstepClip, volume);
    }
}