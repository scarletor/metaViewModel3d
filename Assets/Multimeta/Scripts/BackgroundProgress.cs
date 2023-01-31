using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Piglet;
using Siccity.GLTFUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TriLibCore;
using UnityEngine;

public class BackgroundProgress : SingletonMonoBehaviorBase<BackgroundProgress>
{
    public GameObject MetaVerseRoom { get; private set; }

    public event Action OnLoadMetaverseRoomCompleted;

    private GltfImportTask _task;
    private bool onLoad;
    private Queue<string> subModelQueue;

    IServices services => MultimetaService.Instance;

    void Update()
    {
        // Advance execution of glTF import task
        if (_task != null)
            _task.MoveNext();
    }

    public void DisposeMetaVerseRoom()
    {
        if (MetaVerseRoom != null)
        {
            Destroy(MetaVerseRoom);
            MetaVerseRoom = null;
        }
    }

    public void LoadMetaverseRoomFromBytes(byte[] data, float scale)
    {
        DisposeMetaVerseRoom();
        onLoad = true;
        PigletLoadModel(data, scale);
    }

    public void LoadMetaverseRoomFromPath(string path, float scale)
    {
#if UNITY_EDITOR
        path = "file://" + path;
#elif UNITY_ANDROID
        path = "jar:file://" + path;
#elif UNITY_IOS
        path = "file://" + path;
#endif
        DisposeMetaVerseRoom();
        onLoad = true;
        PigletLoadModel(path, scale);
    }

    public async void LoadSubModelIfNeeded(JArray subTerrainModels)
    {
        await UniTask.WaitUntil(() => onLoad == false);

        if (subTerrainModels == null || subTerrainModels.Count == 0)
            return;

        subModelQueue = new Queue<string>();

        foreach (JObject subModelData in subTerrainModels)
        {
            //Debug.Log(subModelData);
            subModelQueue.Enqueue(subModelData["path"].Value<string>());
        }

        StartLoadSubModelQueue();
    }

    #region Piglet

    private void PigletLoadModel(byte[] data, float scale)
    {
        _task = RuntimeGltfImporter.GetImportTask(data);
        //_task.OnProgress = OnProgress;
        _task.OnCompleted = (gameObject) => { OnLoadModelCompleted(gameObject, scale); };
    }

    private void PigletLoadModel(string path, float scale)
    {
        Uri uri = new Uri(path);
        _task = RuntimeGltfImporter.GetImportTask(uri);
        //_task.OnProgress = OnProgress;
        _task.OnCompleted = (gameObject) => { OnLoadModelCompleted(gameObject, scale); };
    }

    private async UniTask PigletLoadModelAsync(byte[] data)
    {
        onLoad = true;
        _task = RuntimeGltfImporter.GetImportTask(data);
        _task.OnCompleted = (gameObject) => 
        { 
            gameObject.transform.SetParent(MetaVerseRoom.transform, false);
            onLoad = false;
        };
        await UniTask.WaitUntil(() => onLoad == false);
    }

    #endregion

    #region GLTFUtility

    private void GLTFUtilityLoadModel(byte[] data, float scale)
    {
        Importer.ImportGLBAsync(data, new ImportSettings(),
            onFinished: (gameobject) =>
            {
                MetaVerseRoom = gameobject;
                MetaVerseRoom.SetActive(false);
                MetaVerseRoom.transform.SetParent(this.transform, false);
                MetaVerseRoom.transform.localScale = new Vector3(scale, scale, scale);
                AddMeshCollider(MetaVerseRoom);
                OnLoadMetaverseRoomCompleted?.Invoke();
            });
    } 

    #endregion

    #region Trilib

    private void TrilibLoadModel(byte[] data, float scale)
    {
        TrilibLoadModelFromBytes(data,
            onModelLoaded: (gameObject) =>
            {
                MetaVerseRoom = gameObject;
                MetaVerseRoom.SetActive(false);
                MetaVerseRoom.transform.SetParent(this.transform, false);
                MetaVerseRoom.transform.localScale = new Vector3(scale, scale, scale);
                AddMeshCollider(MetaVerseRoom);
                OnLoadMetaverseRoomCompleted?.Invoke();
            }, new CancellationTokenSource());
    }

    private void TrilibLoadModelFromBytes(byte[] bytes, Action<GameObject> onModelLoaded, CancellationTokenSource cancellationTokenSource)
    {
        Stream stream = new MemoryStream(bytes);

        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
        assetLoaderOptions.AddAssetUnloader = false;

        var assetLoaderContext = AssetLoader.LoadModelFromStream
        (
            stream: stream,
            filename: null,
            fileExtension: "glb",
            onLoad: (context) =>
            {
                context.RootGameObject.SetActive(false);
            },
            onMaterialsLoad: (context) =>
            {
                context.RootGameObject.SetActive(true);
                onModelLoaded.Invoke(context.RootGameObject);
            },
            onProgress: (context, progress) => { },
            onError: null,
            wrapperGameObject: null,
            assetLoaderOptions: assetLoaderOptions
        );
        assetLoaderContext.CancellationToken = cancellationTokenSource.Token;
    } 

    #endregion

    private void StartLoadSubModelQueue()
    {
        if (subModelQueue.Count == 0)
        {
            Debug.Log("Load sub model queue completed!");
            return;
        }

        string resourceUrl = "https:" + subModelQueue.Dequeue();
        Debug.Log("Start load sub model: " + resourceUrl);

        services.DownloadFile(resourceUrl,
            onSuccess: async (byteDatas) =>
            {
                await PigletLoadModelAsync(byteDatas);
                StartLoadSubModelQueue();
            },
            onFailure: (error) =>
            {
                Debug.LogError("Error to download model: " + resourceUrl);
                StartLoadSubModelQueue();
            });
    }

    private void OnLoadModelCompleted(GameObject gameObject, float defaultScale)
    {
        MetaVerseRoom = gameObject;
        MetaVerseRoom.SetActive(false);
        MetaVerseRoom.transform.SetParent(this.transform, false);
        MetaVerseRoom.transform.localScale = new Vector3(defaultScale, defaultScale, defaultScale);
        AddMeshCollider(MetaVerseRoom);

        OnLoadMetaverseRoomCompleted?.Invoke();
        onLoad = false;
    }

    private void AddMeshCollider(GameObject gameObject)
    {
        if (gameObject.GetComponent<MeshRenderer>())
            gameObject.AddComponent<MeshCollider>();

        foreach (Transform child in gameObject.transform)
            AddMeshCollider(child.gameObject);
    }
}
