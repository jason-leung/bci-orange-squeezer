using System.Collections; 
using UnityEngine; 
using Assets.LSL4Unity.Scripts.AbstractInlets;
using System;

namespace Assets.LSL4Unity.Scripts.Examples {

    public class EEGStreamInlet : InletFloatSamples
    {
        public bool pullSamplesContinuously = false;
        public int numberOfChannels = 64;
        public float[] eegSample;

        public FileWriter fileWriter;

        void Start()
        {
            // [optional] call this only, if your gameobject hosting this component
            // got instantiated during runtime

            // registerAndLookUpStream();
            fileWriter = FindObjectOfType<FileWriter>();
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
            if (newSample.Length < numberOfChannels) return;

            eegSample = newSample;
            StartCoroutine(ProcessEEGSample(timeStamp, newSample));
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

        IEnumerator ProcessEEGSample(double timestamp, float[] newSample)
        {
            fileWriter.WriteEEG(timestamp, newSample);

            yield break;
        }
    }
}