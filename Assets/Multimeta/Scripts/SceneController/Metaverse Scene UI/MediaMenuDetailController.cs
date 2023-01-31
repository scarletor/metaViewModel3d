using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MediaMenuDetailController : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] Button pauseButton;
    [SerializeField] Slider timeSlider;

    [Header("Player Source")]
    [SerializeField] AudioSource audioPlayer;
    [SerializeField] VideoPlayer videoPlayer;

    [Header("Media Info")]
    [SerializeField] TextMeshProUGUI mediaNameText;
    [SerializeField] TextMeshProUGUI mediaPriceText;
    [SerializeField] RawImage thumbnailImage;

    IServices services => MultimetaService.Instance;

    public enum MediaType
    {
        Audio, 
        Video,
    }

    void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClick);
        pauseButton.onClick.AddListener(OnPauseButtonClick);
    }

    void Update()
    {
        if (videoPlayer.isPlaying)
            timeSlider.value = (float)videoPlayer.frame / (float)videoPlayer.frameCount;

        if (audioPlayer.isPlaying)
            timeSlider.value = (float)audioPlayer.time / (float)audioPlayer.clip.length;
    }

    void OnDisable()
    {
        PlayPauseAudio(false);
        PlayPauseVideo(false);
    }

    private void PlayPauseAudio(bool isPlay)
    {
        if (isPlay)
            audioPlayer.Play();
        else
            audioPlayer.Pause();

        playButton.gameObject.SetActive(!isPlay);
        pauseButton.gameObject.SetActive(isPlay);
    }

    private void PlayPauseVideo(bool isPlay)
    {
        if (isPlay)
            videoPlayer.Play();
        else
            videoPlayer.Pause();

        playButton.gameObject.SetActive(!isPlay);
        pauseButton.gameObject.SetActive(isPlay);
    }

    private void OnPauseButtonClick()
    {
        PlayPauseAudio(false);
        PlayPauseVideo(false);
    }

    private void OnPlayButtonClick()
    {
        if (audioPlayer.gameObject.activeInHierarchy)
            PlayPauseAudio(true);
        else
            PlayPauseVideo(true);
    }

    public void SetData(string name, string price, Texture thumbnail, string url, MediaType type)
    {
        audioPlayer.gameObject.SetActive(false);
        videoPlayer.gameObject.SetActive(false);

        mediaNameText.text = name;
        mediaPriceText.text = price;
        thumbnailImage.texture = thumbnail;
        thumbnailImage.GetComponent<AspectRatioFitter>().aspectRatio = thumbnail.width / thumbnail.height;

        switch (type)
        {
            case MediaType.Audio:
                services.DownloadAudio(url, onSuccess: (audioClip) => 
                { 
                    audioPlayer.clip = audioClip;
                    audioPlayer.gameObject.SetActive(true);
                    PlayPauseAudio(true);
                });
                break;

            case MediaType.Video:
                videoPlayer.url = url;
                videoPlayer.gameObject.SetActive(true);
                PlayPauseVideo(true);
                break;
        }
    }
}
