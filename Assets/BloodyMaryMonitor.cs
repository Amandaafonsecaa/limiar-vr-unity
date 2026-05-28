using UnityEngine;
using TMPro;

public class BloodyMaryMonitor : MonoBehaviour
{
    public GameObject screenUI;
    public TextMeshProUGUI terminalText;
    public AudioSource whisper;

    private bool activated = false;
    private float lookTimer = 0f;

    void Update()
    {
        if (!activated) return;

        Transform cam = Camera.main.transform;

        Vector3 dir =
            (transform.position - cam.position).normalized;

        float dot =
            Vector3.Dot(cam.forward, dir);

        bool looking = dot > 0.95f;

        if (looking)
        {
            lookTimer += Time.deltaTime;

            if (lookTimer >= 3f)
            {
                whisper.Play();

                terminalText.text +=
                    "\n\n\"Diga o nome dela\"";

                enabled = false;
            }
        }
        else
        {
            lookTimer = 0f;
        }
    }

    public void ActivateMonitor()
    {
        if (activated) return;

        activated = true;

        screenUI.SetActive(true);

        terminalText.text =
            "A tela oscila com linhas de interferência.\n\n" +
            "> EXECUTAR: bloody_mary.exe\n" +
            "> STATUS: AGUARDANDO REFLEXO...";
    }
}