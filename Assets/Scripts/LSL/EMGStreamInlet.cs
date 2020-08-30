using System.Collections; 
using UnityEngine; 
using Assets.LSL4Unity.Scripts.AbstractInlets;
using System;

namespace Assets.LSL4Unity.Scripts.Examples {

    public class EMGStreamInlet : InletFloatSamples
    {
        public bool pullSamplesContinuously = false;
        public float[] emgSample;
        public float[] emgProcessed;
        float emg_max = 11f;
        bool processingSample = false;
        DateTime lastProcessTime = DateTime.Now;

        public JuiceController juiceController;
        public FileWriter fileWriter;

        void Start()
        {
            // [optional] call this only, if your gameobject hosting this component
            // got instantiated during runtime

            // registerAndLookUpStream();
            juiceController = FindObjectOfType<JuiceController>();
            fileWriter = FindObjectOfType<FileWriter>();
            emgProcessed = new float[2];
            emg_max = PlayerPrefs.GetFloat("EMG_max");
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
            if (pullSamplesContinuously == false) return;
            if (newSample.Length < 2) return;
            if (processingSample) return;
            if (DateTime.Now.Subtract(lastProcessTime).TotalMilliseconds <= 100) return;

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
            if(pullSamplesContinuously)
                pullSamples();
        }

        IEnumerator ProcessEMGSample(float[] newSample)
        {
            processingSample = true;
            lastProcessTime = DateTime.Now;

            emgSample = newSample;

            fileWriter.WriteEMG(DateTime.Now.ToOADate(), newSample);

            for (int i = 0; i < 2; i++)
                emgProcessed[i] = System.Math.Min(System.Math.Abs(emgSample[i]), emg_max) / emg_max * 9f;

            if (emgProcessed[0] <= 1f) juiceController.StopSqueezeLeft();
            else juiceController.SqueezeLeft(emgProcessed[0]);
            if (emgProcessed[1] <= 1f) juiceController.StopSqueezeRight();
            else juiceController.SqueezeRight(emgProcessed[1]);

            processingSample = false;

            yield break;
        }
    }
}