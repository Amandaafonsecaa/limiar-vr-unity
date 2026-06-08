using System.Collections;
using UnityEngine;

public class MotherRoomFinalEvent : MonoBehaviour
{
    [Header("Room Light")]
    [SerializeField] private Light roomLight;
    [SerializeField] private float flickerDuration = 3f;
    [SerializeField] private float minIntensity = 0.05f;
    [SerializeField] private float maxIntensity = 2f;
    [SerializeField] private float finalRoomIntensity = 0.03f;

    [Header("Anchor Reveal")]
    [SerializeField] private GameObject houseAnchorObject;
    [SerializeField] private Light anchorLight;

    private bool hasPlayed;

    private void Awake()
    {
        if (houseAnchorObject != null)
            houseAnchorObject.SetActive(false);

        if (anchorLight != null)
            anchorLight.enabled = false;
    }

    public void PlayEvent()
    {
        if (hasPlayed)
            return;

        hasPlayed = true;
        StartCoroutine(PlayFinalEvent());
    }

    private IEnumerator PlayFinalEvent()
    {
        Debug.Log("Evento final do quarto da mãe iniciado.");

        yield return FlickerLight();

        if (roomLight != null)
            roomLight.intensity = finalRoomIntensity;

        RevealAnchor();

        Debug.Log("Quarto escuro. Âncora e luz da âncora ativadas.");
    }

    private IEnumerator FlickerLight()
    {
        if (roomLight == null)
        {
            yield return new WaitForSeconds(flickerDuration);
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < flickerDuration)
        {
            elapsed += Time.deltaTime;

            roomLight.intensity = Random.Range(minIntensity, maxIntensity);

            yield return new WaitForSeconds(0.08f);
        }
    }

    private void RevealAnchor()
    {
        if (houseAnchorObject != null)
            houseAnchorObject.SetActive(true);

        if (anchorLight != null)
            anchorLight.enabled = true;
    }
}