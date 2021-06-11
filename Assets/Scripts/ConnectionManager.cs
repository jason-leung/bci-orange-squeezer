using Assets.LSL4Unity.Scripts;
using Assets.LSL4Unity.Scripts.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ConnectionManager : MonoBehaviour
{
    public GameObject connectionPanel;
    public GameObject blockPanel;
    public GameObject continueButton;
    public GameObject gameMarkerObject;
    public GameObject eegStreamObject;
    public GameObject emgStreamObject;

    public GameMarkerStream gameMarkerStream;
    public EMGStreamInlet emgStreamInlet;
    public EEGStreamInlet eegStreamInlet;
    public TrialManager trialManager;

    public Sprite onlineSprite;
    public Sprite offlineSprite;

    public bool isGameMarkerConnected = true;
    public bool isEMGConnected = true;
    public bool isEEGConnected = false;

    public bool connectionLost = false;
    public DateTime lastConnectedTime;
    public float connectionLostThreshold = 0.5f; // seconds


    // Start is called before the first frame update
    void Start()
    {
        gameMarkerStream = FindObjectOfType<GameMarkerStream>();
        emgStreamInlet = FindObjectOfType<EMGStreamInlet>();
        eegStreamInlet = FindObjectOfType<EEGStreamInlet>();
        trialManager = FindObjectOfType<TrialManager>();
        lastConnectedTime = DateTime.Now;
    }

    void FixedUpdate()
    {
        // get connection status
        isGameMarkerConnected = true;
        isEMGConnected = true;
        isEEGConnected = eegStreamInlet.pullSamplesContinuously;
        connectionLost = (!isGameMarkerConnected || !isEMGConnected || !isEEGConnected);

        // get last connectedTime
        if (!connectionLost)
        {
            lastConnectedTime = DateTime.Now;
        }
        else
        {
            if ((DateTime.Now - lastConnectedTime).TotalSeconds >= connectionLostThreshold)
            {
                connectionPanel.SetActive(true);
                trialManager.currentTrialFailed = true;
                trialManager.gamePaused = true;
                gameMarkerStream.WriteGameMarker("current trial failed");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (connectionPanel.activeSelf)
        {
            SetStreamConnection(gameMarkerObject, isGameMarkerConnected);
            SetStreamConnection(emgStreamObject, isEMGConnected);
            SetStreamConnection(eegStreamObject, isEEGConnected);
            UpdateContinueButton();
        }
    }

    public void SetStreamConnection(GameObject stream, bool isConnected)
    {
        stream.transform.Find("Text").GetComponent<Text>().text = isConnected ? "online" : "offline";
        stream.transform.Find("Image").GetComponent<Image>().color = isConnected ? Color.green : Color.red;
    }

    public void UpdateContinueButton()
    {
        continueButton.GetComponent<Image>().sprite = !connectionLost ? onlineSprite : offlineSprite;
        continueButton.transform.Find("Text").GetComponent<Text>().text = !connectionLost ? "Continue" : "Check Connections";
        continueButton.GetComponent<Button>().interactable = !connectionLost;
    }

    public void Continue()
    {
        connectionPanel.SetActive(false);
        gameMarkerStream.WriteGameMarker("conections initialized_" + DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss.fff"));
        Time.timeScale = 1f;
        if (!blockPanel.activeSelf) trialManager.gamePaused = false;
    }
}
