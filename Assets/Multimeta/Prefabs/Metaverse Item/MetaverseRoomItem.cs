using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ModelDatas;

public class MetaverseRoomItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image metaThumbnail;

    [SerializeField] Button selectBtn;
    [SerializeField] Sprite[] textures;

    private MetaVerseData metaVerseData;
    private event Action<MetaVerseData> OnItemClicked;

    IServices services => MultimetaService.Instance;

    // Start is called before the first frame update
    void Awake()
    {
        selectBtn.onClick.AddListener(OnSelectButtonClick);
    }

    public void SetData(MetaVerseData metaVerseData, Action<MetaVerseData> OnItemClicked)
    {
        this.metaVerseData = metaVerseData;
        this.OnItemClicked = OnItemClicked;

        nameText.text = metaVerseData.name;
        metaThumbnail.gameObject.SetActive(false);
        // Load meta thumbnail
        if (metaVerseData.thumbnail != null) 
        {
            string thumbnailUrl = "https://asset.airclass.io/public/" + metaVerseData.thumbnail;
            services.DownloadImage(thumbnailUrl, OnDownloadThumbnailSuccess, OnDownloadThumbnailFailure);
        }
    }

    private void OnDownloadThumbnailFailure(string obj)
    {
        // Hide loading anim
        nameText.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void OnDownloadThumbnailSuccess(Texture2D obj)
    {
        var spriteImg = Sprite.Create(obj, new Rect(0.0f, 0.0f, obj.width, obj.height), new Vector2(0.5f, 0.5f), 100.0f);
        metaThumbnail.sprite = spriteImg;
        metaThumbnail.gameObject.SetActive(true);
    }

    public void OnSelectButtonClick()
    {
        OnItemClicked?.Invoke(metaVerseData);
    }
}
