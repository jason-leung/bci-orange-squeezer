using Assets.LSL4Unity.Scripts;
using Assets.LSL4Unity.Scripts.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionPanel : MonoBehaviour
{
    public GameObject connectionPanel;
    public GameObject gameMarker;
    public GameObject emg;
    public GameObject eeg;
    public GameObject continueButton;
    
    public Sprite onlineSprite;
    public Sprite offlineSprite;

    public bool isGameMarkerConnected = false;
    public bool isEMGConnected = false;
    public bool isEEGConnected = false;
    public bool pauseGame = true;

    public EMGStreamInlet emgStreamInlet;
    public GameMarkerStream gameMarkerStream;
    

    // Start is called before the first frame update
    void Start()
    {
        connectionPanel = FindObjectOfType<Canvas>().transform.Find("Connection_Panel").gameObject;
        gameMarker = connectionPanel.transform.Find("GameMarker").gameObject;
        emg = connectionPanel.transform.Find("EMG").gameObject;
        eeg = connectionPanel.transform.Find("EEG").gameObject;
        continueButton = connectionPanel.transform.Find("Continue_Button").gameObject;

        emgStreamInlet = FindObjectOfType<EMGStreamInlet>();
        gameMarkerStream = FindObjectOfType<GameMarkerStream>();
    }

    // Update is called once per frame
    void Update()
    {
        // get connection status
        isGameMarkerConnected = true;
        isEMGConnected = emgStreamInlet.pullSamplesContinuously;
        isEEGConnected = true;

        // Pause game if any connections are offline
        if (!isGameMarkerConnected || !isEMGConnected || !isEEGConnected) pauseGame = true;

        if (pauseGame)
        {
            // Stop time and send marker
            Time.timeScale = 0f;
            gameMarkerStream.WriteGameMarker("conection_lost");

            // update UI
            connectionPanel.SetActive(true);
            SetStreamConnection(gameMarker, isGameMarkerConnected);
            SetStreamConnection(emg, isEMGConnected);
            SetStreamConnection(eeg, isEEGConnected);
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
        bool enableContinueButton = !(!isGameMarkerConnected || !isEMGConnected || !isEEGConnected);
        continueButton.GetComponent<Image>().sprite = enableContinueButton ? onlineSprite : offlineSprite;
        continueButton.transform.Find("Text").GetComponent<Text>().text = enableContinueButton ? "Continue" : "Check Connections";
        continueButton.GetComponent<Button>().interactable = enableContinueButton;
    }

    public void Continue()
    {
        pauseGame = false;
        connectionPanel.SetActive(false);
        gameMarkerStream.WriteGameMarker("conection_found");
        Time.timeScale = 1f;
    }
}
