using ReadyPlayerMe;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AvatarSceneController : MonoBehaviour
{
    [SerializeField] Button confirmBtn;

    [SerializeField] TMP_InputField userNameIp;
    [SerializeField] Button maleBtn;
    [SerializeField] Button femaltBtn;
    [SerializeField] Button createAvatarBtn;

    [SerializeField] Button avatar01Btn;
    [SerializeField] Button avatar02Btn;
    [SerializeField] Button avatar03Btn;
    [SerializeField] Button avatar04Btn;

    [SerializeField] GameObject[] maleAvatarPrefabs;
    [SerializeField] GameObject[] femaleAvatarPrefabs;
    [SerializeField] Avatar maleAvatar;
    [SerializeField] Avatar femaleAvatar;

    [SerializeField] GameObject avatarPreviewContainer;

    [SerializeField] private WebView webView;
    [SerializeField] private GameObject loadingLabel;
    [SerializeField] private GameObject loadingEnviromentLabel;

    private bool isMale = true;

    void Awake()
    {
        //Screen.orientation = ScreenOrientation.LandscapeLeft;
        webView.KeepSessionAlive = false;

        confirmBtn.interactable = false;
        confirmBtn.onClick.AddListener(OnConfirm);

        maleBtn.onClick.AddListener(delegate { OnChangeGender(true); });
        femaltBtn.onClick.AddListener(delegate { OnChangeGender(false); });
        createAvatarBtn.onClick.AddListener(OnCreateAvatarClick);

        avatar01Btn.onClick.AddListener(delegate { OnAvatarSelected(0); });
        avatar02Btn.onClick.AddListener(delegate { OnAvatarSelected(1); });
        avatar03Btn.onClick.AddListener(delegate { OnAvatarSelected(2); });
        avatar04Btn.onClick.AddListener(delegate { OnAvatarSelected(3); });

        BackgroundProgress.Instance.OnLoadMetaverseRoomCompleted += OnLoadEnviromentCompleted;
    }

    void OnDestroy()
    {
        BackgroundProgress.Instance.OnLoadMetaverseRoomCompleted -= OnLoadEnviromentCompleted;
    }

    void OnLoadEnviromentCompleted()
    {
        loadingEnviromentLabel.gameObject.SetActive(false);
        confirmBtn.interactable = true;
    }

    void OnChangeGender(bool isMale)
    {
        this.isMale = isMale;
        OnAvatarSelected(GameContext.AvatarIndex);
        ShowAvatarImage(isMale);
    }

    void ShowAvatarImage(bool isMale)
    {
        if (isMale)
        {
            avatar01Btn.transform.GetChild(1).gameObject.SetActive(true);
            avatar02Btn.transform.GetChild(1).gameObject.SetActive(true);
            avatar03Btn.transform.GetChild(1).gameObject.SetActive(true);
            avatar04Btn.transform.GetChild(1).gameObject.SetActive(true);

            avatar01Btn.transform.GetChild(2).gameObject.SetActive(false);
            avatar02Btn.transform.GetChild(2).gameObject.SetActive(false);
            avatar03Btn.transform.GetChild(2).gameObject.SetActive(false);
            avatar04Btn.transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            avatar01Btn.transform.GetChild(1).gameObject.SetActive(false);
            avatar02Btn.transform.GetChild(1).gameObject.SetActive(false);
            avatar03Btn.transform.GetChild(1).gameObject.SetActive(false);
            avatar04Btn.transform.GetChild(1).gameObject.SetActive(false);

            avatar01Btn.transform.GetChild(2).gameObject.SetActive(true);
            avatar02Btn.transform.GetChild(2).gameObject.SetActive(true);
            avatar03Btn.transform.GetChild(2).gameObject.SetActive(true);
            avatar04Btn.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    async void OnAvatarSelected(int index)
    {
        var avatarPrefabs = maleAvatarPrefabs;
        if (!isMale)
            avatarPrefabs = femaleAvatarPrefabs;

        Destroy(avatarPreviewContainer.transform.GetChild(0).gameObject);
        if (GameContext.RPMAvatar != null)
        {
            Destroy(GameContext.RPMAvatar.gameObject);
            GameContext.RPMAvatar = null; 
        }
        loadingLabel.SetActive(false);

        GameObject avatar = Instantiate(avatarPrefabs[index], avatarPreviewContainer.transform);
        avatar.transform.localPosition = Vector3.zero;
        avatar.transform.localEulerAngles = new Vector3(0, 180, 0);
        avatar.transform.localScale = Vector3.one;

        Animator anim = avatarPreviewContainer.GetComponentInChildren<Animator>();
        if (isMale)
            anim.avatar = maleAvatar;
        else
            anim.avatar = femaleAvatar;

        avatarPreviewContainer.gameObject.SetActive(false);
        await Task.Delay(500);
        avatarPreviewContainer.gameObject.SetActive(true);

        UpdateAvatarContext(isMale, index);
    }

    void UpdateAvatarContext(bool isMaleAvatar, int avatarIndex)
    {
        GameContext.IsMaleAvatar = isMaleAvatar;
        GameContext.AvatarIndex = avatarIndex;
    }

    void OnConfirm()
    {
        GameContext.UserName = userNameIp.text;
        SceneManager.LoadScene(SceneNameConfig.METAVERSE_SCENE);
    }

    void OnCreateAvatarClick()
    {
        DisplayWebView();
        loadingLabel.SetActive(true);
        //Screen.orientation = ScreenOrientation.Portrait;
    }

    #region RDM API

    // Display WebView or create it if not initialized yet 
    private void DisplayWebView()
    {
        if (webView.Loaded)
        {
            webView.SetVisible(true);
        }
        else
        {
            webView.CreateWebView();
            webView.OnAvatarCreated = OnAvatarCreated;
        }
    }

    // WebView callback for retrieving avatar url
    private void OnAvatarCreated(string url)
    {
        //if (avatar) Destroy(avatar);
        //Screen.orientation = ScreenOrientation.LandscapeLeft;
        webView.SetVisible(false);

        Destroy(avatarPreviewContainer.transform.GetChild(0).gameObject);

        var avatarLoader = new AvatarLoader();
        avatarLoader.OnCompleted += Completed;
        avatarLoader.OnFailed += Failed;
        avatarLoader.LoadAvatar(url);
    }

    private void Failed(object sender, FailureEventArgs args)
    {
        Debug.LogError(args.Type);
    }

    // AvatarLoader callback for retrieving loaded avatar game object
    private void Completed(object sender, CompletionEventArgs args)
    {
        if (avatarPreviewContainer.transform.childCount > 0)
            return;

        var avatar = args.Avatar;
        var url = args.Url;

        loadingLabel.SetActive(false);

        avatar.transform.SetParent(avatarPreviewContainer.transform, false);
        avatar.transform.localEulerAngles = new Vector3(0, 180, 0);

        RPMAvatar rpmAvatar = Instantiate(avatar).AddComponent<RPMAvatar>();
        rpmAvatar.gameObject.SetActive(false);
        rpmAvatar.url = url;
        GameContext.RPMAvatar = rpmAvatar;
    }

    #endregion
}