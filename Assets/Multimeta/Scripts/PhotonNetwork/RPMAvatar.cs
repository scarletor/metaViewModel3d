using UnityEngine;

public class RPMAvatar : MonoBehaviour
{
    public string url;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
