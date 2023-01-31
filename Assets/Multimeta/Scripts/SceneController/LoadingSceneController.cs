using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    [SerializeField] Slider loadingSlider;
    [SerializeField] GameObject fillGroupItems;
    [SerializeField] GameObject errorPanel;

    private int fillItemNum = 34;

    IServices services => MultimetaService.Instance;

    async void Start()
    {
        BackgroundProgress.Instance.DisposeMetaVerseRoom();

        try
        {
            JObject jo = GameContext.SelectedMetaverseData.meta_setting.my_terrain.ToObject<JObject>();
            string modelType = jo["type"].Value<string>();
            string resourceUrl = "https:" + jo["terrain_path"].Value<string>();
            string modelName = resourceUrl.Split('/').Last();
            float scale = jo["scale"].Value<float>();
            JArray subTerrains = jo.ContainsKey("sub_terrains") ? jo["sub_terrains"].Value<JArray>() : null;

            Debug.Log("Model Name: " + modelName);

            if (!modelType.Contains("glb"))
                throw new NotSupportedException();

            var localPath = Path.Combine(Application.persistentDataPath, modelName);
            // Load model file from local path
            if (File.Exists(localPath))
            {
                SceneManager.LoadScene(SceneNameConfig.AVATAR_SCENE);
                await Task.Delay(1000);
                BackgroundProgress.Instance.LoadMetaverseRoomFromPath(localPath, scale);
                BackgroundProgress.Instance.LoadSubModelIfNeeded(subTerrains);
            }
            // Download model file
            else
            {
                services.DownloadFile(resourceUrl,
                    onProgress: (progress) =>
                    {
                        loadingSlider.value = progress;

                        int fillActiveNum = (int)(fillItemNum * progress);
                        for (int i = 0; i < fillActiveNum; i++)
                            fillGroupItems.transform.GetChild(i).gameObject.SetActive(true);
                    },
                    onSuccess: async (byteDatas) =>
                    {
                        CacheModelFile(byteDatas, modelName);
                        SceneManager.LoadScene(SceneNameConfig.AVATAR_SCENE);
                        await Task.Delay(1000);
                        BackgroundProgress.Instance.LoadMetaverseRoomFromBytes(byteDatas, scale);
                        BackgroundProgress.Instance.LoadSubModelIfNeeded(subTerrains);
                    },
                    onFailure: (error) =>
                    {
                        errorPanel.SetActive(true);
                    });
            }
        }
        catch
        {
            errorPanel.SetActive(true);
        }
    }

    private void CacheModelFile(byte[] bytes, string modelName)
    {
        var path = Path.Combine(Application.persistentDataPath, modelName);

        #region Using File System
        //if (!File.Exists(path))
        //{
        //    File.WriteAllBytes(path, bytes);
        //}
        #endregion

        #region Using File Stream
        if (!File.Exists(path))
        {
            FileStream fs = new FileStream(path, FileMode.CreateNew);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }
        #endregion

        Debug.Log("Cached model file path: " + path);
    }

    public void BackToHome()
    {
        SceneManager.LoadScene(SceneNameConfig.HOME_SCENE);
    }
}
