using Newtonsoft.Json.Linq;
using UnityEngine;
using static EbookItem;

public class EbookMenuController : MonoBehaviour
{
    [SerializeField] Transform ebookContainer;
    [SerializeField] EbookItem ebookItemPrefab;

    // Start is called before the first frame update
    void Start()
    {
        ClearData();
        GetEbookList();
    }

    private void ClearData()
    {
        foreach (Transform child in ebookContainer)
            Destroy(child.gameObject);
    }

    private void GetEbookList()
    {
        JObject jo = GameContext.SelectedMetaverseData.media_config.ebook.ToObject<JObject>();
        JArray playList = jo["playlist"].Value<JArray>();
        foreach (var ebookDataJT in playList)
        {
            JObject ebookData = ebookDataJT.ToObject<JObject>();
            var ebookItem = Instantiate(ebookItemPrefab, ebookContainer);
            ebookItem.SetEbookItemData(new EbookItemData(ebookData));
        }
    }
}
