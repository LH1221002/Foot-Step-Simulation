using HapticShoes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

public class ShoeLogger : MonoBehaviour
{
    public GameObject LeftShoe;
    public GameObject RightShoe;


    public string LogName = "Haptic Shoes";



    private FileStream outFile;
    private GZipStream compress;
    private StreamWriter writer;


    void Awake()
    {
        outFile = File.Create(LogName + DateTime.Now.ToString() + ".gz");
        compress = new GZipStream(outFile, CompressionMode.Compress);
        writer = new StreamWriter(compress);

        LeftShoe.GetComponentInChildren<ShoeController>().ReceiveData(UpdatePressureLeft);
        RightShoe.GetComponentInChildren<ShoeController>().ReceiveData(UpdatePressureRight);
    }


    private int LeftShoePressure = 0;
    private int RightShoePressure = 0;

    private void UpdatePressureLeft(int raw, int cal)
    {
        LeftShoePressure = raw;
    }

    private void UpdatePressureRight(int raw, int cal)
    {
        RightShoePressure = raw;
    }


    private void FinishLogfile()
    {
        writer?.Close();
    }

    [Serializable]
    struct LogPart
    {
        public bool isLeft;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scaling;
        public int Pressure;
    }

    void FixedUpdate()
    {
        LogPart logl = new LogPart()
        {
            isLeft = true,
            Position = LeftShoe.transform.position,
            Rotation = LeftShoe.transform.rotation,
            Pressure = LeftShoePressure
        };

        LogPart logr = new LogPart()
        {
            isLeft = false,
            Position = RightShoe.transform.position,
            Rotation = LeftShoe.transform.rotation,
            Pressure = LeftShoePressure
        };

        writer?.WriteLine(JsonUtility.ToJson(logl));
        writer?.WriteLine(JsonUtility.ToJson(logr));

    }


    void OnApplicationExit()
    {
        FinishLogfile();

    }

}
