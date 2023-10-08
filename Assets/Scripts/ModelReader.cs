using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;

public class ModelReader : MonoBehaviour
{
    public Texture2D texture;
    public NNModel modelAsset;

    private Model runtimeModel;
    private IWorker engine;

    [Serializable]
    public struct Prediction
    {
        public int predictedValue;
        public float[] predicteds;

        public void SetPrediction(Tensor t)
        {
            predicteds = t.AsFloats();
            predictedValue = Array.IndexOf(predicteds, predicteds.Max());
            Debug.Log($"Predicted: {predictedValue}");
        }
    }

    public Prediction prediction;


    public void Start()
    {
        //runtimeModel = ModelLoader.Load(modelAsset);
        //engine = WorkerFactory.CreateWorker(runtimeModel, WorkerFactory.Device.GPU);
        //prediction = new Prediction();
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    Debug.Log("Predicting...");
        //    int channelCount = 1;
        //    var inputX = new Tensor(texture, channelCount);

        //    Tensor outputY = engine.Execute(inputX).PeekOutput();
        //    inputX.Dispose();
        //    prediction.SetPrediction(outputY);
        //}
    }

    private void OnDestroy()
    {
        engine?.Dispose();
    }
}
