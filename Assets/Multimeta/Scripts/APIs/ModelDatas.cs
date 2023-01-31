using Newtonsoft.Json.Linq;
using System;

public class ModelDatas
{
    [Serializable]
    public class SignInResData
    {
        public string refresh;
        public string access;
    }

    [Serializable]
    public class ListMetaVerseResData
    {
        public int count;
        public string next;
        public string previous;
        public MetaVerseData[] results;
    }

    [Serializable]
    public class MetaVerseData
    {
        public string id;
        public string name;
        public string model_3D;
        public string email;
        public bool is_login;
        public JToken meta_type;
        public string domain;
        public MetaVerseSetting meta_setting;
        public MediaConfig media_config;
        public string thumbnail;
        public string group;
    }

    [Serializable]
    public class MetaVerseSetting
    {
        public string id;
        public string created_at;
        public string updated_at;
        public JToken my_terrain;
        public JToken my_avatar;
        public string metaverse;
    }

    [Serializable]
    public class Terrain
    {
        public float x;
        public float y;
        public float z;
        public string type;
        public float scale;
        public string terrain_path;
    }

    [Serializable]
    public class Avatar
    {
        public float x;
        public float y;
        public float z;
    }

    [Serializable]
    public class MediaConfig
    {
        public string id;
        public string created_at;
        public string updated_at;
        public JToken audio;
        public JToken video;
        public JToken ebook;
        public string metaverse;
    }

    [Serializable]
    public class Audio
    {
        public string url;
    }

    [Serializable]
    public class Video
    {
        public float x;
        public float y;
        public float z;
        public float width;
        public float height;
        public JValue[] playlist;
    }

    [Serializable]
    public class PlayList
    {
        public string url;
        public string name;
    }

    [Serializable]
    public class Ebook
    {
        public string url;
    }
}
