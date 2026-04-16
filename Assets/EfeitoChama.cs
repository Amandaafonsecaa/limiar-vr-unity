using UnityEngine;
using System.Collections;

public class EfeitoChama : MonoBehaviour
{
    private Light luz;
    private Vector3 posicaoOriginal;

    [Header("Brilho da Vela")]
    public float intensidadeMin = 0.6f;
    public float intensidadeMax = 1.1f;
    public float velocidadeOscilacao = 0.07f;

    [Header("Tremor do Fogo")]
    public float forcaDoVento = 0.01f; 

    void Start()
    {
        luz = GetComponent<Light>();
        posicaoOriginal = transform.localPosition;

        if (luz != null)
        {
            StartCoroutine(AnimarChama());
        }
    }

    IEnumerator AnimarChama()
    {
        while (true)
        {
            // Variação de intensidade aleatória
            luz.intensity = Random.Range(intensidadeMin, intensidadeMax);

            // Pequeno tremor na posição para sombras dinâmicas
            transform.localPosition = posicaoOriginal + (Random.insideUnitSphere * forcaDoVento);

            yield return new WaitForSeconds(velocidadeOscilacao);
        }
    }
}