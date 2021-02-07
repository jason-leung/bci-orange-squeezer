using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    public GameObject howtoplay_panel;
    public GameObject howtoplay_pageNumber;
    public GameObject howtoplay_backButton;
    public GameObject howtoplay_nextButton;
    public int pageNumber;
    public GameObject settings_panel;
    public GameObject[] settings_emgMax_slider;
    public GameObject[] settings_emgMax_value;
    public GameObject settings_numBlocks_slider;
    public GameObject settings_numBlocks_value;
    public GameObject settings_numTrials_slider;
    public GameObject settings_numTrials_value;

    void Start()
    {
        howtoplay_panel = FindObjectOfType<Canvas>().transform.Find("HowToPlay_Panel").gameObject;
        howtoplay_pageNumber = howtoplay_panel.transform.Find("PageNumber").gameObject;
        howtoplay_backButton = howtoplay_panel.transform.Find("Back_Button").gameObject;
        howtoplay_nextButton = howtoplay_panel.transform.Find("Next_Button").gameObject;
        pageNumber = System.Int32.Parse(howtoplay_pageNumber.GetComponent<Text>().text);
        howtoplay_backButton.SetActive(false);
    }

    private void Awake()
    {
        settings_emgMax_slider = new GameObject[2];
        settings_emgMax_value = new GameObject[2];
        settings_panel = FindObjectOfType<Canvas>().transform.Find("Settings_Panel").gameObject;
        settings_emgMax_slider[0] = settings_panel.transform.Find("EMG_Max_Left").transform.Find("Slider").gameObject;
        settings_emgMax_slider[1] = settings_panel.transform.Find("EMG_Max_Right").transform.Find("Slider").gameObject;
        settings_numBlocks_slider = settings_panel.transform.Find("Num_Blocks").transform.Find("Slider").gameObject;
        settings_numTrials_slider = settings_panel.transform.Find("Num_Trials").transform.Find("Slider").gameObject;
        settings_emgMax_value[0] = settings_panel.transform.Find("EMG_Max_Left").transform.Find("Value").gameObject;
        settings_emgMax_value[1] = settings_panel.transform.Find("EMG_Max_Right").transform.Find("Value").gameObject;
        settings_numBlocks_value = settings_panel.transform.Find("Num_Blocks").transform.Find("Value").gameObject;
        settings_numTrials_value = settings_panel.transform.Find("Num_Trials").transform.Find("Value").gameObject;

        // Default EMG MAX settings
        if (!PlayerPrefs.HasKey("EMG_Max_Left")) PlayerPrefs.SetFloat("EMG_Max_Left", 11f / 1000f);
        if (!PlayerPrefs.HasKey("EMG_Max_Right")) PlayerPrefs.SetFloat("EMG_Max_Right", 11f / 1000f);

        if (PlayerPrefs.HasKey("EMG_Max_Left")) settings_emgMax_slider[0].GetComponent<Slider>().value = PlayerPrefs.GetFloat("EMG_Max_Left") * 1000f;
        if (PlayerPrefs.HasKey("EMG_Max_Right")) settings_emgMax_slider[1].GetComponent<Slider>().value = PlayerPrefs.GetFloat("EMG_Max_Right") * 1000f;

        // Default trial settings
        if (!PlayerPrefs.HasKey("NumBlocks")) PlayerPrefs.SetInt("NumBlocks", 6);
        if (!PlayerPrefs.HasKey("NumTrials")) PlayerPrefs.SetInt("NumTrials", 16);

        if (PlayerPrefs.HasKey("NumBlocks")) settings_numBlocks_slider.GetComponent<Slider>().value = PlayerPrefs.GetInt("NumBlocks");
        if (PlayerPrefs.HasKey("NumTrials")) settings_numTrials_slider.GetComponent<Slider>().value = PlayerPrefs.GetInt("NumTrials");
    }

    public void ExitApplication()
    {
        PlayerPrefs.DeleteAll(); 
        Application.Quit();
    }

    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    public void HowToPlay()
    {
        howtoplay_pageNumber.GetComponent<Text>().text = "1";
        howtoplay_backButton.SetActive(false);
        howtoplay_nextButton.SetActive(true);
        for (int i = 1; i <= 9; i++) howtoplay_panel.transform.Find("Page" + i).gameObject.SetActive(i == 1);
        howtoplay_panel.SetActive(!howtoplay_panel.activeSelf);
    }

    public void HowToPlayLastPage()
    {
        pageNumber = System.Int32.Parse(howtoplay_pageNumber.GetComponent<Text>().text);
        pageNumber -= 1;
        pageNumber = Math.Max(1, pageNumber);
        howtoplay_pageNumber.GetComponent<Text>().text = pageNumber.ToString();
        howtoplay_backButton.SetActive(pageNumber != 1);
        howtoplay_nextButton.SetActive(pageNumber != 9);

        for (int i = 1; i <= 9; i++) howtoplay_panel.transform.Find("Page" + i).gameObject.SetActive(i == pageNumber);
    }

    public void HowToPlayNextPage()
    {
        pageNumber = System.Int32.Parse(howtoplay_pageNumber.GetComponent<Text>().text);
        pageNumber += 1;
        pageNumber = Math.Min(9, pageNumber);
        howtoplay_pageNumber.GetComponent<Text>().text = pageNumber.ToString();
        howtoplay_backButton.SetActive(pageNumber != 1);
        howtoplay_nextButton.SetActive(pageNumber != 9);

        for (int i = 1; i <= 9; i++) howtoplay_panel.transform.Find("Page" + i).gameObject.SetActive(i == pageNumber);
    }

    public void Settings()
    {
        settings_panel.SetActive(!settings_panel.activeSelf);
    }

    public void OnSetEMGSlider(int side)
    {
        double emg_max = System.Math.Round(settings_emgMax_slider[side].GetComponent<Slider>().value, 2);
        settings_emgMax_value[side].GetComponent<Text>().text = emg_max.ToString("n2") + " mV";
        string prefKey = "EMG_Max_";
        prefKey += (side == 0) ? "Left" : "Right";
        PlayerPrefs.SetFloat(prefKey, (float)(emg_max / 1000f));
    }

    public void OnSetNumBlocksSlider()
    {
        int numBlocks = (int)settings_numBlocks_slider.GetComponent<Slider>().value;
        settings_numBlocks_value.GetComponent<Text>().text = numBlocks.ToString();
        PlayerPrefs.SetInt("NumBlocks", numBlocks);
    }

    public void OnSetNumTrialsSlider()
    {
        int numTrials = (int)settings_numTrials_slider.GetComponent<Slider>().value;
        settings_numTrials_value.GetComponent<Text>().text = numTrials.ToString();
        PlayerPrefs.SetInt("NumTrials", numTrials);
    }
}
