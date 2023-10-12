using Python.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DetectMode
{
    TextToSign,
    SignToText
}

public class AppManager : MonoBehaviour
{
    public SentenceConverter sentenceConverter;
    public WebCamReader webCamReader;
    public Button switchBtn;
    public TextMeshProUGUI modeTxt1, modeTxt2;

    private string pythonPath = "Assets/Pythons/python-3.8.0-embed/python38.dll";
    private readonly string handDetectPath = "Assets/Pythons/python-3.8.0-embed/ActionDetect";
    private DetectMode currentMode;


    private void Awake()
    {
        Runtime.PythonDLL = pythonPath;

        PythonEngine.Initialize();

        dynamic sys = Py.Import("sys");
        //sys.path.remove("Assets/Pythons/python-3.8.0-embed/Lib/site-packages/cv2");
        sys.path.append(handDetectPath);
        Debug.Log(sys.path);
    }

    private void Start()
    {
        sentenceConverter.Init();
        webCamReader.Init();
        SwitchMode(DetectMode.TextToSign);

        switchBtn.onClick.AddListener(ToggleMode);
    }

    private void OnDestroy()
    {
        if (PythonEngine.IsInitialized)
        {
            PythonEngine.Shutdown();
        }
        switchBtn.onClick.RemoveListener(ToggleMode);
    }

    public void ToggleMode()
    {
        SwitchMode(currentMode == DetectMode.TextToSign ? DetectMode.SignToText : DetectMode.TextToSign);
    }

    public void SwitchMode(DetectMode mode)
    {
        currentMode = mode;
        switch(mode)
        {
            case DetectMode.TextToSign:
                modeTxt1.text = "Văn bản";
                modeTxt2.text = "Cử chỉ";
                sentenceConverter.gameObject.SetActive(true);
                webCamReader.gameObject.SetActive(false);
                break;
            case DetectMode.SignToText:
                modeTxt1.text = "Cử chỉ";
                modeTxt2.text = "Văn bản";
                sentenceConverter.gameObject.SetActive(false);
                webCamReader.gameObject.SetActive(true);
                break;
        }
    }
}
