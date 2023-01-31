// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Exit Games GmbH">
//   Exit Games GmbH, 2012
// </copyright>
// <summary>
//   The "Particle" demo is a load balanced and Photon Cloud compatible "coding" demo.
//   The focus is on showing how to use the Photon features without too much "game" code cluttering the view.
// </summary>
// <author>developer@photonengine.com</author>
// --------------------------------------------------------------------------------------------------------------------

namespace ExitGames.Client.Multimeta
{
    using ExitGames.Client.Photon;
    using global::Photon.Realtime;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Extends Player with some Particle Demo specific properties and methods.
    /// </summary>
    /// <remarks>
    /// Instances of this class are created by GameLogic.CreatePlayer.
    /// There's a GameLogic.LocalPlayer field, that represents this user's player (in the room).
    ///
    /// This class does not make use of networking directly. It's updated by incoming events but
    /// the actual sending and receiving is handled in GameLogic.
    ///
    /// The WriteEv* and ReadEv* methods are simple ways to create event-content per player.
    /// Only the LocalPlayer per client will actually send data. This is used to update the remote
    /// clients of position (and color, etc).
    /// Receiving clients identify the corresponding Player and call ReadEv* to update that
    /// (remote) player.
    /// Read the remarks in WriteEvMove.
    /// </remarks>
    public class MultimetaPlayer : Player
    {
        public GameObject PlayerAvatar { get; set; }

        /// <summary>
        /// Stores this client's "group interest currently set on server" of this player (not necessarily the current one).
        /// </summary>
        public byte VisibleGroup { get; set; }

        public List<PlayerInfo> cachedPlayerInfo = new List<PlayerInfo>();

        public int UpdateAge { get { return GameLogic.Timestamp - this.LastUpdateTimestamp; } }

        private int LastUpdateTimestamp { get; set; }

        public event Action<PlayerInfo> OnReadEvPlayerInfo;

        public MultimetaPlayer(string nickName, int actorID, bool isLocal, Hashtable actorProperties) : base(nickName, actorID, isLocal, actorProperties)
        {
            //if (isLocal) { }
        }

        public class PlayerInfo
        {
            public string name;
            public bool isMale;
            public int index;
            public string rpmUrl;
            public MultimetaPlayer remotePlayer;

            public PlayerInfo(string name, bool isMale, int index, string rpmUrl, MultimetaPlayer remotePlayer)
            {
                this.name = name;
                this.isMale = isMale;
                this.index = index;
                this.rpmUrl = rpmUrl;
                this.remotePlayer = remotePlayer;
            }
        }

        /// <summary>
        /// Converts the player info into a string.
        /// </summary>
        /// <returns>String showing basic info about this player.</returns>
        //public override string ToString()
        //{
        //    return this.ActorNumber + "'" + this.NickName + "':" + this.GetGroup() + " " + this.PosX + ":" + this.PosY + " PlayerProps: " + SupportClass.DictionaryToString(this.CustomProperties);
        //}

        /// <summary>Creates the "custom content" Hashtable that is sent as position update.</summary>
        /// <remarks>
        /// As with event codes, the content of this event is arbitrary and "made up" for this demo.
        /// Your game (e.g.) could use floats as positions or you send a height and actions or state info.
        /// It makes sense to use numbers (best: bytes) as Hashtable key type, cause they are use less space.
        /// But this is not a requirement as you see in WriteEvColor.
        ///
        /// The position can only go up to 128 in this demo, so a byte[] technically is the best (leanest)
        /// choice here.
        /// </remarks>
        /// <returns>Hashtable for event "move" to update others</returns>
        public Hashtable WriteEvMove()
        {
            if (PlayerAvatar == null)
                return new Hashtable();

            var animator = PlayerAvatar.GetComponent<Animator>();
            float Speed = animator.GetFloat("Speed");
            var Jump = animator.GetBool("Jump");
            var Grounded = animator.GetBool("Grounded");
            var FreeFall = animator.GetBool("FreeFall");
            int animationState = 0;
            if (Jump)
                animationState = 1;
            else if (Grounded)
                animationState = 2;
            else if (FreeFall)
                animationState = 3;

            Hashtable evContent = new Hashtable();
            evContent[(byte)1] = $"{this.PlayerAvatar.transform.position.x}/{this.PlayerAvatar.transform.position.y}/{this.PlayerAvatar.transform.position.z}";
            evContent[(byte)2] = $"{this.PlayerAvatar.transform.eulerAngles.x}/{this.PlayerAvatar.transform.eulerAngles.y}/{this.PlayerAvatar.transform.eulerAngles.z}";
            evContent[(byte)3] = animationState;
            evContent[(byte)4] = Speed;

            //Debug.Log(evContent);

            return evContent;
        }

