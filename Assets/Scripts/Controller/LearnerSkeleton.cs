using System;
using Test_Environment.Scripts.Pipes;
using Test_Environment.Scripts.Player;
using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(Learner))]
    public class LearnerSkeleton : MonoBehaviour
    {
        private Learner learner;

        [SerializeField, Tooltip("Max Lifetime")]
        private float lifeTime = 10f;

        [SerializeField] private PlayerController playerController;

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
            var position = transform.position;
            inputs[0] = position.y;

            var pipeTrans = PipeManager.Instance.GetFirstPipe();
            inputs[1] = pipeTrans.x;
            inputs[2] = pipeTrans.y;

            // Generate Outputs
            var outputs = learner.Think(inputs);
            if (outputs[0] < outputs[1])
            {
                playerController.Jump();
            }

            // Check if time is up
            if (!(lifeTime < Time.time))
                return;
            // Set Learner to Not Alive
            learner.Alive = false;
        }

        /// <summary>
        /// Increase Fitness of Network
        /// </summary>
        /// <param name="fitness">float increase value</param>
        public void AddFitness(float fitness)
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