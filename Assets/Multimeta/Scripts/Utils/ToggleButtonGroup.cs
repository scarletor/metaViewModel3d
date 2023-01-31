using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonGroup : MonoBehaviour
{
    [SerializeField] Button[] buttonGroup;

    [SerializeField] Color selectedColor;
    [SerializeField] Color deselectedColor;

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
            button.GetComponent<Image>().color = deselectedColor;

        buttonSelected.GetComponent<Image>().color = selectedColor;
    }
}
