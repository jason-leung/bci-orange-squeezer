using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileWriter : MonoBehaviour
{
    // file paths
    string gameMarkerFilePath;
    string emgFilePath;
    string eegFilePath;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize File Path
        gameMarkerFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "esibci_markers_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".txt");
        emgFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "esibci_emg_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".txt");
        eegFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "esibci_eeg_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".txt");

        // Create directory if it doesn't exist
        Directory.CreateDirectory(Path.GetDirectoryName(gameMarkerFilePath));
        Directory.CreateDirectory(Path.GetDirectoryName(emgFilePath));
        Directory.CreateDirectory(Path.GetDirectoryName(eegFilePath));

        // Write Headers
        using (StreamWriter gameMarkerWriter = File.AppendText(gameMarkerFilePath))
        {
            gameMarkerWriter.WriteLine("Time,Marker");
        }
        using (StreamWriter emgWriter = File.AppendText(emgFilePath))
        {
            string header = "Time,";
            for (int i = 0; i < 2; i++) header += i.ToString() + ",";
            emgWriter.WriteLine(header);
        }
        using (StreamWriter eegWriter = File.AppendText(eegFilePath))
        {
            string header = "Time,";
            for (int i = 0; i < 64; i++) header += i.ToString() + ",";
            eegWriter.WriteLine(header);
        }
    }

    public void WriteGameMarker(double timestamp, string marker)
    {
        using (StreamWriter gameMarkerWriter = File.AppendText(gameMarkerFilePath))
        {
            gameMarkerWriter.WriteLine(timestamp + "," + marker);
        }
    }

    public void WriteEMG(double timestamp, float[] data)
    {
        using (StreamWriter emgWriter = File.AppendText(emgFilePath))
        {
            string sample = "";
            for (int i = 0; i < data.Length; i++) sample += "," + data[i];
            emgWriter.WriteLine(timestamp + sample);
        }
    }

    public void WriteEEG(double timestamp, float[] data)
    {
        using (StreamWriter eegWriter = File.AppendText(eegFilePath))
        {
            string sample = "";
            for (int i = 0; i < data.Length; i++) sample += "," + data[i];
            eegWriter.WriteLine(timestamp + sample);
        }
    }
}
