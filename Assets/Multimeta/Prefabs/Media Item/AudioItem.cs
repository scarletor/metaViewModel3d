using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioItem : MonoBehaviour
{
    [SerializeField] Button selectButton;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] RawImage thumbnail;

    IServices services => MultimetaService.Instance;
    AudioItemData audioItemData;

    public class AudioItemData
    {
        public string url;
        public string name;
        public string cover;
        public string price;

        public AudioItemData(JObject jobject)
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
        UIManager.Instance.mediaMenuDetail.SetData(nameText.text, priceText.text, thumbnail.texture, audioItemData.url, MediaMenuDetailController.MediaType.Audio);
    }

    public void SetAudioItemData(AudioItemData audioItemData)
    {
        this.audioItemData = audioItemData;

        nameText.text = audioItemData.name;
        priceText.text = audioItemData.price;
        // Download thumbnail
        services.DownloadImage(audioItemData.cover, 
            onSuccess: (texture) => 
            {
                thumbnail.texture = texture;
            });
    }
}
