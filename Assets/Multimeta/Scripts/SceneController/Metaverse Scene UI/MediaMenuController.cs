using Newtonsoft.Json.Linq;
using UnityEngine;
using static AudioItem;
using static VideoItem;

public class MediaMenuController : MonoBehaviour
{
    [Header("Audio Component")]
    [SerializeField] Transform audioContainer;
    [SerializeField] AudioItem audioItemPrefab;

    [Header("Vdieo Component")]
    [SerializeField] Transform videoContainer;
    [SerializeField] VideoItem videoItemPrefab;

    // Start is called before the first frame update
    void Start()
    {
        ClearData();
        GetAudioList();
        GetVideoList();
    }

    private void ClearData()
    {
        foreach (Transform child in audioContainer)
            Destroy(child.gameObject);

        foreach (Transform child in videoContainer)
            Destroy(child.gameObject);
    }

    private void GetAudioList()
    {
        JObject jo = GameContext.SelectedMetaverseData.media_config.audio.ToObject<JObject>();
        JArray playList = jo["playlist"].Value<JArray>();
        foreach (var audioDataJT in playList)
        {
            JObject audioData = audioDataJT.ToObject<JObject>();
            var audioItem = Instantiate(audioItemPrefab, audioContainer);
            audioItem.SetAudioItemData(new AudioItemData(audioData));
        }
    }

    private void GetVideoList()
    {
        JObject jo = GameContext.SelectedMetaverseData.media_config.video.ToObject<JObject>();
        JArray playList = jo["playlist"].Value<JArray>();
        foreach (var videoDataJT in playList)
        {
            JObject videoData = videoDataJT.ToObject<JObject>();
            var videoItem = Instantiate(videoItemPrefab, videoContainer);
            videoItem.SetVideoItemData(new VideoItemData(videoData));
        }
    }
}
