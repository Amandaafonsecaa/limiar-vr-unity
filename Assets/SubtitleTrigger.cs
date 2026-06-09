using UnityEngine;
using System.Collections;

public class SubtitleTrigger : MonoBehaviour
{
    [System.Serializable]
    public class LinhaLegenda
    {
        [TextArea(2, 4)]
        public string texto;
        public float duracao = 3f;
    }

    public LegendaVR legendaVR;
    public LinhaLegenda[] falas;

    public bool tocarAoIniciar = false;
    public bool tocarUmaVez = true;

    public AudioSource musicaDepois;

    private bool jaTocou = false;

    void Start()
    {
        if (tocarAoIniciar)
            TocarLegenda();
    }

    public void TocarLegenda()
    {
        if (tocarUmaVez && jaTocou) return;

        jaTocou = true;

        StartCoroutine(TocarSequencia());
    }

    IEnumerator TocarSequencia()
    {
        if (legendaVR != null)
            legendaVR.MostrarSequencia(falas);

        float tempoTotal = 0f;

        foreach (LinhaLegenda linha in falas)
            tempoTotal += linha.duracao + 0.2f;

        yield return new WaitForSeconds(tempoTotal);

        if (musicaDepois != null)
            musicaDepois.Play();
    }
}