using UnityEngine;

public class MonstroParado : MonoBehaviour
{
    [Header("Som do Susto")]
    public AudioSource somAoSumir; // Arraste o som/fala do monstro aqui

    private Animator animator;
    private bool jaSumiu = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.Play("Idle", 0); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // O "if" garante que o susto só aconteça uma vez
        if (!jaSumiu)
        {
            SumirEFecharObjeto();
        }
    }

    void SumirEFecharObjeto()
    {
        jaSumiu = true;

        // 1. Toca a fala do monstro se ela foi colocada no Inspector
        if (somAoSumir != null)
        {
            somAoSumir.Play();
        }

        // 2. O PULO DO GATO: Em vez de apagar o objeto inteiro e cortar o som,
        // nós apenas movemos o monstro para muito longe ou desativamos o visual para o som continuar rodando.
        // O jeito mais simples e seguro é destruir/desativar o monstro após o tempo do áudio acabar:
        float tempoDoSom = somAoSumir != null ? somAoSumir.clip.length : 0f;
        
        // Desativa o visual escondendo o monstro instantaneamente
        DesativarVisualDoMonstro();

        // Destrói ou desativa o objeto completo após os segundos da fala terminarem
        Destroy(gameObject, tempoDoSom);
    }

    void DesativarVisualDoMonstro()
    {
        // Esconde todos os renderizadores de modelo do monstro para ele ficar invisível
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }
    }
}