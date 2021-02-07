using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.LSL4Unity.Scripts;

public class TrialManager : MonoBehaviour
{
    // Trial Variables
    public List<string> blockTypes;
    public List<string> stateTypes;
    public List<int> stateStartFrame;
    public int numBlocks;
    public int numTrials;
    public int trialFrames;
    public int frameNumber;
    public int currentBlock;
    public int currentTrial;
    public string currentState;
    public bool currentTrialFailed;
    public List<string> blockStructure;
    public List<List<string>> trialStructure;
    public bool allBlocksCompleted;
    public bool gamePaused;

    // UI
    public GameObject readyText;
    public GameObject goText;
    public GameObject glassL;
    public GameObject glassR;
    public GameObject handL;
    public GameObject handR;
    public GameObject armL;
    public GameObject armR;
    public GameObject juiceL;
    public GameObject juiceR;
    public GameObject blockPanel;
    public GameObject connectionPanel;
    public GameObject sessionOverPanel;
    public GameObject clearBox;
    public GameObject backgroundCover;

    // LSL
    public GameMarkerStream gameMarkerStream;

    // Start is called before the first frame update
    void Start()
    {
        // Trial
        blockTypes = new List<string>() { "ME", "MI" };
        stateTypes = new List<string>() { "cue_ready", "cue_go", "task", "rest" };
        stateStartFrame = new List<int>() { 0, 49, 50, 99, 100, 399, 400, 499 };
        trialFrames = 500;
        numBlocks = PlayerPrefs.GetInt("NumBlocks");
        numTrials = PlayerPrefs.GetInt("NumTrials");
        frameNumber = 0;

        currentBlock = 0;
        currentTrial = 0;
        currentState = "";
        currentTrialFailed = false;

        blockStructure = new List<string>();
        trialStructure = new List<List<string>>();
        for (int b = 0; b < numBlocks; b++)
        {
            blockStructure.Add((b % 2 == 0) ? blockTypes[0] : blockTypes[1]);
            List<string> trial = new List<string>();
            for (int t = 0; t < numTrials; t++)
            {
                trial.Add((t % 2 == 0) ? "r" : "l");
            }
            trial = trial.OrderBy(x => UnityEngine.Random.value).ToList();
            trialStructure.Add(trial);
        }
        blockStructure = blockStructure.OrderBy(x => UnityEngine.Random.value).ToList();

        allBlocksCompleted = false;
        gamePaused = true;

        // UI
        blockPanel.transform.Find("BlockNumber").gameObject.GetComponent<Text>().text = (currentBlock + 1).ToString();
        blockPanel.transform.Find("Action_Mode").gameObject.SetActive(blockStructure[currentBlock] == "ME");
        blockPanel.transform.Find("Imagine_Mode").gameObject.SetActive(blockStructure[currentBlock] == "MI");

        // LSL
        gameMarkerStream = FindObjectOfType<GameMarkerStream>();

    }

