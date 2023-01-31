using ExitGames.Client.Multimeta;
using Newtonsoft.Json.Linq;
using ReadyPlayerMe;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static ExitGames.Client.Multimeta.MultimetaPlayer;

public class MetaverseSceneController : MonoBehaviour
{
    [SerializeField] GameObject[] maleAvatarPrefabs;
    [SerializeField] GameObject[] femaleAvatarPrefabs;
    [SerializeField] Avatar maleAvatar;
    [SerializeField] Avatar femaleAvatar;
    [SerializeField] GameObject avatarPreviewContainer;
    [SerializeField] GameObject remotePlayerPrefab;

    void Start()
    {
        BackgroundProgress.Instance.MetaVerseRoom.SetActive(true);
        LoadAvatar();
        SpawnRemotePlayer();

        PhotonNetworkManager.Instance.OnPlayerJoined += OnPlayerJoinedHandle;
        PhotonNetworkManager.Instance.OnPlayerLeaved += OnPlayerLeavedHandle;
    }

    void OnDestroy()
    {
        PhotonNetworkManager.Instance.OnPlayerJoined -= OnPlayerJoinedHandle;
        PhotonNetworkManager.Instance.OnPlayerLeaved -= OnPlayerLeavedHandle;
        PhotonNetworkManager.Instance.StopGame();
    }

    async void LoadAvatar()
    {
        var avatarPrefabs = maleAvatarPrefabs;
        if (!GameContext.IsMaleAvatar)
            avatarPrefabs = femaleAvatarPrefabs;

        try
        {
            var myAvatar = GameContext.SelectedMetaverseData.meta_setting.my_avatar;
            Vector3 position = new Vector3(myAvatar["x"].Value<float>(), myAvatar["y"].Value<float>(), myAvatar["z"].Value<float>());
            avatarPreviewContainer.transform.position = position;
        }
        catch { }

        Destroy(avatarPreviewContainer.transform.GetChild(3).gameObject);

        if (GameContext.RPMAvatar == null)
        {
            var localAvatar = Instantiate(avatarPrefabs[GameContext.AvatarIndex], avatarPreviewContainer.transform);
            localAvatar.transform.localPosition = Vector3.zero;
            localAvatar.transform.localEulerAngles = Vector3.zero;
            localAvatar.transform.localScale = Vector3.one;

            Animator anim = avatarPreviewContainer.GetComponentInChildren<Animator>();
            if (GameContext.IsMaleAvatar)
                anim.avatar = maleAvatar;
            else
                anim.avatar = femaleAvatar;
        }
        else
        {
            var rpmAvatar = Instantiate(GameContext.RPMAvatar.gameObject, avatarPreviewContainer.transform);
            rpmAvatar.transform.localPosition = Vector3.zero;
            rpmAvatar.transform.localEulerAngles = Vector3.zero;
            rpmAvatar.transform.localScale = Vector3.one;
            if (rpmAvatar.GetComponent<Animator>())
                rpmAvatar.GetComponent<Animator>().enabled = false;
            rpmAvatar.gameObject.SetActive(true);

            Animator anim = avatarPreviewContainer.GetComponentInChildren<Animator>();
            anim.avatar = rpmAvatar.GetComponent<Animator>().avatar;
        }

        avatarPreviewContainer.gameObject.SetActive(false);
        await Task.Delay(500);
        avatarPreviewContainer.gameObject.SetActive(true);

        if (PhotonNetworkManager.LocalPlayer != null)
        {
            PhotonNetworkManager.LocalPlayer.PlayerAvatar = avatarPreviewContainer;
            PhotonNetworkManager.Instance.UpdateLocalPlayerInfo();
        }
    }

    private void OnPlayerJoinedHandle(MultimetaPlayer particlePlayer)
    {
        if (particlePlayer.IsLocal)
        {
            PhotonNetworkManager.LocalPlayer.PlayerAvatar = avatarPreviewContainer;
        }

        SpawnRemotePlayer();
        PhotonNetworkManager.Instance.UpdateLocalPlayerInfo();
    }

