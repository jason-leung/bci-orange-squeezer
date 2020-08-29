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
    public GameObject settings_emgMax_slider;
    public GameObject settings_emgMax_value;

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
        settings_panel = FindObjectOfType<Canvas>().transform.Find("Settings_Panel").gameObject;
        settings_emgMax_slider = settings_panel.transform.Find("EMG_Max").transform.Find("Slider").gameObject;
        settings_emgMax_value = settings_panel.transform.Find("EMG_Max").transform.Find("Value").gameObject;

        if (PlayerPrefs.HasKey("EMG_max")) settings_emgMax_slider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("EMG_max");
    }

    public void ExitApplication()
    {
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

    public void OnSetEMGSlider()
    {
        double emg_max = System.Math.Round(settings_emgMax_slider.GetComponent<Slider>().value, 2);
        settings_emgMax_value.GetComponent<Text>().text = emg_max.ToString("n2") + " V";
        PlayerPrefs.SetFloat("EMG_max", (float)emg_max);
    }
}
