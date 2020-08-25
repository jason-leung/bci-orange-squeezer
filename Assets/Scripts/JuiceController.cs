using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuiceController : MonoBehaviour
{
    List<KeyCode> key_alpha;
    List<KeyCode> key_keypad;
    public GameObject orangeRight;

    // Start is called before the first frame update
    void Start()
    {
        key_alpha = new List<KeyCode>();
        key_keypad = new List<KeyCode>();
        for (int i = 0; i < 10; i++)
        {
            key_alpha.Add((KeyCode)(48 + i));
            key_keypad.Add((KeyCode)(256 + i));
        }

        ResetJuiceSpawner();
    }

    void ResetJuiceSpawner()
    {
        Water2D.Water2D_Spawner.StopSpawner();
        Water2D.Water2D_Spawner.instance.size = 0.25f;
        Water2D.Water2D_Spawner.instance.initSpeed = new Vector2(0.2f, -2f);
        Water2D.Water2D_Spawner.instance.DelayBetweenParticles = 10f;
        Water2D.Water2D_Spawner.instance.LifeTime = 40f;
    }

    // Update is called once per frame
    void Update()
    {
        // Stop and Restore
        if (Input.GetKey(KeyCode.R))
        {
            ResetJuiceSpawner();
            orangeRight.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            return;
        }

        // case 1-9: adjust water amount based on numeric key press by changing delay from 0.3 to 0.05
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKey(key_alpha[i]) || Input.GetKey(key_keypad[i]))
            {
                orangeRight.transform.localScale = new Vector3(0.5f-(i*(0.15f / 9f)), 0.5f, 1f);
                Water2D.Water2D_Spawner.instance.DelayBetweenParticles = 10f-i;
                Water2D.Water2D_Spawner.RunSpawner();
                return;
            }
        }

        // base case: turn off water when 0 or nothing pressed
        orangeRight.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        Water2D.Water2D_Spawner.JustStopSpawner();
    }
}
