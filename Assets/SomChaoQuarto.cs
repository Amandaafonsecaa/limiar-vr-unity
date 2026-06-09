using UnityEngine;
using System.Collections;

public class SomChaoQuarto : MonoBehaviour
{
    [Header("Áudio do Quarto (Loop)")]
    [Tooltip("Arraste aqui o AudioSource com o som ensurdecedor/ressonância.")]
    public AudioSource somExternoLoop;

    [Header("Sistema de Legendas (Opcional)")]
    [SerializeField] private LegendaVR sistemaLegenda;
    public SubtitleTrigger.LinhaLegenda[] linhasAoEntrar;
    [SerializeField] private bool dispararLegendaApenasUmaVez = true;

    private bool jaDisparouLegenda = false;

    void Start()
    {
        if (somExternoLoop != null) 
        {
            somExternoLoop.playOnAwake = false;
            somExternoLoop.loop = true; // Garante o loop contínuo
        }
    }

    // Quando o jogador pisa em qualquer parte do chão do quarto
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<Camera>() != null || other.name.Contains("Origin"))
        {
            Debug.Log("[Quarto] Jogador pisou no quarto. Ligando som em loop.");
            
            if (somExternoLoop != null && !somExternoLoop.isPlaying)
            {
                somExternoLoop.Play();
            }

            // Dispara a legenda se configurada
            if (sistemaLegenda != null && linhasAoEntrar != null && linhasAoEntrar.Length > 0)
            {
                if (dispararLegendaApenasUmaVez && jaDisparouLegenda) return;

                jaDisparouLegenda = true;
                sistemaLegenda.MostrarSequencia(linhasAoEntrar);
            }
        }
    }

    // Quando o jogador sai do quarto (vai para o corredor)
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<Camera>() != null || other.name.Contains("Origin"))
        {
            Debug.Log("[Quarto] Jogador saiu do quarto. Desligando som.");

            if (somExternoLoop != null)
            {
                somExternoLoop.Stop();
            }
        }
    }
}