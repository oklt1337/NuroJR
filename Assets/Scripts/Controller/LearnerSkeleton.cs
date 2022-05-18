using System;
using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(Learner))]
    public class LearnerSkeleton : MonoBehaviour
    {
        private Learner learner;
        [SerializeField, Tooltip("Max Lifetime")] private float lifeTime = 10f;

        #region Unity Methods

        private void Start()
        {
            learner = GetComponent<Learner>();
            lifeTime += Time.time;
        }
        
        private void FixedUpdate()
        {
            // Check if Learner is alive
            if (!learner.Alive) 
                return;
            
            // Create Inputs
            var inputs = new float[3];
            // Set Inputs
            inputs[0] = 1;
            inputs[1] = 1;
            inputs[2] = 1;

            // Generate Outputs
            var outputs = learner.Think(inputs);
            
            // This is an example of how you could use the output (the output is a float between -1 and 1)
            transform.position += new Vector3(outputs[0], 0, outputs[1]) * Time.fixedDeltaTime;

            // Check if time is up
            if (!(lifeTime < Time.time)) 
                return;
            // Set Learner to Not Alive
            learner.Alive = false;
        }

        #endregion
        
        #region Private Methods

        /// <summary>
        /// Increase Fitness of Network
        /// </summary>
        /// <param name="fitness">float increase value</param>
        private void AddFitness(float fitness)
        {
            // Null Check
            if (learner == null)
                return;
            // Add float to fitness if does sth correct make sure value is positive
            learner.Fitness += Math.Abs(fitness);
        }

        #endregion
    }
}