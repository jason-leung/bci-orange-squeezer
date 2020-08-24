using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
    GameObject howtoplay_panel;
    // Start is called before the first frame update
    void Start()
    {
        howtoplay_panel = FindObjectOfType<Canvas>().transform.Find("HowToPlay_Panel").gameObject;
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void HowToPlay()
    {
        howtoplay_panel.SetActive(!howtoplay_panel.activeSelf);
    }
}
