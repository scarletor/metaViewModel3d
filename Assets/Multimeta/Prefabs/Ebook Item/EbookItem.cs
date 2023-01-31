using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EbookItem : MonoBehaviour
{
    [SerializeField] Button buyButton;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] RawImage thumbnail;

    IServices services => MultimetaService.Instance;
    EbookItemData ebookItemData;

    public class EbookItemData
    {
        public string url;
        public string name;
        public string cover;
        public string price;

        public EbookItemData(JObject jobject)
        {
            url = "https:" + jobject["url"].Value<string>();
            name = jobject["name"].Value<string>();
            cover = "https:" + jobject["cover"].Value<string>();
            price = jobject["price"].Value<string>();
        }
    }

    void Start()
    {
        buyButton.onClick.AddListener(OnBuyButtonClick);
    }

    private void OnBuyButtonClick()
    {
        UIManager.Instance.OpenMenu(UIManager.MenuType.EbookDetailMenu);
        UIManager.Instance.ebookMenuDetail.SetData(thumbnail.texture, nameText.text, priceText.text, ebookItemData.url);
    }

    public void SetEbookItemData(EbookItemData ebookItemData)
    {
        this.ebookItemData = ebookItemData;

        nameText.text = ebookItemData.name;
        priceText.text = ebookItemData.price;
        // Download thumbnail
        services.DownloadImage(ebookItemData.cover,
            onSuccess: (texture) =>
            {
                thumbnail.texture = texture;
            });
    }
}
