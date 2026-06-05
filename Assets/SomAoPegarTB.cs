using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SomAoPegarTB : MonoBehaviour
{
    private AudioSource audioSource;
    private bool jaTocou = false; // Garante que o susto/áudio só aconteça uma vez, se quiser

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Configurações de segurança para o áudio
        audioSource.playOnAwake = false;
    }

    // Esta função pública vai ser chamada pelo sistema de VR quando o objeto for pego
    public void DispararSomObjeto()
    {
        // Se já coletou ou se não tem clipe de áudio, ignora
        if (jaTocou || audioSource == null || audioSource.clip == null) return;

        jaTocou = true;
        audioSource.Play();
        Debug.Log($"🔊 [Áudio] Objeto pego! Tocando som de {audioSource.clip.length} segundos até o final.");
    }
}