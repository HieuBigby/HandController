using NaughtyAttributes;
using Python.Runtime;
using PythonEvent;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;

public class WebCamReader : MonoBehaviour
{
    public RawImage img;

    private WebCamTexture webCam;
    private bool isDetecting;
    private string handDetectPath = "Assets/Pythons/python-3.8.0-embed/ActionDetect";
    private string imageFilePath = "Assets/Pictures/tex.jpg";

    private string[] actions = {"A", "B", "C", "D", "E"};

    private dynamic detectScript;
    private dynamic cv2;
    private dynamic np;
    private dynamic cap;
    private dynamic mp;
    private dynamic holistic;
    private dynamic model;

    private PyList sequences;
    private Thread videoThread;
    private bool isRunning = false;

    public NNModel modelAsset;

    private Model runtimeModel;
    private IWorker engine;


    private void Start()
    {
        EventHandler.PrintCallback -= PrintFromWebCam;
        EventHandler.PrintCallback += PrintFromWebCam;

        //runtimeModel = ModelLoader.Load(modelAsset);
        //engine = WorkerFactory.CreateWorker(runtimeModel, WorkerFactory.Device.GPU);
    }

    public void PrintFromWebCam()
    {
        Debug.Log("Hello");
    }

    [Button]
    public void ShowWebCam()
    {
        webCam = new WebCamTexture();
        if(webCam.isPlaying == false ) webCam.Play();
        img.texture = webCam;
    }

    [Button]
    public void StartDetect()
    {
        Runtime.PythonDLL = "E:/Hieu/UnityProjects/TestHand/Assets/Pythons/python-3.8.0-embed/python38.dll";

        // Initialize Python.NET
        PythonEngine.Initialize();

        detectScript = Py.Import("actionDetect");
        cv2 = Py.Import("cv2");
        mp = Py.Import("mediapipe");

        dynamic mp_holistic = mp.solutions.holistic;
        holistic = mp_holistic.Holistic();

        sequences = new PyList();

        string modelPath = handDetectPath + "/" + "action_test_2.h5";

        model = detectScript.load_model(modelPath, (10, 126), 5);
        //np = Py.Import("numpy");
        //cap = cv2.VideoCapture(0);

        //Thread videoThread = new Thread(StartVideo);
        //videoThread.Start();
        //StartVideo();

        isDetecting = true;

        //StartCoroutine(StartVideoCapture());

        //string modelPath = handDetectPath + "/" + "action_test_2.h5";
        //dynamic my_module = Py.Import("actionDetect");
        //my_module.begin_detect(modelPath);
    }

    public void StartVideo()
    {
        //Debug.Log("Thread start...");
        //using (Py.GIL())
        //{
        //    intPtr = PythonEngine.BeginAllowThreads();
        //    Debug.Log(intPtr.ToString());   
        //    detectScript.video_capture();
        //}
    }

    private IEnumerator StartVideoCapture()
    {
        Debug.Log("Starting...");
        isRunning = true;
        string modelPath = handDetectPath + "/" + "action_test_2.h5";
        dynamic my_module = Py.Import("actionDetect");
        my_module.begin_detect(modelPath);
        yield return null;
    }

    [Button]
    public void StopDetect()
    {
        isDetecting = false;
        //PythonEngine.EndAllowThreads(intPtr);

        isRunning = false; // Signal the video thread to stop
        //videoThread.Join(); // Wait for the video thread to finish
        PythonEngine.Shutdown();
    }

    private void Update()
    {
        if(isDetecting)
        {
            //detectScript.video_capture();

            Texture2D rawImageTexture = GetTexture2D(img.mainTexture);
            byte[] imageBytes = rawImageTexture.EncodeToJPG();
            System.IO.File.WriteAllBytes(imageFilePath, imageBytes);

            dynamic readImg = cv2.imread(imageFilePath);
            //PyString pyFilePath = new PyString(imageFilePath);
            //PyObject keypoints = detectScript.InvokeMethod("get_keypoints_path", pyFilePath);


            dynamic results = holistic.process(readImg);
            dynamic keypoints = detectScript.extract_keypoints(results);
           
            sequences.Append(keypoints);

            if(sequences.Length() == 10)
            {
                // Detect 
                dynamic detected = detectScript.detect(sequences, model);
                Debug.Log(detected);

                sequences.DelItem(0);
            }
            //Debug.Log(sequences.Length());


            //dynamic keypoints = detectScript.get_keypoints(readImg);

            //Debug.Log(keypoints);

            //PyList image = TextureToIntegerList(rawImageTexture);
            //dynamic image_array = np.array(image);
            //cv2.imshow("Test", image_array);
            //int channelCount = 3;
            //var inputX = new Tensor(img.texture, channelCount);

            //using (Py.GIL()) // Acquire the Global Interpreter Lock
            //{
            //    // Convert the Unity Texture2D to a byte array
            //    Texture2D rawImageTexture = GetTextureFromRawImage(img);
            //    byte[] bytes = rawImageTexture.GetRawTextureData();

            //    // Create a NumPy array from the byte array
            //    PyObject npArray = cv2.imdecode(bytes, cv2.IMREAD_COLOR);
            //    dynamic keypoints = detectScript.get_keypoints(npArray);
            //    Debug.Log(keypoints);
            //    //detectScript.InvokeMethod("get_keypoints", npArray);
            //}
        }
    }

    private PyList TextureToIntegerList(Texture2D texture)
    {
        PyList pixelValues = new PyList();
        Color32[] pixels = texture.GetPixels32();

        foreach (Color32 pixel in pixels)
        {
            int value = (pixel.r << 16) | (pixel.g << 8) | pixel.b;
            pixelValues.Append(value.ToPython());
        }

        return pixelValues;
    }

    private Texture2D GetTexture2D(Texture texture)
    {
        Texture2D dest = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

        Graphics.CopyTexture(texture, dest);

        return dest;
    }
}
