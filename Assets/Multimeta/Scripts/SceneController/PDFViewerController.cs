using Paroxe.PdfRenderer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PDFViewerController : MonoBehaviour
{
    [SerializeField] Button exitButton;
    [SerializeField] PDFViewer pdfViewer;

    void Start()
    {
        pdfViewer.FileURL = GameContext.EbookUrl;
        exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnExitButtonClick()
    {
        SceneManager.UnloadSceneAsync(SceneNameConfig.PDFVIEWER_SCENE);
    }
}
