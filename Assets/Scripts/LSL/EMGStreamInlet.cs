using System.Collections; 
using UnityEngine; 
using Assets.LSL4Unity.Scripts.AbstractInlets;
using System;

namespace Assets.LSL4Unity.Scripts.Examples {

    public class EMGStreamInlet : InletFloatSamples
    {
        public bool pullSamplesContinuously = false;

        public GameMarkerStream gameMarkerStream;
        public TrialManager trialManager;

        public bool coroutineAlreadyRunning = false;

        public float[] emgProcessed;
        float[] emg_max;
        float[] sample_max;
        int sample_number = 0;
        int window_size = 50;

        void Start()
        {
            // [optional] call this only, if your gameobject hosting this component
            // got instantiated during runtime

            // registerAndLookUpStream();
            trialManager= FindObjectOfType<TrialManager>();
            gameMarkerStream = FindObjectOfType<GameMarkerStream>();

            emgProcessed = new float[2];
            emg_max = new float[2] { 0.011f, 0.011f };
            if (PlayerPrefs.HasKey("EMG_Max_Left")) emg_max[0] = PlayerPrefs.GetFloat("EMG_Max_Left");
            if (PlayerPrefs.HasKey("EMG_Max_Right")) emg_max[1] = PlayerPrefs.GetFloat("EMG_Max_Right");
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
            if (trialManager.currentState != "task") return;
            if (trialManager.blockStructure[trialManager.currentBlock] != "ME") return;
            if (pullSamplesContinuously == false) return;
            if (newSample.Length < 6) return;
            
            StartCoroutine(ProcessEMGSample(newSample));
        }

        protected override void OnStreamAvailable()
        {
            pullSamplesContinuously = true;
            gameMarkerStream.WriteGameMarker("EMG stream found");
        }

        protected override void OnStreamLost()
        {
            pullSamplesContinuously = false;
            gameMarkerStream.WriteGameMarker("EMG stream lost");
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
                emgProcessed[0] = System.Math.Min(sample_max[0] / emg_max[0] , 1f);
                emgProcessed[1] = System.Math.Min(sample_max[1] / emg_max[1] , 1f);

                if (emgProcessed[0] <= 0.1f)
                {
                    trialManager.juiceL.SetActive(false);
                    trialManager.handL.transform.localScale = new Vector3(0.8f, 0.8f, 0.02221f);
                }
                else
                {
                    trialManager.juiceL.SetActive(true);
                    trialManager.handL.transform.localScale = new Vector3(0.8f - (0.3f * emgProcessed[0]), 0.8f, 0.02221f);
                }

                if (emgProcessed[1] <= 0.1f)
                {
                    trialManager.juiceR.SetActive(false);
                    trialManager.handR.transform.localScale = new Vector3(0.8f, 0.8f, 0.02221f);
                }
                else
                {
                    trialManager.juiceR.SetActive(true);
                    trialManager.handR.transform.localScale = new Vector3(0.8f - (0.3f * emgProcessed[1]), 0.8f, 0.02221f);
                }

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
