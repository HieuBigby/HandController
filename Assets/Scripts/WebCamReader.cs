using NaughtyAttributes;
using Python.Runtime;
using PythonEvent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;

public class WebCamReader : MonoBehaviour
{
    public RawImage img;
    public TextMeshProUGUI detectText;

    private WebCamTexture webCam;
    private bool isDetecting;
    private string handDetectPath = "Assets/Pythons/python-3.8.0-embed/ActionDetect";
    private string imageFilePath = "Assets/Pictures/tex.jpg";
    private string[] actions = {"A", "B", "C", "D", "E"};

    private dynamic detectScript;
    private dynamic cv2;
    private dynamic cap;
    private dynamic mp;
    private dynamic holistic;
    private dynamic model;

    public bool Initialized;


    public void Init()
    {
        Debug.Log("Khởi tạo WebCam");
        webCam = new WebCamTexture(640, 480);
        img.texture = webCam;

        // Import thư viện 
        detectScript = Py.Import("actionDetect");
        cv2 = Py.Import("cv2");
        mp = Py.Import("mediapipe");

        // Khởi tạo mediapipe
        dynamic mp_holistic = mp.solutions.holistic;
        holistic = mp_holistic.Holistic(min_detection_confidence: 0.5, min_tracking_confidence: 0.5);

        // Khởi tạo model phát hiện thủ ngữ 
        string modelPath = $"{handDetectPath}/action_test_2.h5";
        model = detectScript.load_model(modelPath, actions.Length);

        Initialized = true;
    }

    private void OnEnable()
    {
        if (!Initialized) return;

        ShowWebCam();
        StartDetect();
    }

    private void OnDisable()
    {
        webCam.Stop();
        StopDetect();
    }

    public void ShowWebCam()
    {
        if(webCam.isPlaying == false) webCam.Play();
    }

    public void StartDetect()
    {
        isDetecting = true;
        StartCoroutine(DetectRoutine());
    }

    // Logic phát hiện thủ ngữ
    public IEnumerator DetectRoutine()
    {
        PyList sequences = new PyList();

        while (isDetecting)
        {
            // Lưu ảnh vào thư mục để đọc ảnh bằng cv2 
            Texture2D rawImageTexture = GetTexture2D(img.mainTexture);
            byte[] imageBytes = rawImageTexture.EncodeToJPG();
            System.IO.File.WriteAllBytes(imageFilePath, imageBytes);
            yield return null;
            dynamic readImg = cv2.imread(imageFilePath);
            yield return null;

            // Lấy thông tin về các điểm keypoints
            dynamic results = holistic.process(cv2.cvtColor(readImg, cv2.COLOR_BGR2RGB));
            yield return null;
            dynamic keypoints = detectScript.extract_keypoints(results);
            //Debug.Log(keypoints);

            // Nếu đủ 10 batch keypoint thì đưa vào mô hình để phát hiện
            sequences.Append(keypoints);
            if (sequences.Length() == 10)
            {
                // Thực hiện phát hiện  
                dynamic detected = detectScript.detect(sequences, model);
                Debug.Log(detected);

                // Hiển thị hành động phát hiện được
                if ((object)detected != null) detectText.text = actions[(int)detected];

                sequences.DelItem(0);
            }
        }
    }

    public void StopDetect()
    {
        isDetecting = false;
        detectText.text = "";
    }

    private Texture2D GetTexture2D(Texture texture)
    {
        Texture2D dest = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

        Graphics.CopyTexture(texture, dest);

        return dest;
    }
}
