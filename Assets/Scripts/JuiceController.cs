using Assets.LSL4Unity.Scripts.Examples;
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

    public BlockManager blockManager;

    // Start is called before the first frame update
    void Start()
    {
        // Find UI compoenents
        waterSpawner_right = GameObject.Find("Right").transform.Find("Water2D_spawner").GetComponent<Water2D_Spawner>();
        waterSpawner_left = GameObject.Find("Left").transform.Find("Water2D_spawner").GetComponent<Water2D_Spawner>();

        // Find classes
        blockManager = FindObjectOfType<BlockManager>();

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
        // case 1-9: adjust water amount based on numeric key press
        for (int i = 1; i <= 9; i++)
        {
            // Use keypad numbers for right hand
            if (Input.GetKey(key_keypad[i]))
            {
                SqueezeRight(i);
                return;
            }

            // Use alpha numbers for left hand
            if (Input.GetKey(key_alpha[i]))
            {
                SqueezeLeft(i);
                return;
            }
        }

        // base case: turn off water when 0 or nothing pressed
        StopSqueezeLeft();
        StopSqueezeRight();
    }

    public void StopSqueezeLeft()
    {
        if (Time.timeScale == 0f) return; // don't do anything if timescale is set to 0
        orange_left.transform.localScale = new Vector3(-0.5f, 0.5f, 1f); 
        waterSpawner_left._breakLoop = true;
    }

    public void StopSqueezeRight()
    {
        if (Time.timeScale == 0f) return; // don't do anything if timescale is set to 0
        orange_right.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        waterSpawner_right._breakLoop = true;
    }

    public void SqueezeLeft(float magnitude)
    {
        if (Time.timeScale == 0f) return; // don't do anything if timescale is set to 0

        if (blockManager.state == "task")
        {
            orange_left.transform.localScale = new Vector3(-0.5f + (magnitude * (0.15f / 9f)), 0.5f, 1f);
            waterSpawner_left.DelayBetweenParticles = 10f - magnitude;
            waterSpawner_left.initSpeed = new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), -2f);
            waterSpawner_left.Spawn();
        }
    }

    public void SqueezeRight(float magnitude)
    {
        if (Time.timeScale == 0f) return; // don't do anything if timescale is set to 0

        if (blockManager.state == "task")
        {
            orange_right.transform.localScale = new Vector3(0.5f - (magnitude * (0.15f / 9f)), 0.5f, 1f);
            waterSpawner_right.DelayBetweenParticles = 10f - magnitude;
            waterSpawner_right.initSpeed = new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), -2f);
            waterSpawner_right.Spawn();
        }
    }
}
