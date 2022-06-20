using System.Linq;
using Controller;
using Example.Scripts.Pipes;
using UnityEngine;

namespace Example.Scripts.Player
{
    public class TestSkeleton : LearnerSkeleton
    {
        [SerializeField] private PlayerController playerController;

        /// <summary>
        /// Process Outputs
        /// </summary>
        /// <param name="outputs">float[]</param>
        protected override void Action(float[] outputs)
        {
            if (outputs[0] < outputs[1])
                playerController.Jump();
        }

        /// <summary>
        /// generate The Inputs for the Neural Network
        /// </summary>
        /// <returns>float[]</returns>
        protected override float[] GenerateInputs()
        {
            var hit = Physics2D.Raycast(playerController.RayOrigin, Vector2.right);
            var firstPositionTop = new Vector2(playerController.Top.x, PipeManager.Instance.pipes.First().Top.y);
            var firstPositionBottom =
                new Vector2(playerController.Bottom.x, PipeManager.Instance.pipes.First().Bottom.y);

            var distanceVerticalTop = Vector2.Distance(firstPositionTop, playerController.Top); // Distance Learner and Pipes TopOpening 
            var distanceVerticalBottom = Vector2.Distance(firstPositionBottom, playerController.Bottom); // Distance Learner and Pipes BottomOpening 
            
            float distanceHorizontal = 0;
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("PipeDistance"))
                {
                    distanceHorizontal = hit.distance; // Distance Learner and left of Pipe
                }
                    
            }

            // Create Inputs
            var inputs = new float[4]; // make sure your Inputs[] is the same length as count of the neurons in the neural network
            // Set Inputs
            inputs[0] = playerController.Rigidbody2D.velocity.y;
            inputs[1] = distanceHorizontal;
            inputs[2] = distanceVerticalTop;
            inputs[3] = distanceVerticalBottom;

            return inputs;
        }
    }
}