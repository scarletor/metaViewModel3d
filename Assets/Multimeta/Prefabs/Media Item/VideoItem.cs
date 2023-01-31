using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoItem : MonoBehaviour
{
    [SerializeField] Button selectButton;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] RawImage thumbnail;

    IServices services => MultimetaService.Instance;
    VideoItemData videoItemData;

    public class VideoItemData
    {
        public string url;
        public string name;
        public string cover;
        public string price;

        public VideoItemData(JObject jobject)
        {
            url = "https:" + jobject["url"].Value<string>();
            name = jobject["name"].Value<string>();
            cover = "https:" + jobject["cover"].Value<string>();
            price = jobject["price"].Value<string>();
        }
    }

    void Start()
    {
        selectButton.onClick.AddListener(OnSelectButtonClick);
    }

    private void OnSelectButtonClick()
    {
        UIManager.Instance.OpenMenu(UIManager.MenuType.MediaDetailMenu);
        UIManager.Instance.mediaMenuDetail.SetData(nameText.text, priceText.text, thumbnail.texture, videoItemData.url, MediaMenuDetailController.MediaType.Video);
    }

    public void SetVideoItemData(VideoItemData videoItemData)
    {
        this.videoItemData = videoItemData;

        nameText.text = videoItemData.name;
        priceText.text = videoItemData.price;
        // Download thumbnail
        services.DownloadImage(videoItemData.cover,
            onSuccess: (texture) =>
            {
                thumbnail.texture = texture;
            });
    }
}