        /// <summary>Reads the "custom content" Hashtable that is sent as position update.</summary>
        /// <returns>Hashtable for event "move" to update others</returns>
        public void ReadEvMove(Hashtable evContent)
        {
            if (evContent.ContainsKey((byte)1))
            {
                if (PlayerAvatar != null)
                {
                    PlayerAvatar.transform.position = StringUtils.StringToVector3((string)evContent[(byte)1]);
                }
            }
            // js client event support (those can't send with byte-keys)
            //else if (evContent.ContainsKey("1")) {}

            if (evContent.ContainsKey((byte)2))
            {
                if (PlayerAvatar != null)
                {
                    PlayerAvatar.transform.eulerAngles = StringUtils.StringToVector3((string)evContent[(byte)2]);
                }
            }

            if (evContent.ContainsKey((byte)3))
            {
                if (PlayerAvatar != null)
                {
                    int animationState = (int) evContent[(byte)3];
                    PlayerAvatar.GetComponent<Animator>().SetBool("Jump", false);
                    PlayerAvatar.GetComponent<Animator>().SetBool("Grounded", false);
                    PlayerAvatar.GetComponent<Animator>().SetBool("FreeFall", false);
                    switch (animationState)
                    {
                        case 1:
                            PlayerAvatar.GetComponent<Animator>().SetBool("Jump", true);
                            break;
                        case 2:
                            PlayerAvatar.GetComponent<Animator>().SetBool("Grounded", true);
                            break;
                        case 3:
                            PlayerAvatar.GetComponent<Animator>().SetBool("FreeFall", true);
                            break;
                        default:
                            break;
                    }
                }
            }

            if (evContent.ContainsKey((byte)4))
            {
                if (PlayerAvatar != null)
                {
                    float animationSpeed = (float)evContent[(byte)4];
                    PlayerAvatar.GetComponent<Animator>().SetFloat("Speed", animationSpeed);
                }
            }

            this.LastUpdateTimestamp = GameLogic.Timestamp;
        }

        /// <summary>Creates the "custom content" Hashtable that is sent as color update.</summary>
        /// <returns>Hashtable for event "color" to update others</returns>
        public Hashtable WriteEvPlayerInfo(string name, bool isMale, int avatarIndex, string rpmUrl)
        {
            Hashtable evContent = new Hashtable();
            evContent[(byte)1] = name;
            evContent[(byte)2] = isMale;
            evContent[(byte)3] = avatarIndex;
            evContent[(byte)4] = rpmUrl;
            return evContent;
        }

        /// <summary>Reads the "custom content" Hashtable that is sent as color update.</summary>
        /// <returns>Hashtable for event "color" to update others</returns>
        public void ReadEvPlayerInfo(Hashtable evContent, MultimetaPlayer remotePlayer)
        {
            string name = "Player";
            bool isMale = true;
            int index = 0;
            string rpmUrl = "";

            //Debug.Log(evContent + "Player Info: " + remotePlayer.NickName);

            if (evContent.ContainsKey((byte)1))
            {
                name = (string)evContent[(byte)1];
            }
            // js client event support (those can't send with byte-keys)
            //else if (evContent.ContainsKey("1")) {}

            if (evContent.ContainsKey((byte)2))
            {
                isMale = (bool)evContent[(byte)2];
            }

            if (evContent.ContainsKey((byte)3))
            {
                index = (int)evContent[(byte)3];
            }

            if (evContent.ContainsKey((byte)4))
            {
                rpmUrl = (string)evContent[(byte)4];
            }

            var playerInfo = new PlayerInfo(name, isMale, index, rpmUrl, remotePlayer);

            if (OnReadEvPlayerInfo != null)
                OnReadEvPlayerInfo.Invoke(playerInfo);
            else
                cachedPlayerInfo.Add(playerInfo);

            this.LastUpdateTimestamp = GameLogic.Timestamp;
        }

        /// <summary>
        /// Gets the "Interest Group" this player is in, based on it's position (in this demo).
        /// </summary>
        /// <returns>The group id this player is in.</returns>
        public byte GetGroup()
        {
            //ParticleRoom pr = this.RoomReference as ParticleRoom;
            //if (pr != null)
            //{
            //    return pr.GetGroup(this.PosX, this.PosY);
            //}

            return 0;
        }
    }
}
