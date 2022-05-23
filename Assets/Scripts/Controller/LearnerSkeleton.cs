using System;
using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(Learner))]
    public abstract class LearnerSkeleton : MonoBehaviour
    {
        [SerializeField] private Learner learner;

        #region Unity Methods

        private void Start()
        {
            learner = GetComponent<Learner>();
        }

        private void FixedUpdate()
        {
            // Check if Learner is alive
            if (!learner.Alive)
                return;
            
            SetTime();
            // Generate Outputs
            var outputs = learner.Think(GenerateInputs());
            Action(outputs);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Increase Fitness of Network
        /// </summary>
        /// <param name="fitness">float increase value</param>
        public void AddFitness(float fitness)
        {
            // Null Check
            if (learner == null)
                return;
            if (!learner.Alive)
                return;
            // Add float to fitness if does sth correct make sure value is positive
            learner.Fitness += Math.Abs(fitness);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Do Stuff with Outputs
        /// </summary>
        protected abstract void Action(float[] outputs);

        /// <summary>
        /// Generate your Inputs.
        /// </summary>
        /// <returns>Inputs float[]</returns>
        protected abstract float[] GenerateInputs();

        #endregion

        #region Private Methods

        /// <summary>
        /// Setting the time the learner is alive
        /// </summary>
        private void SetTime()
        {
            learner.TimeAlive += Time.fixedDeltaTime;
        }

        #endregion
    }
}