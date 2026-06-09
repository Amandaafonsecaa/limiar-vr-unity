using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroVideoVR : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        if (videoPlayer != null)
        {
            // Quando o vídeo terminar
            videoPlayer.loopPointReached += VideoFinished;

            // Inicia o vídeo
            videoPlayer.Play();
        }
        else
        {
            Debug.LogError("VideoPlayer não foi atribuído!");
        }
    }

    void VideoFinished(VideoPlayer vp)
    {
        Debug.Log("Vídeo terminou!");

        // Carrega a cena do Hub
        SceneManager.LoadScene("Hub");
    }
}