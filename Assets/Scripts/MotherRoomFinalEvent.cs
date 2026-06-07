using System.Collections;
using UnityEngine;

public class MotherRoomFinalEvent : MonoBehaviour
{
    [Header("Room Light")]
    [SerializeField] private Light roomLight;
    [SerializeField] private float flickerDuration = 3f;
    [SerializeField] private float minIntensity = 0.05f;
    [SerializeField] private float maxIntensity = 2f;

    [Header("Anchor")]
    [SerializeField] private GameObject houseAnchorObject;

    [Header("Timing")]
    [SerializeField] private float delayBeforeAnchor = 1.5f;

    private bool hasPlayed;

    public void PlayEvent()
    {
        if (hasPlayed)
            return;

        hasPlayed = true;
        StartCoroutine(PlayFinalEvent());
    }

    private IEnumerator PlayFinalEvent()
    {
        Debug.Log("Evento final simples do quarto da mãe iniciado.");

        if (houseAnchorObject != null)
            houseAnchorObject.SetActive(false);

        yield return FlickerLight();

        yield return new WaitForSeconds(delayBeforeAnchor);

        if (houseAnchorObject != null)
            houseAnchorObject.SetActive(true);

        Debug.Log("Âncora da casa ativada.");
    }

    private IEnumerator FlickerLight()
    {
        if (roomLight == null)
        {
            yield return new WaitForSeconds(flickerDuration);
            yield break;
        }

        float originalIntensity = roomLight.intensity;
        float elapsed = 0f;

        while (elapsed < flickerDuration)
        {
            elapsed += Time.deltaTime;
            roomLight.intensity = Random.Range(minIntensity, maxIntensity);
            yield return new WaitForSeconds(0.08f);
        }

        roomLight.intensity = originalIntensity;
    }
}