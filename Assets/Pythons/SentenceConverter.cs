using NaughtyAttributes;
using Python.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

[Serializable]
public class AnimDict
{
    public string word;
    public AnimationClip clip;
}

public class SentenceConverter : MonoBehaviour
{
    public List<AnimDict> animDicts = new List<AnimDict>();
    public HandAnimationManager animationManager;
    public TMP_InputField inputField; 
    public Animation inputAnimation;

    private string fileName = "languageToSign";
    private string pythonPath = "Assets/Pythons/python-3.8.0-embed/python38.dll";
    private readonly string handDetectPath = "Assets/Pythons/python-3.8.0-embed/ActionDetect";
    private PyObject pythonScript;



    private void Start()
    {
        Runtime.PythonDLL = pythonPath;

        PythonEngine.Initialize();

        dynamic sys = Py.Import("sys");
        //sys.path.remove("Assets/Pythons/python-3.8.0-embed/Lib/site-packages/cv2");
        sys.path.append(handDetectPath);
        Debug.Log(sys.path);

        pythonScript = Py.Import(fileName);
    }

    private void OnDestroy()
    {
        if (PythonEngine.IsInitialized)
        {
            PythonEngine.Shutdown();
        }
    }


    [Button]
    public void RunPythonFile()
    {
        Debug.Log("Running Python...");
        Debug.Log(inputField.text);

        var parameter = new PyString(inputField.text);
        //var parameter = new PyString("Xin chào, tôi tên là Hiếu");

        PyDict replacements = new PyDict();
        replacements["Tôi Tên"] = new PyString("tôi tên là");
        replacements["Vui Gặp"] = new PyString("Rất vui được gặp bạn");
        replacements["E^/"] = new PyString("Ế");

        PyList vocab_dict = new PyList();
        vocab_dict.Append(new PyString("Xin chào"));
        vocab_dict.Append(new PyString("Tôi"));
        vocab_dict.Append(new PyString("Tên"));
        vocab_dict.Append(new PyString("H"));
        vocab_dict.Append(new PyString("I"));
        vocab_dict.Append(new PyString("Ế"));
        vocab_dict.Append(new PyString("U"));
        vocab_dict.Append(new PyString("Em"));

        // Lấy kết quả và hiển thị lên màn hình  
        var result = pythonScript.InvokeMethod("language_to_sign", new PyObject[] { parameter, replacements, vocab_dict });
        Debug.Log(result);

        AnimationClip[] clipSequence = CreateAnimationSequence(result);
        //inputAnimation.Play("InputHide");
        animationManager.PlaySequenceAnimation(clipSequence, () =>
        {
            //inputAnimation.Play("InputShow");
        });
    }

    public AnimationClip[] CreateAnimationSequence(PyObject result)
    {
        PyIter pyIter = PyIter.GetIter(result); // Replace yourPythonObject with your Python iterable
        List<AnimationClip> animations = new List<AnimationClip>();   

        while (pyIter.MoveNext())
        {
            PyObject pyObject = pyIter.Current;
            string word = pyObject.ToString();
            AnimDict foundAnimDict = animDicts.FirstOrDefault(animDict => animDict.word.Equals(word));
            if(foundAnimDict != null)
            {
                animations.Add(foundAnimDict.clip);
            }
            else
            {
                Debug.LogError("Không tìm thấy " + word);
            }
        }

        return animations.ToArray();
    }
}
