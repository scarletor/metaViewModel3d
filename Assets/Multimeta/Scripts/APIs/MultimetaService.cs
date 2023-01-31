using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static ModelDatas;

public class MultimetaService : SingletonMonoBehaviorBase<MultimetaService>, IServices
{
    private bool isApiCall = false;
    private string accessToken = string.Empty;
    private string refreshToken = string.Empty;

    const string PARSE_JSON_FAILURE = "Can not parse json data.";

    public void SignIn(string email, string password, Action<SignInResData> onSuccess = null, Action<string> onFailure = null)
    {
        if (!isApiCall)
        {
            isApiCall = true;
            StartCoroutine(SignInImpl(email, password, onSuccess, onFailure));
        }
    }

    public void SignUp(string email, string username, string password, Action<string> onSuccess = null, Action<string> onFailure = null)
    {
        if (!isApiCall)
        {
            isApiCall = true;
            StartCoroutine(SignUpImpl(email, username, password, onSuccess, onFailure));
        }
    }

    public void GetListMetaVerses(string group = null, int page = 1, Action<ListMetaVerseResData> onSuccess = null, Action<string> onFailure = null)
    {
        if (!isApiCall)
        {
            isApiCall = true;
            StartCoroutine(GetListMetaVersesImpl(accessToken, group, page, onSuccess, onFailure));
        }
    }

    public void DownloadFile(string url, Action<float> onProgress = null, Action<byte[]> onSuccess = null, Action<string> onFailure = null)
    {
        StartCoroutine(DownloadFileImpl(url, onProgress, onSuccess, onFailure));
    }

    public void DownloadImage(string url, Action<Texture2D> onSuccess = null, Action<string> onFailure = null)
    {
        StartCoroutine(DownloadImageImpl(url, onSuccess, onFailure));
    }

    public void DownloadAudio(string url, Action<AudioClip> onSuccess = null, Action<string> onFailure = null)
    {
        StartCoroutine(DownloadAudioImpl(url, onSuccess, onFailure));
    }

    #region Core Implement

    IEnumerator SignInImpl(string email, string password, Action<SignInResData> onSuccess, Action<string> onFailure)
    {
        JObject reqJson = new JObject();
        reqJson.Add("email", email);
        reqJson.Add("password", password);
        byte[] bodyRawData = Encoding.UTF8.GetBytes(reqJson.ToString());

        var www = new UnityWebRequest(ServiceConfig.DOMAIN + ServiceConfig.SIGN_IN, "POST");
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRawData);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        isApiCall = false;
        string respBody = www.downloadHandler.text;

        Debug.Log("<color=red> UWRServiceImpl.SignIn </color> response = " + respBody);

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.DataProcessingError)
        {
            onFailure?.Invoke(respBody);
        }
        else
        {
            try
            {
                var responseData = JsonConvert.DeserializeObject<SignInResData>(respBody);
                accessToken = responseData.access;
                refreshToken = responseData.refresh;

                if (accessToken != null)
                    onSuccess?.Invoke(responseData);
                else
                    onFailure?.Invoke(respBody);
            }
            catch
            {
                onFailure?.Invoke(PARSE_JSON_FAILURE);
            }
        }
    }

    IEnumerator SignUpImpl(string email, string username, string password, Action<string> onSuccess, Action<string> onFailure)
    {
        JObject reqJson = new JObject();
        reqJson.Add("email", email);
        reqJson.Add("username", username);
        reqJson.Add("password", password);
        byte[] bodyRawData = Encoding.UTF8.GetBytes(reqJson.ToString());

        var www = new UnityWebRequest(ServiceConfig.DOMAIN + ServiceConfig.SIGN_UP, "POST");
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRawData);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        isApiCall = false;
        string respBody = www.downloadHandler.text;

        Debug.Log("<color=red> UWRServiceImpl.SignUpImpl </color> response = " + respBody);

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.DataProcessingError)
        {
            onFailure?.Invoke(respBody);
        }
        else
        {
            onSuccess?.Invoke(respBody);
        }
    }

    IEnumerator GetListMetaVersesImpl(string accessToken, string group, int page, Action<ListMetaVerseResData> onSuccess, Action<string> onFailure)
    {
        string url = ServiceConfig.DOMAIN + ServiceConfig.GET_LIST_METAVERSES;
        if (group != null)
            url += "?group=" + group + "&page=" + page;
        else
            url += "?page=" + page;

        var www = new UnityWebRequest(url, "GET");
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        isApiCall = false;
        string respBody = www.downloadHandler.text;

        Debug.Log("<color=red> UWRServiceImpl.GetListMetaVerses</color> response = " + respBody);

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.DataProcessingError)
        {
            onFailure?.Invoke(respBody);
        }
        else
        {
            try
            {
                var responseData = JsonConvert.DeserializeObject<ListMetaVerseResData>(respBody);
                onSuccess?.Invoke(responseData);
            }
            catch
            {
                onFailure?.Invoke(PARSE_JSON_FAILURE);
            }
        }
    }

    IEnumerator DownloadFileImpl(string url, Action<float> onProgress, Action<byte[]> onSuccess, Action<string> onFailure)
    {
        var www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        www.downloadHandler = new DownloadHandlerBuffer();

        var operation = www.SendWebRequest();
        while (!operation.isDone)
        {
            //Debug.Log("Progress: " + uwr.downloadProgress);
            onProgress?.Invoke(www.downloadProgress);
            yield return null;
        }

        //Debug.Log("<color=red> UWRServiceImpl.DownloadFileImpl</color> response = " + www.downloadHandler.text);

        if (www.result != UnityWebRequest.Result.Success)
        {
            onFailure?.Invoke(www.error);
        }
        else
        {
            byte[] datas = www.downloadHandler.data;
            onSuccess?.Invoke(datas);
        }
    }

    IEnumerator DownloadImageImpl(string url, Action<Texture2D> onSuccess, Action<string> onFailure)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        //Debug.Log("<color=red> UWRServiceImpl.DownloadImageImpl</color> response = " + www.downloadHandler.text);

        if (www.result != UnityWebRequest.Result.Success)
        {
            onFailure?.Invoke(www.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            onSuccess.Invoke(texture);
        }
    }

    IEnumerator DownloadAudioImpl(string url, Action<AudioClip> onSuccess, Action<string> onFailure)
    {
        string fileType = Path.GetExtension(url);
        Enum.TryParse(fileType.ToUpper(), out AudioType audioType);

        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, audioType);
        yield return www.SendWebRequest();

        //Debug.Log("<color=red> UWRServiceImpl.DownloadImageImpl</color> response = " + www.downloadHandler.text);

        if (www.result != UnityWebRequest.Result.Success)
        {
            onFailure?.Invoke(www.error);
        }
        else
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            onSuccess.Invoke(clip);
        }
    }

    #endregion
}
