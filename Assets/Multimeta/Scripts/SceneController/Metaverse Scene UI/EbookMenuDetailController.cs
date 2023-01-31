using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EbookMenuDetailController : MonoBehaviour
{
    [SerializeField] Button buyButton;
    [SerializeField] RawImage thumbnailImage;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI priceText;

    void Start()
    {
        buyButton.onClick.AddListener(OnBuyButtonClick);
    }

    private void OnBuyButtonClick()
    {
        SceneManager.LoadScene(SceneNameConfig.PDFVIEWER_SCENE, LoadSceneMode.Additive);
    }

    public void SetData(Texture thumbnail, string name, string price, string url)
    {
        thumbnailImage.texture = thumbnail;
        nameText.text = name;
        priceText.text = price;
        GameContext.EbookUrl = url;
    }
}
