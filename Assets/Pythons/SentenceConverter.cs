using NaughtyAttributes;
using Python.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    private string filePath = "Assets/Pythons/python-3.8.0-embed/languageToSign.py";
    private string fileName = "languageToSign";
    private string pythonPath = "Assets/Pythons/python-3.8.0-embed/python38.dll";
    private PyObject pythonScript;


    private void Start()
    {
        Runtime.PythonDLL = pythonPath;

        // Initialize Python.NET
        PythonEngine.Initialize();

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
        //using (Py.GIL()) // Acquire the Global Interpreter Lock
        //{
        Debug.Log("Running Python...");
        Debug.Log(inputField.text);
        var parameter = new PyString(inputField.text);

        PyDict replacements = new PyDict();
        replacements["Tôi Tên"] = new PyString("Tôi tên là");
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

        var result = pythonScript.InvokeMethod("language_to_sign", new PyObject[] { parameter, replacements, vocab_dict });
        Debug.Log(result);
        AnimationClip[] clipSequence = CreateAnimationSequence(result);
        animationManager.PlaySequenceAnimation(clipSequence);
        //}
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

    //[Button]
    //public void RunPythonFile()
    //{
    //    if(File.Exists(filePath))
    //    {
    //        Debug.Log("Đang đọc file...");
    //        try
    //        {
    //            // Read the Python file as a string
    //            string pythonScriptText = File.ReadAllText(filePath);

    //            // Now, you have the Python script as a string (pythonScript)
    //            Debug.Log("Python Script Contents:\n" + pythonScriptText);

    //            Runtime.PythonDLL = pythonPath;

    //            // Initialize Python.NET
    //            PythonEngine.Initialize();

    //            using (Py.GIL()) // Acquire the Global Interpreter Lock
    //            {
    //                Debug.Log("Running Python...");
    //                PyObject pythonScript = Py.Import(fileName);
    //                //var parameter = new PyString("Xin chao, Toi ten la Hieu");
    //                var parameter = new PyString("Xin chào, tôi tên là Hiếu");

    //                PyDict replacements = new PyDict();
    //                replacements["Toi ten"] = new PyString("Toi ten la");
    //                replacements["Vui Gap"] = new PyString("Rat vui duoc gap ban");
    //                replacements["E^/"] = new PyString("E");
                    
    //                PyList vocab_dict = new PyList();
    //                vocab_dict.Append(new PyString("Xin chao"));
    //                vocab_dict.Append(new PyString("Toi"));
    //                vocab_dict.Append(new PyString("Ten"));
    //                vocab_dict.Append(new PyString("H"));
    //                vocab_dict.Append(new PyString("I"));
    //                vocab_dict.Append(new PyString("E"));
    //                vocab_dict.Append(new PyString("U"));
    //                vocab_dict.Append(new PyString("Em"));

    //                var result = pythonScript.InvokeMethod("language_to_sign", new PyObject[] { parameter, replacements, vocab_dict });
    //                //var result = pythonScript.InvokeMethod("test");
    //                Debug.Log(result);

    //                //PyObject pyObject = null;
    //                //PythonEngine.Exec(pythonScriptText, null, pyObject);
    //                //var parameter = new PyString("34");
    //                //PyObject pyResult = pyObject.InvokeMethod("is_number", new PyObject[] { parameter });
    //                //Debug.Log(pyResult);


    //                //PyObject pyObject = PythonEngine.Compile(filePath);
    //                //var parameter = new PyString("34");
    //                //PyObject pyResult = pyObject.InvokeMethod("is_number", new PyObject[] { parameter });
    //                //Debug.Log(pyResult);

    //                //dynamic scope = Py.CreateScope(); // Create a Python scope

    //                //try
    //                //{
    //                //    PythonEngine.Exec(pythonScriptText, scope); // Replace with the path to your Python script

    //                //    // Get a reference to the Python function
    //                //    dynamic isNumberFunction = scope.Get("is_number");

    //                //    // Call the Python function with arguments
    //                //    dynamic result = isNumberFunction("Hello");

    //                //    print(result);
    //                //}
    //                //catch (Exception e)
    //                //{
    //                //    Debug.LogError("Error executing Python script: " + e);
    //                //}
    //            }

    //            PythonEngine.Shutdown();
    //        }
    //        catch (IOException e)
    //        {
    //            Debug.LogError("Error reading Python file: " + e.Message);
    //        }

    //        //PythonRunner.RunFile(filePath);


    //        //try
    //        //{
    //        //    np = PyModule.Import("numpy");
    //        //    print("pi: " + np.pi);
    //        //}
    //        //catch (Exception e)
    //        //{
    //        //    print(e);
    //        //    print(e.StackTrace);
    //        //}
    //    }
    //}
}
