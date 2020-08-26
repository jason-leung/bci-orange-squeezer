using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class BlockManager : MonoBehaviour
{
    // Block and Trials
    public List<string> blocks;
    public List<List<string>> trials;
    public int trials_per_block;
    public int block_number;
    private int trial_number;
    bool trialStarted;
    DateTime trialStartTime;
    public List<List<float>> trial_timings;

    // UI
    public GameObject blockPanel;
    public GameObject blockNumber;
    public GameObject blockAction;
    public GameObject blockImagine;
    public GameObject sessionOverPanel;
    public GameObject glassRight;
    public GameObject glassLeft;
    public GameObject cueReady;
    public GameObject cueGo;

    // Classes
    public JuiceController juiceController;

    void Start()
    {
        // Find UI elements
        blockPanel = FindObjectOfType<Canvas>().transform.Find("Block_Panel").gameObject;
        blockNumber = blockPanel.transform.Find("BlockNumber").gameObject;
        blockAction = blockPanel.transform.Find("Action_Mode").gameObject;
        blockImagine = blockPanel.transform.Find("Imagine_Mode").gameObject;
        sessionOverPanel = FindObjectOfType<Canvas>().transform.Find("SessionOver_Panel").gameObject;
        glassRight = GameObject.Find("Right").transform.Find("Glass").gameObject;
        glassLeft = GameObject.Find("Left").transform.Find("Glass").gameObject;
        cueReady = FindObjectOfType<Canvas>().transform.Find("Ready_Text").gameObject;
        cueGo = FindObjectOfType<Canvas>().transform.Find("Go_Text").gameObject;

        // Find classes
        juiceController = FindObjectOfType<JuiceController>();

        // Initialize blocks and trials
        // blocks = new List<string>() { "ME", "ME", "ME", "MI", "MI", "MI" };
        blocks = new List<string>() { "ME", "MI" };
        trials = new List<List<string>>();
        // trials_per_block = 16;
        trials_per_block = 2;
        trial_timings = new List<List<float>>() { new List<float>() { 0f, 1f }, new List<float>() { 1f, 2f }, // cue ready, cue go
                                                  new List<float>() { 2f, 8f }, new List<float>() { 8f, 9.5f } }; // task, rest
        block_number = 0;
        trial_number = 0;
        trialStarted = false;

        InitializeTrials();
        LoadBlock();
    }

    void Update()
    {
        if (trialStarted)
        {
            DateTime currentTime = DateTime.Now;
            double timeElapsed = currentTime.Subtract(trialStartTime).TotalSeconds;
            
            // Cue Ready
            if (timeElapsed >= trial_timings[0][0] && timeElapsed < trial_timings[0][1])
            {
                cueReady.SetActive(true);
                cueGo.SetActive(false);
                glassLeft.SetActive(trials[block_number][trial_number] == "l");
                glassRight.SetActive(trials[block_number][trial_number] == "r");
            }
            // Cue Go
            else if (timeElapsed >= trial_timings[1][0] && timeElapsed < trial_timings[1][1])
            {
                cueReady.SetActive(false);
                cueGo.SetActive(true);
                glassLeft.SetActive(trials[block_number][trial_number] == "l");
                glassRight.SetActive(trials[block_number][trial_number] == "r");
            }
            // Task
            else if (timeElapsed >= trial_timings[2][0] && timeElapsed < trial_timings[2][1])
            {
                cueReady.SetActive(false);
                cueGo.SetActive(false);
                glassLeft.SetActive(trials[block_number][trial_number] == "l");
                glassRight.SetActive(trials[block_number][trial_number] == "r");
            }
            // Rest
            else if (timeElapsed >= trial_timings[3][0] && timeElapsed < trial_timings[3][1])
            {
                cueReady.SetActive(false);
                cueGo.SetActive(false);
                glassLeft.SetActive(false);
                glassRight.SetActive(false);
            }
            // End of trial
            else if (timeElapsed >= trial_timings[3][1])
            {
                trialStarted = false;
                trial_number += 1;

                // Start another trial
                if (trial_number < trials_per_block)
                {
                    StartTrial();
                }
                // End of block
                else
                {
                    block_number += 1;
                    trial_number = 0;
                    
                    // Start another block
                    if (block_number < blocks.Count)
                    {
                        LoadBlock();
                    }
                    // End of session
                    else
                    {
                        sessionOverPanel.SetActive(true);
                    }
                }
            }
        }
    }

    public void InitializeTrials()
    {
        // Generate ME trials
        int num_ME_blocks = ((from x in blocks where x.Equals("ME") select x).Count());
        List<string> trials_ME = new List<string>();
        for (int i = 0; i < trials_per_block * num_ME_blocks; i++) trials_ME.Add(i % 2 == 0 ? "r" : "l");
        trials_ME = trials_ME.OrderBy(x => UnityEngine.Random.value).ToList();

        // Generate MI trials
        int num_MI_blocks = ((from x in blocks where x.Equals("MI") select x).Count());
        List<string> trials_MI = new List<string>();
        for (int i = 0; i < trials_per_block * num_MI_blocks; i++) trials_MI.Add(i % 2 == 0 ? "r" : "l");
        trials_MI = trials_MI.OrderBy(x => UnityEngine.Random.value).ToList();

        // Add ME and MI trials to list
        int index_ME = 0;
        int index_MI = 0;
        for (int i = 0; i < blocks.Count; i++)
        {
            if (blocks[i] == "ME")
            {
                trials.Add(trials_ME.GetRange(index_ME, trials_per_block));
                index_ME += trials_per_block;
            }
            else if (blocks[i] == "MI")
            {
                trials.Add(trials_MI.GetRange(index_MI, trials_per_block));
                index_MI += trials_per_block;
            }
        }
    }

    public void LoadBlock()
    {
        blockPanel.SetActive(true);
        blockNumber.GetComponent<Text>().text = (block_number+1).ToString();
        blockAction.SetActive(blocks[block_number] == "ME");
        blockImagine.SetActive(blocks[block_number] == "MI");
    }

    public void StartBlock()
    {
        blockPanel.SetActive(false);
    }

    public void StartTrial()
    {
        trialStarted = true;
        trialStartTime = DateTime.Now;
        juiceController.ResetJuiceSpawner();
    }

    public void ReturnToHome()
    {
        SceneManager.LoadScene("Home");
    }
}
