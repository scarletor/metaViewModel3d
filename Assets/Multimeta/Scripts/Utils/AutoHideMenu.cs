using UnityEngine;

public class AutoHideMenu : MonoBehaviour
{
    void Update()
    {
        HideIfClickedOutside();
    }

    private void HideIfClickedOutside()
    {
        if (Input.GetMouseButton(0) && gameObject.activeSelf &&
            !RectTransformUtility.RectangleContainsScreenPoint(
                gameObject.GetComponent<RectTransform>(),
                Input.mousePosition,
                null))
        {
            gameObject.SetActive(false);
        }
    }
}
