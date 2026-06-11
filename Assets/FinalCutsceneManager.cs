using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class FinalCutsceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public VideoClip primeiraCutscene;
    public VideoClip segundaCutscene;

    public string cenaCreditos = "Creditos";

    private int etapa = 1;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;

        videoPlayer.clip = primeiraCutscene;
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (etapa == 1)
        {
            etapa = 2;
            videoPlayer.clip = segundaCutscene;
            videoPlayer.Play();
        }
        else
        {
            SceneManager.LoadScene(cenaCreditos);
        }
    }
}