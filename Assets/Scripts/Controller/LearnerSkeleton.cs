using System;
using System.Linq;
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

        private float[] GenerateInputs()
        {
            var hit = Physics2D.Raycast(playerController.RayOrigin, Vector2.right);
            var firstPositionTop = new Vector2(playerController.Top.x, PipeManager.Instance.pipes.First().Top.y);
            var firstPositionBottom =
                new Vector2(playerController.Bottom.x, PipeManager.Instance.pipes.First().Bottom.y);

            var distanceVerticalTop = Vector2.Distance(firstPositionTop, playerController.Top);
            var distanceVerticalBottom = Vector2.Distance(firstPositionBottom, playerController.Bottom);
            
            float distanceHorizontal = 0;
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("PipeDistance"))
                {
                    distanceHorizontal = hit.distance;
                }
                    
            }

            // Create Inputs
            var inputs = new float[4];
            // Set Inputs
            inputs[0] = playerController.Rigidbody2D.velocity.y;
            inputs[1] = distanceHorizontal;
            inputs[2] = distanceVerticalTop;
            inputs[3] = distanceVerticalBottom;

            return inputs;
        }

        private void FixedUpdate()
        {
            // Check if Learner is alive
            if (!learner.Alive)
                return;

            // Generate Outputs
            var outputs = learner.Think(GenerateInputs());
            if (outputs[0] < outputs[1])
                playerController.Jump();
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
            if (!learner.Alive)
                return;
            // Add float to fitness if does sth correct make sure value is positive
            learner.Fitness += Math.Abs(fitness);
        }

        #endregion
    }
}