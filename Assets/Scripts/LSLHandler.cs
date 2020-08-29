using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
using System;

public class LSLHandler : MonoBehaviour
{
    public liblsl.StreamInfo markersStreamInfo;
    public liblsl.StreamOutlet markersStreamOutlet;

    /*
    public enum UpdateMoment { FixedUpdate, Update }
    public UpdateMoment moment;
    public liblsl.StreamInfo[] emgStreamInfo;
    public liblsl.StreamInlet emgStreamInlet;
    public liblsl.ContinuousResolver emgStreamResolver;
    public int emgExpectedChannels = 2;
    public string emgStreamName = "EMGData";
    public string emgStreamType = "EMG";
    float[] emgSample;
    */

    void Start()
    {
        CreateMarkersStreamOutlet();
        // CreateEMGStreamInlet();
    }

    public void CreateMarkersStreamOutlet()
    {
        markersStreamInfo = new liblsl.StreamInfo("GameMarkers", "Markers", 1, 0, liblsl.channel_format_t.cf_string, "esibci_0000");
        markersStreamOutlet = new liblsl.StreamOutlet(markersStreamInfo);
        Debug.Log("Created Markers LSL Stream Outlet");
    }

    public void Send_LSL_StringMarker(string marker)
    {
        string[] sample = new string[1];
        sample[0] = marker;
        markersStreamOutlet.push_sample(sample, DateTime.Now.ToOADate());
    }

    /*
    public void CreateEMGStreamInlet()
    {
        Debug.Log("Creating LSL resolver for stream with type: EMG ");
        emgStreamResolver = new liblsl.ContinuousResolver("type", emgStreamType);
        StartCoroutine(ResolveEMGStream());
    }

    IEnumerator ResolveEMGStream()
    {
        var results = emgStreamResolver.results();
        yield return new WaitUntil(() => results.Length > 0);
        Debug.Log(string.Format("Resolving Stream: {0}", emgStreamName));
        emgStreamInlet = new liblsl.StreamInlet(results[0]);
        emgExpectedChannels = emgStreamInlet.info().channel_count();
        yield return null;
    }

    public void Get_LSL_EMGSample()
    {
        emgSample = new float[emgExpectedChannels];
        try
        {
            double lastTimeStamp = emgStreamInlet.pull_sample(emgSample, 0.0f);
            if (lastTimeStamp != 0.0)
            {
                // do not miss the first one found
                ProcessEMG(emgSample, lastTimeStamp);
                // pull as long samples are available
                while ((lastTimeStamp = emgStreamInlet.pull_sample(emgSample, 0.0f)) != 0)
                {
                    ProcessEMG(emgSample, lastTimeStamp);
                }
            }
        }
        catch (ArgumentException aex)
        {
            Debug.LogError("An Error on pulling samples deactivating LSL inlet on...", this);
            this.enabled = false;
            Debug.LogException(aex, this);
        }
    }

    // protected abstract void ProcessEMG(float[] newSample, double timeStamp);

    void FixedUpdate()
    {
        if (moment == UpdateMoment.FixedUpdate && emgStreamInlet != null)
            Get_LSL_EMGSample();
    }

    void Update()
    {
        if (moment == UpdateMoment.Update && emgStreamInlet != null)
            Get_LSL_EMGSample();
    }
    */
}
