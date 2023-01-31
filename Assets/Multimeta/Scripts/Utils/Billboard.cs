using UnityEngine;

public class Billboard : MonoBehaviour
{
    public float damping = 1;

    Transform mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main.transform;
    }

    private void Update()
    {
        var lookPos = mainCamera.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }
}
