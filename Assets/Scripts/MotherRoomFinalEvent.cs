using System.Collections;
using UnityEngine;

public class MotherRoomFinalEvent : MonoBehaviour
{
    [Header("Room Lights")]
    [SerializeField] private Light[] roomLights;
    [SerializeField] private float flickerDuration = 3f;
    [SerializeField] private float minLightIntensity = 0.1f;
    [SerializeField] private float maxLightIntensity = 2f;

    [Header("Presence")]
    [SerializeField] private GameObject presenceInMotherRoom;
    [SerializeField] private float presenceDuration = 2f;
    [SerializeField] private GameObject finalPresenceInLivingRoom;

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Player Teleport")]
    [SerializeField] private Transform playerRig;
    [SerializeField] private Transform livingRoomReturnPoint;

    [Header("Anchor")]
    [SerializeField] private GameObject houseAnchorObject;

    private bool hasPlayed;

    public void PlayEvent()
    {
        if (hasPlayed)
            return;

        hasPlayed = true;
        StartCoroutine(PlayFinalSequence());
    }

    private IEnumerator PlayFinalSequence()
    {
        if (presenceInMotherRoom != null)
            presenceInMotherRoom.SetActive(false);

        if (finalPresenceInLivingRoom != null)
            finalPresenceInLivingRoom.SetActive(false);

        if (houseAnchorObject != null)
            houseAnchorObject.SetActive(false);

        yield return FlickerLights();

        if (presenceInMotherRoom != null)
            presenceInMotherRoom.SetActive(true);

        yield return new WaitForSeconds(presenceDuration);

        yield return Fade(1f);

        if (presenceInMotherRoom != null)
            presenceInMotherRoom.SetActive(false);

        TeleportPlayerToLivingRoom();

        if (finalPresenceInLivingRoom != null)
            finalPresenceInLivingRoom.SetActive(true);

        if (houseAnchorObject != null)
            houseAnchorObject.SetActive(true);

        yield return Fade(0f);

        Debug.Log("Evento final do quarto da mãe concluído.");
    }

    private void TeleportPlayerToLivingRoom()
    {
        if (playerRig == null || livingRoomReturnPoint == null)
        {
            Debug.LogWarning("Player Rig ou Living Room Return Point não foram configurados.");
            return;
        }

        CharacterController characterController = playerRig.GetComponent<CharacterController>();

        if (characterController != null)
            characterController.enabled = false;

        Vector3 targetPosition = livingRoomReturnPoint.position;

        Quaternion targetRotation = Quaternion.Euler(
            0f,
            livingRoomReturnPoint.eulerAngles.y,
            0f
        );

        playerRig.SetPositionAndRotation(targetPosition, targetRotation);

        Physics.SyncTransforms();

        if (characterController != null)
            characterController.enabled = true;

        Debug.Log("Jogador teleportado para a sala final.");
    }

    private IEnumerator FlickerLights()
    {
        if (roomLights == null || roomLights.Length == 0)
        {
            yield return new WaitForSeconds(flickerDuration);
            yield break;
        }

        float[] originalIntensities = new float[roomLights.Length];

        for (int i = 0; i < roomLights.Length; i++)
        {
            if (roomLights[i] != null)
                originalIntensities[i] = roomLights[i].intensity;
        }

        float elapsed = 0f;

        while (elapsed < flickerDuration)
        {
            elapsed += Time.deltaTime;

            for (int i = 0; i < roomLights.Length; i++)
            {
                if (roomLights[i] != null)
                    roomLights[i].intensity = Random.Range(minLightIntensity, maxLightIntensity);
            }

            yield return new WaitForSeconds(0.08f);
        }

        for (int i = 0; i < roomLights.Length; i++)
        {
            if (roomLights[i] != null)
                roomLights[i].intensity = originalIntensities[i];
        }
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeCanvas == null)
            yield break;

        float startAlpha = fadeCanvas.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        fadeCanvas.alpha = targetAlpha;
    }
}