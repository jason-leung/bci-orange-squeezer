using System.Collections; 
using UnityEngine; 
using Assets.LSL4Unity.Scripts.AbstractInlets;
using System;
using UnityEngine.UI;
namespace Assets.LSL4Unity.Scripts.Examples {

    public class EMGStreamTestInlet : InletFloatSamples
    {
        public bool pullSamplesContinuously = false;
        public bool coroutineAlreadyRunning = false;

        public float[] emgProcessed;
        float[] emg_min;
        float[] emg_max;
        float[] sample_max;
        int sample_number = 0;
        int window_size = 50;

        public GameObject emgLeft;
        public GameObject emgRight;

        void Start()
        {
            // [optional] call this only, if your gameobject hosting this component
            // got instantiated during runtime

            // registerAndLookUpStream();

            emgProcessed = new float[2];
            emg_min = new float[2] { 0f, 0f };
            emg_max = new float[2] { 0.011f, 0.011f };
            sample_max = new float[2] { 0, 0 };
        }

        protected override bool isTheExpected(LSLStreamInfoWrapper stream)
        {
            // the base implementation just checks for stream name and type
            var predicate = base.isTheExpected(stream);
            // add a more specific description for your stream here specifying hostname etc.
            //predicate &= stream.HostName.Equals("Expected Hostname");
            return predicate;
        }

        /// <summary>
        /// Override this method to implement whatever should happen with the samples...
        /// IMPORTANT: Avoid heavy processing logic within this method, update a state and use
        /// coroutines for more complexe processing tasks to distribute processing time over
        /// several frames
        /// </summary>
        /// <param name="newSample"></param>
        /// <param name="timeStamp"></param>
        protected override void Process(float[] newSample, double timeStamp)
        {
            if (coroutineAlreadyRunning) return;
            if (pullSamplesContinuously == false) return;
            if (newSample.Length < 6) return;
            
            StartCoroutine(ProcessEMGSample(newSample));
        }

        protected override void OnStreamAvailable()
        {
            pullSamplesContinuously = true;
        }

        protected override void OnStreamLost()
        {
            pullSamplesContinuously = false;
        }
         
        private void Update()
        {
            if (pullSamplesContinuously) 
                pullSamples();
        }

        IEnumerator ProcessEMGSample(float[] newSample)
        {
            coroutineAlreadyRunning = true;
            if (sample_number == window_size)
            {
                if (PlayerPrefs.HasKey("EMG_Min_Left")) emg_min[0] = PlayerPrefs.GetFloat("EMG_Min_Left");
                if (PlayerPrefs.HasKey("EMG_Min_Right")) emg_min[1] = PlayerPrefs.GetFloat("EMG_Min_Right");
                if (PlayerPrefs.HasKey("EMG_Max_Left")) emg_max[0] = PlayerPrefs.GetFloat("EMG_Max_Left");
                if (PlayerPrefs.HasKey("EMG_Max_Right")) emg_max[1] = PlayerPrefs.GetFloat("EMG_Max_Right");

                emgProcessed[0] = System.Math.Max(System.Math.Min((sample_max[0] - emg_min[0]) / (emg_max[0] - emg_min[0]), 1f), 0f);
                emgProcessed[1] = System.Math.Max(System.Math.Min((sample_max[1] - emg_min[1]) / (emg_max[1] - emg_min[1]), 1f), 0f);

                // update UI
                emgLeft.transform.Find("Value0").gameObject.GetComponent<Text>().text = newSample[0].ToString();
                emgLeft.transform.Find("Value1").gameObject.GetComponent<Text>().text = newSample[1].ToString();
                emgLeft.transform.Find("Value2").gameObject.GetComponent<Text>().text = newSample[2].ToString();
                emgRight.transform.Find("Value0").gameObject.GetComponent<Text>().text = newSample[3].ToString();
                emgRight.transform.Find("Value1").gameObject.GetComponent<Text>().text = newSample[4].ToString();
                emgRight.transform.Find("Value2").gameObject.GetComponent<Text>().text = newSample[5].ToString();
                emgLeft.transform.Find("ValueProc").gameObject.GetComponent<Text>().text = emgProcessed[0].ToString();
                emgRight.transform.Find("ValueProc").gameObject.GetComponent<Text>().text = emgProcessed[1].ToString();

                sample_number = 0;
                sample_max[0] = 0f;
                sample_max[1] = 0f;
            }

            // Left sensors
            sample_max[0] = System.Math.Max(System.Math.Abs(newSample[0]), sample_max[0]);
            sample_max[0] = System.Math.Max(System.Math.Abs(newSample[1]), sample_max[0]);
            sample_max[0] = System.Math.Max(System.Math.Abs(newSample[2]), sample_max[0]);

            // Right sensors
            sample_max[1] = System.Math.Max(System.Math.Abs(newSample[3]), sample_max[1]);
            sample_max[1] = System.Math.Max(System.Math.Abs(newSample[4]), sample_max[1]);
            sample_max[1] = System.Math.Max(System.Math.Abs(newSample[5]), sample_max[1]);

            sample_number += 1;

            coroutineAlreadyRunning = false;
            yield return null;
        }
    }
}
