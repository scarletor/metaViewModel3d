using System;
using UnityEngine;
using static ModelDatas;

public interface IServices 
{
    //-----User-----
    void SignIn(string email, string password, Action<SignInResData> onSuccess = null, Action<string> onFailure = null);
    void SignUp(string email, string username, string password, Action<string> onSuccess = null, Action<string> onFailure = null);

    //-----Metaverse-----
    void GetListMetaVerses(string group = null, int page = 1, Action<ListMetaVerseResData> onSuccess = null, Action<string> onFailure = null);

    //-----File-----
    void DownloadFile(string url, Action<float> onProgress = null, Action<byte[]> onSuccess = null, Action<string> onFailure = null);
    void DownloadImage(string url, Action<Texture2D> onSuccess = null, Action<string> onFailure = null);
    void DownloadAudio(string url, Action<AudioClip> onSuccess = null, Action<string> onFailure = null);
}
