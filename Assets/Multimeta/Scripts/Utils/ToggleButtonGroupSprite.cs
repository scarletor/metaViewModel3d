using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonGroupSprite : MonoBehaviour
{
    [SerializeField] Button[] buttonGroup;

    [SerializeField] Sprite selectedSprite;
    [SerializeField] Sprite deselectedSprite;
    [SerializeField] Color selectedFontColor;
    [SerializeField] Color deselectedFontColor;

    void Start()
    {
        foreach (var button in buttonGroup)
            button.onClick.AddListener(() => { OnButtonSelected(button); });

        SetStartButtonSelected();
    }

    void SetStartButtonSelected()
    {
        OnButtonSelected(buttonGroup[0]);
    }

    void OnButtonSelected(Button buttonSelected)
    {
        foreach (var button in buttonGroup)
        {
            button.GetComponent<Image>().sprite = deselectedSprite;
            button.GetComponentInChildren<TextMeshProUGUI>().color = deselectedFontColor;
        }

        buttonSelected.GetComponent<Image>().sprite = selectedSprite;
        buttonSelected.GetComponentInChildren<TextMeshProUGUI>().color = selectedFontColor;
    }
}
