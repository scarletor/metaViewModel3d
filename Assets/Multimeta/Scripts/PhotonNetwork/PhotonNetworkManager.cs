using System;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Multimeta;

public class PhotonNetworkManager : SingletonMonoBehaviorBase<PhotonNetworkManager>
{
    public GameLogic GameLogic { get; private set; }

    public static MultimetaPlayer LocalPlayer;
    public static Dictionary<string, MultimetaPlayer> RemotePlayers = new Dictionary<string, MultimetaPlayer>();

    public event Action<MultimetaPlayer> OnPlayerJoined;
    public event Action<MultimetaPlayer> OnPlayerLeaved;

    private void Update()
    {
        UpdateLocal();
    }

    // Update is called once per frame
    private void UpdateLocal()
    {
        if (GameLogic != null)
        {
            GameLogic.UpdateLoop();
        }
    }

    /// <summary>
    /// Start connect to Photon Cloud AppId
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="gameVersion"></param>
    /// <param name="serverAddress">Is Optional</param>
    public void StartGame(string appId = "6c9ca915-4db7-450c-9a65-2035737850e1", string gameVersion = "1.0", string serverAddress = "")
    {
        // Initialize local game
        GameLogic = new GameLogic(appId, gameVersion);

        GameLogic.OnEventJoin = this.OnJoinedPlayer;
        GameLogic.OnEventLeave = this.OnLeavedPlayer;

        if (!string.IsNullOrEmpty(serverAddress))
        {
            GameLogic.MasterServerAddress = serverAddress;
        }
        GameLogic.CallConnect();
    }

    public void StopGame()
    {
        GameLogic.CallLeave();

        // Clear session datas
        LocalPlayer = null;
        RemotePlayers.Clear();
    }

    public void UpdateLocalPlayerInfo()
    {
        string rpmUrl = GameContext.RPMAvatar != null ? GameContext.RPMAvatar.url : string.Empty;
        var eventObj = LocalPlayer.WriteEvPlayerInfo(GameContext.UserName, GameContext.IsMaleAvatar, GameContext.AvatarIndex, rpmUrl);
        GameLogic.ChangeLocalPlayercolor(eventObj);
    }

    /// <summary>
    /// Handler for "Player Joined" Event
    /// </summary>
    /// <param name="particlePlayer">Player that joined the game</param>
    private void OnJoinedPlayer(MultimetaPlayer particlePlayer)
    {
        if (!particlePlayer.IsLocal)
        {
            lock (RemotePlayers)
            {
                if (!RemotePlayers.ContainsKey(particlePlayer.NickName))
                {
                    //GameObject playerCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //playerCube.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
                    //playerCube.name = particlePlayer.NickName;
                    //cubes.Add(playerCube);

                    RemotePlayers.Add(particlePlayer.NickName, particlePlayer);
                    Debug.Log(particlePlayer.NickName + "Joinded");
                }
            }
        }
        else
        {
            LocalPlayer = particlePlayer;

            lock (GameLogic)
            {
                foreach (MultimetaPlayer p in GameLogic.LocalRoom.Players.Values)
                {
                    //foreach (GameObject cube in cubes)
                    //{
                    //    if (cube.name == p.NickName) return;
                    //}
                    //GameObject playerCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //playerCube.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
                    //playerCube.name = p.NickName;
                    //cubes.Add(playerCube);
                    //remotePlayers.Add(p.NickName, p);
                    //Debug.Log(particlePlayer.NickName + "Joinded Local");

                    if (!p.IsLocal)
                        RemotePlayers.Add(p.NickName, p);
                }
            }
        }

        OnPlayerJoined?.Invoke(particlePlayer);
    }

    /// <summary>
    /// Handler for "Player Leaved" Event
    /// </summary>
    /// <param name="particlePlayer">Player that leaved the game</param>
    private void OnLeavedPlayer(MultimetaPlayer particlePlayer)
    {
        string name = particlePlayer.NickName;
        RemotePlayers.Remove(name);

        //foreach (GameObject cube in cubes)
        //{
        //    if (cube.name == name)
        //    {
        //        remotePlayers.Remove(name);
        //        cubes.Remove(cube);
        //        GameObject.Destroy(cube);
        //        return;
        //    }
        //}

        OnPlayerLeaved?.Invoke(particlePlayer);
    }
}
