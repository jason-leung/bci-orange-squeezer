using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
using System;

public class LSLHandler : MonoBehaviour
{
    public liblsl.StreamInfo streamInfo;
    public liblsl.StreamOutlet streamOutlet;

    void Start()
    {
        // create stream info and outlet
        streamInfo = new liblsl.StreamInfo("GameMarkers", "Markers", 1, 0, liblsl.channel_format_t.cf_string, "esibci_0000");
        streamOutlet = new liblsl.StreamOutlet(streamInfo);
    }

    public void SendLSLStringMarker(string marker)
    {
        string[] sample = new string[1];
        sample[0] = marker;
        streamOutlet.push_sample(sample, DateTime.Now.ToOADate());
    }
}
