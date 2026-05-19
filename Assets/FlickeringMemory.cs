using UnityEngine;

public class FlickeringMemory : MonoBehaviour
{
    public SpriteRenderer spriteMae; // Arraste o Sprite da Mãe aqui
    public Light luzQuarto;         // Arraste a Point Light aqui
    
    [Header("Configurações de Piscada")]
    public float tempoMin = 0.05f;
    public float tempoMax = 0.3f;
    public float chanceDeApagar = 0.4f; // 40% de chance de ficar apagado

    private float cronometro;

    void Update()
    {
        cronometro -= Time.deltaTime;

        if (cronometro <= 0)
        {
            // Sorteia se a luz/imagem vai estar ligada ou desligada
            bool estaAtivo = Random.value > chanceDeApagar;

            if (estaAtivo)
            {
                luzQuarto.enabled = true;
                // Deixa o sprite visível (Alpha = 1)
                Color c = spriteMae.color;
                c.a = 1f;
                spriteMae.color = c;
            }
            else
            {
                luzQuarto.enabled = false;
                // Deixa o sprite invisível (Alpha = 0)
                Color c = spriteMae.color;
                c.a = 0f;
                spriteMae.color = c;
            }

            // Define quanto tempo vai durar esse estado (aceso ou apagado)
            cronometro = Random.Range(tempoMin, tempoMax);
        }
    }
}