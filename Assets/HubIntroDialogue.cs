using UnityEngine;
using TMPro;
using System.Collections;

public class HubIntroDialogue : MonoBehaviour
{
    [System.Serializable]
    public class Linha
    {
        [TextArea(2, 4)]
        public string texto;
        public float duracao = 3f;
    }

    public TextMeshProUGUI subtitleText;
    public Linha[] falasIniciais;
    public AudioSource musicaHub;

    void Start()
    {
        subtitleText.text = "";
        StartCoroutine(RodarIntro());
    }

    IEnumerator RodarIntro()
    {
        yield return new WaitForSeconds(1.5f);

        foreach (Linha linha in falasIniciais)
        {
            subtitleText.text = linha.texto;
            yield return new WaitForSeconds(linha.duracao);
            subtitleText.text = "";
            yield return new WaitForSeconds(0.4f);
        }

        if (musicaHub != null)
            musicaHub.Play();
    }
}