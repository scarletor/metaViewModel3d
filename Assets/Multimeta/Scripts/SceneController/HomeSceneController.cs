using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static ModelDatas;

public class HomeSceneController : MonoBehaviour
{
    [SerializeField] GameObject metaverseRoomPrefab;
    [SerializeField] Transform metaverseTableContent;
    [SerializeField] ScrollRect scrollRect;

    public bool autoLogin;

    IServices services =>  MultimetaService.Instance;

    private const string BDSG_GROUP = "1";
    private int currentPageNum;
    private int totalItem;
    private bool isLoading;

    void Start()
    {
        if (autoLogin)
            services.SignIn("nam.vh@gmail.com", "admin", (respData) => { LoadMetaverses(); });
        else
            LoadMetaverses();

        scrollRect.onValueChanged.AddListener(OnTableScroll);
    }

    private void OnTableScroll(Vector2 value)
    {
        if (isLoading)
            return;

        if (1 - value.y >= 0.4f)
        {
            CheckUpdateListMetaverse();
        }
    }

    private void CheckUpdateListMetaverse()
    {
        if (metaverseTableContent.transform.childCount < totalItem)
            LoadNextPage();
    }

    private void LoadMetaverses()
    {
        currentPageNum = 1;
        isLoading = true;
        services.GetListMetaVerses(BDSG_GROUP, currentPageNum, (data) => { OnGetListMetaverseSuccess(data, true); });
    }

    private void LoadNextPage()
    {
        currentPageNum++;
        isLoading = true;
        services.GetListMetaVerses(BDSG_GROUP, currentPageNum, (data) => { OnGetListMetaverseSuccess(data, false); });
    }

    private void OnGetListMetaverseSuccess(ListMetaVerseResData listMetaVerseResData, bool isRefresh)
    {
        if (isRefresh)
        {
            foreach (Transform child in metaverseTableContent)
                Destroy(child.gameObject);
        }

        foreach (var metaverse in listMetaVerseResData.results)
        {
            GameObject metaverseRoom = Instantiate(metaverseRoomPrefab, metaverseTableContent);
            var metaverseRoomItem = metaverseRoom.GetComponent<MetaverseRoomItem>();
            metaverseRoomItem.SetData(metaverse, OnMetaverseItemSelected);
        }

        totalItem = listMetaVerseResData.count;
        isLoading = false;
    }

    private void OnMetaverseItemSelected(MetaVerseData metaVerseData)
    {
        GameContext.SelectedMetaverseData = metaVerseData;
        GameContext.RoomName = metaVerseData.name;
        GoToMetaverseScene();
    }

    public void GoToMetaverseScene()
    {
        PhotonNetworkManager.Instance.StartGame();
        SceneManager.LoadScene(SceneNameConfig.LOADING_SCENE);
    }
}
