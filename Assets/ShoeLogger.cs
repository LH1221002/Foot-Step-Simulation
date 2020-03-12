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


    IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        outFile = File.Create("D:\\LuisH\\HapticFeedbackProject\\Foot-Step-Simulation\\" + LogName +DateTime.Now.Hour+ LogName + DateTime.Now.Minute + DateTime.Now.Second+" "+(ToggleMapAndSetup.UseStaticVibration ? "Static" : "Dybamic") +".gz ");
        print(outFile);
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
        outFile.Close();
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
            Rotation = RightShoe.transform.rotation,
            Pressure = RightShoePressure
        };

        writer?.WriteLine(JsonUtility.ToJson(logl));
        writer?.Flush();
        writer?.WriteLine(JsonUtility.ToJson(logr));
        writer?.Flush();

    }

    void OnApplicationQuit()
    {
        Debug.Log("Hello");
        FinishLogfile();

    }

}