    void FixedUpdate()
    {

        // Write GameMarkers
        if (frameNumber == stateStartFrame[0] || frameNumber == stateStartFrame[2] || frameNumber == stateStartFrame[4] || frameNumber == stateStartFrame[6])
        {
            gameMarkerStream.WriteGameMarker(currentState);
        }
        if (allBlocksCompleted) gameMarkerStream.WriteGameMarker("trial_END");

        // Update frames
        if(!allBlocksCompleted && !gamePaused)
        {
            frameNumber += 1;
            // Reached the end of trial
            if (frameNumber == trialFrames)
            {
                frameNumber = 0;
                if (!currentTrialFailed) currentTrial += 1;
                currentTrialFailed = false;

                // Reached the end of block
                if (currentTrial > numTrials - 1)
                {
                    currentTrial = 0;
                    currentBlock += 1;
                    gamePaused = true;

                    // All blocks completed
                    if (currentBlock > numBlocks - 1)
                    {
                        currentBlock = 0;
                        allBlocksCompleted = true;
                    }

                    Time.timeScale = 0f;
                    blockPanel.transform.Find("BlockNumber").gameObject.GetComponent<Text>().text = (currentBlock + 1).ToString();
                    blockPanel.transform.Find("Action_Mode").gameObject.SetActive(blockStructure[currentBlock] == "ME");
                    blockPanel.transform.Find("Imagine_Mode").gameObject.SetActive(blockStructure[currentBlock] == "MI");
                    blockPanel.SetActive(true);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (allBlocksCompleted)
        {
            sessionOverPanel.SetActive(true);
            return;
        }

        // Cue ready
        if (frameNumber >= stateStartFrame[0] && frameNumber <= stateStartFrame[1])
        {
            currentState = stateTypes[0];
            readyText.SetActive(true);
            glassL.SetActive(trialStructure[currentBlock][currentTrial] == "l");
            glassR.SetActive(trialStructure[currentBlock][currentTrial] == "r");
            handL.SetActive(true);
            handR.SetActive(true);
            armL.SetActive(true);
            armR.SetActive(true);

            juiceL.SetActive(false);
            juiceR.SetActive(false);

            clearBox.SetActive(false);
            backgroundCover.SetActive(false);
        }
        // Cue go
        else if (frameNumber >= stateStartFrame[2] && frameNumber <= stateStartFrame[3])
        {
            currentState = stateTypes[1];
            readyText.SetActive(false);
            goText.SetActive(true);
        }
        // Task
        else if (frameNumber >= stateStartFrame[4] && frameNumber <= stateStartFrame[5])
        {
            currentState = stateTypes[2];
            goText.SetActive(false);

            if (blockStructure[currentBlock] == "MI")
            {
                if (trialStructure[currentBlock][currentTrial] == "l")
                {
                    juiceL.SetActive(true);
                    juiceL.GetComponent<ParticleGenerator>().particlesState = DynamicParticle.STATES.LAVA;
                    handL.transform.localScale = new Vector3(0.5f, 0.8f, 0.02221f);
                    juiceR.SetActive(false);
                    juiceR.GetComponent<ParticleGenerator>().particlesState = DynamicParticle.STATES.NONE;
                    handR.transform.localScale = new Vector3(0.8f, 0.8f, 0.02221f);
                }
                else
                {
                    juiceL.SetActive(false);
                    juiceL.GetComponent<ParticleGenerator>().particlesState = DynamicParticle.STATES.NONE;
                    handL.transform.localScale = new Vector3(0.8f, 0.8f, 0.02221f);
                    juiceR.SetActive(true);
                    juiceR.GetComponent<ParticleGenerator>().particlesState = DynamicParticle.STATES.LAVA;
                    handR.transform.localScale = new Vector3(0.5f, 0.8f, 0.02221f);
                }
            }
            else if (blockStructure[currentBlock] == "ME")
            {
                juiceL.GetComponent<ParticleGenerator>().particlesState = DynamicParticle.STATES.LAVA;
                juiceR.GetComponent<ParticleGenerator>().particlesState = DynamicParticle.STATES.LAVA;
            }

        }
        // Rest
        else if (frameNumber >= stateStartFrame[6] && frameNumber <= stateStartFrame[7])
        {
            currentState = stateTypes[3];
            glassL.SetActive(false);
            glassR.SetActive(false);
            handL.SetActive(false);
            handR.SetActive(false);
            armL.SetActive(false);
            armR.SetActive(false);

            handL.transform.localScale = new Vector3(0.8f, 0.8f, 0.02221f);
            handR.transform.localScale = new Vector3(0.8f, 0.8f, 0.02221f);
            juiceL.SetActive(false);
            juiceL.GetComponent<ParticleGenerator>().particlesState = DynamicParticle.STATES.NONE;
            juiceR.SetActive(false);
            juiceR.GetComponent<ParticleGenerator>().particlesState = DynamicParticle.STATES.NONE;
            
            clearBox.SetActive(true);
            backgroundCover.SetActive(true);
        }
        // Error State
        else
        {
            currentState = "";
        }
    }

    public void StartBlock()
    {
        gamePaused = false;
        Time.timeScale = 1f;
        blockPanel.SetActive(false);
        gameMarkerStream.WriteGameMarker("start_block_" + currentBlock + "_" + blockStructure[currentBlock] + "_trial_" + currentTrial + "_" + trialStructure[currentBlock][currentTrial]);
    }

    public void ContinueAfterConnectionLost()
    {
        gamePaused = false;
        Time.timeScale = 1f;
        connectionPanel.SetActive(false);
    }

    public void ReturnToHome()
    {
        SceneManager.LoadScene("Home");
    }
}
