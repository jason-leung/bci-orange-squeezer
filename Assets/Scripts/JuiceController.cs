using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Water2D;

public class JuiceController : MonoBehaviour
{
    List<KeyCode> key_alpha;
    List<KeyCode> key_keypad;
    public GameObject orange_right;
    public GameObject orange_left;
    public Water2D_Spawner waterSpawner_right;
    public Water2D_Spawner waterSpawner_left;
    public LSLHandler lslHandler;
    public float[] emg_data;
    public int[] emg_magnitude;

    // Start is called before the first frame update
    void Start()
    {
        // Find UI compoenents
        waterSpawner_right = GameObject.Find("Right").transform.Find("Water2D_spawner").GetComponent<Water2D_Spawner>();
        waterSpawner_left = GameObject.Find("Left").transform.Find("Water2D_spawner").GetComponent<Water2D_Spawner>();
        lslHandler = GameObject.Find("LSLHandler").GetComponent<LSLHandler>();

        // Initialize Input Keys
        key_alpha = new List<KeyCode>();
        key_keypad = new List<KeyCode>();
        for (int i = 0; i < 10; i++)
        {
            key_alpha.Add((KeyCode)(48 + i));
            key_keypad.Add((KeyCode)(256 + i));
        }
    }

    public void ResetJuiceSpawner()
    {
        waterSpawner_right.Restore();
        waterSpawner_right.size = 0.25f;
        waterSpawner_right.initSpeed = new Vector2(0.2f, -2f);
        waterSpawner_right.DelayBetweenParticles = 10f;
        waterSpawner_right.LifeTime = 40f;

        waterSpawner_left.Restore();
        waterSpawner_left.size = 0.25f;
        waterSpawner_left.initSpeed = new Vector2(0.2f, -2f);
        waterSpawner_left.DelayBetweenParticles = 10f;
        waterSpawner_left.LifeTime = 40f;

        orange_right.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        orange_left.transform.localScale = new Vector3(-0.5f, 0.5f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        // Get EMG data
        emg_data = lslHandler.Get_LSL_EMGData();
        // take absolute value, divide by EMG_max, multiple by 9, and round to nearest integer
        for(int i = 0; i < 2; i++)
        {
            emg_magnitude[i] = (int) System.Math.Round(System.Math.Abs(emg_data[i]) / PlayerPrefs.GetFloat("EMG_max") * 9f);
        }
        */
        


        // case 1-9: adjust water amount based on numeric key press by changing delay from 0.3 to 0.05
        for (int i = 1; i <= 9; i++)
        {
            // Use keypad numbers for right hand
            if (Input.GetKey(key_keypad[i]))
            {
                orange_right.transform.localScale = new Vector3(0.5f - (i * (0.15f / 9f)), 0.5f, 1f);
                waterSpawner_right.DelayBetweenParticles = 10f-i;
                waterSpawner_right.initSpeed = new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), -2f);
                waterSpawner_right.Spawn();
                return;
            }

            // Use alpha numbers for left hand
            if (Input.GetKey(key_alpha[i]))
            {
                orange_left.transform.localScale = new Vector3(-0.5f + (i * (0.15f / 9f)), 0.5f, 1f);
                waterSpawner_left.DelayBetweenParticles = 10f - i;
                waterSpawner_left.initSpeed = new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), -2f);
                waterSpawner_left.Spawn();
                return;
            }
        }

        // base case: turn off water when 0 or nothing pressed
        orange_right.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        orange_left.transform.localScale = new Vector3(-0.5f, 0.5f, 1f);
        waterSpawner_right._breakLoop = true;
        waterSpawner_left._breakLoop = true;
    }
}