    private void OnPlayerLeavedHandle(MultimetaPlayer particlePlayer)
    {
        Destroy(particlePlayer.PlayerAvatar.gameObject);
    }

    void SpawnRemotePlayer()
    {
        foreach (var player in PhotonNetworkManager.RemotePlayers.Values)
        {
            if (player.PlayerAvatar != null)
                continue;

            GameObject avatar = Instantiate(remotePlayerPrefab);
            avatar.transform.localPosition = Vector3.zero;
            avatar.transform.localEulerAngles = Vector3.zero;
            avatar.transform.localScale = Vector3.one;

            player.PlayerAvatar = avatar;
            player.OnReadEvPlayerInfo += LoadRemotePlayerInfo;

            foreach (var playerInfo in player.cachedPlayerInfo)
                LoadRemotePlayerInfo(playerInfo);
        }
    }

    async void LoadRemotePlayerInfo(PlayerInfo playerInfo)
    {
        if (playerInfo.remotePlayer.PlayerAvatar.transform.childCount > 1)
            return;

        playerInfo.remotePlayer.PlayerAvatar.GetComponentInChildren<TextMeshPro>().text = playerInfo.name;
        Animator anim = playerInfo.remotePlayer.PlayerAvatar.GetComponent<Animator>();

        if (string.IsNullOrEmpty(playerInfo.rpmUrl))
        {
            var avatarPrefabs = maleAvatarPrefabs;
            if (!playerInfo.isMale)
                avatarPrefabs = femaleAvatarPrefabs;

            var remoteAvatar = Instantiate(avatarPrefabs[playerInfo.index], playerInfo.remotePlayer.PlayerAvatar.transform);
            remoteAvatar.transform.localPosition = Vector3.zero;
            remoteAvatar.transform.localEulerAngles = Vector3.zero;
            remoteAvatar.transform.localScale = Vector3.one;
            
            if (playerInfo.isMale)
                anim.avatar = maleAvatar;
            else
                anim.avatar = femaleAvatar;

            playerInfo.remotePlayer.PlayerAvatar.gameObject.SetActive(false);
            await Task.Delay(500);
            playerInfo.remotePlayer.PlayerAvatar.gameObject.SetActive(true);
            anim.SetFloat("MotionSpeed", 1);
        }

        else
        {
            var avatarLoader = new AvatarLoader();
            avatarLoader.OnCompleted += async (sender, args) =>
            {
                Debug.Log($"Loaded avatar. [{Time.timeSinceLevelLoad:F2}]");

                //var rpmAvatar = Instantiate(args.Avatar, playerInfo.remotePlayer.playerAvatar.transform);
                var rpmAvatar = args.Avatar;
                rpmAvatar.transform.SetParent(playerInfo.remotePlayer.PlayerAvatar.transform, false);
                rpmAvatar.transform.localPosition = Vector3.zero;
                rpmAvatar.transform.localEulerAngles = Vector3.zero;
                rpmAvatar.transform.localScale = Vector3.one;
                if (rpmAvatar.GetComponent<Animator>())
                    rpmAvatar.GetComponent<Animator>().enabled = false;
                rpmAvatar.gameObject.SetActive(true);

                anim.avatar = rpmAvatar.GetComponent<Animator>().avatar;
                playerInfo.remotePlayer.PlayerAvatar.gameObject.SetActive(false);
                await Task.Delay(500);
                playerInfo.remotePlayer.PlayerAvatar.gameObject.SetActive(true);
                anim.SetFloat("MotionSpeed", 1);
            };
            avatarLoader.OnFailed += (sender, args) =>
            {
                Debug.Log(args.Type);
            };

            avatarLoader.LoadAvatar(playerInfo.rpmUrl);
        }
    }
}
