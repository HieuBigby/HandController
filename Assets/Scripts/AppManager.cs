using Python.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    private string[] actions;
    private Dictionary<string, string> structureDict;
    private DetectMode currentMode;

    public static AppManager Instance { get; private set; }
    public string[] ActionList => actions;
    public PyList PyListActions
    {
        get
        {
            PyList pyList = new PyList();
            foreach (var action in actions)
            {
                //Debug.Log(action);
                pyList.Append(new PyString(action));
            }
            return pyList;
        }
    }
    public PyDict PyDictStructure
    {
        get
        {
            PyDict pyDict = new PyDict();

            foreach(var item in structureDict)
            {
                //Debug.Log($"{item.Key}:{item.Value}");
                pyDict[item.Key] = new PyString(item.Value);
            }
            return pyDict;
        }
    }

    private void ReadActions(string txtPath)
    {
        string[] lines = File.ReadAllLines(txtPath);

        actions = new string[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            string[] keyValue = lines[i].Split(':');
            string key = keyValue[0].Trim();
            actions[i] = key;
        }
    }

    private void ReadStructures(string txtPath)
    {
        string[] lines = File.ReadAllLines(txtPath);
        structureDict = new Dictionary<string, string>();

        foreach (string line in lines)
        {
            string[] keyValue = line.Split(':');

            string key = keyValue[0].Trim();
            string value = keyValue[1].Trim();

            structureDict.Add(key, value);
        }
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Runtime.PythonDLL = pythonPath;
        PythonEngine.Initialize();

        dynamic sys = Py.Import("sys");
        //sys.path.remove("Assets/Pythons/python-3.8.0-embed/Lib/site-packages/cv2");
        sys.path.append(handDetectPath);

        string actionPath = $"{handDetectPath}/dictionary.txt";
        ReadActions(actionPath);

        string structurePath = $"{handDetectPath}/structure.txt";
        ReadStructures(structurePath);
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
