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
    public float submitWaitTime = 2f;
   
    private PyObject pythonScript;
    private string submitedText = "";
    private float idleTime = 0f;


    public void Init()
    {
        Debug.Log("Khởi tạo Model dịch...");

        pythonScript = Py.Import("languageToSign");

        inputField.onValueChanged.RemoveListener(OnEdit);
        inputField.onValueChanged.AddListener(OnEdit);

        inputField.onSubmit.RemoveListener(OnSubmit);
        inputField.onSubmit.AddListener(OnSubmit);
    }

    private void OnSubmit(string text)
    {
        StartTranslate();
    }

    private void OnEdit(string text)
    {
        idleTime = 0f;
    }

    private void OnDisable()
    {
        inputField.text = "";
        submitedText = "";
        animationManager.ResetAnimation();
    }

    private void Update()
    {
        if(inputField.text != submitedText)
            idleTime += Time.deltaTime;

        if(idleTime >= submitWaitTime)
        {
            idleTime = 0f;
            Debug.Log("Submit...");
            StartTranslate();
        }
    }

    private void OnDestroy()
    {
        inputField.onSubmit.RemoveListener(OnSubmit);
        inputField.onValueChanged.RemoveListener(OnEdit);
    }


    [Button]
    public void StartTranslate()
    {
        Debug.Log("Running Python...");
        Debug.Log(inputField.text);

        submitedText = inputField.text;
        if (inputField.text == "") return;

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
        animationManager.ResetAnimation();
        animationManager.PlaySequenceAnimation(clipSequence, () =>
        {
            submitedText = "";
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
