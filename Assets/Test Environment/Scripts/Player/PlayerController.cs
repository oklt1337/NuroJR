using Controller;
using UnityEngine;

namespace Test_Environment.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float velocity;
        [SerializeField] private Rigidbody2D rb2D;
        [SerializeField] private Transform rayOrigin;
        [SerializeField] private Transform top;
        [SerializeField] private Transform bottom;
        [SerializeField] private LearnerSkeleton learner;
        [SerializeField] private float fitnessInterval = 0.1f;
        [SerializeField] private float fitnessToAdd = 0.01f;

        public Vector2 RayOrigin => rayOrigin.position;
        public Vector2 Top => top.position;
        public Vector2 Bottom => bottom.position;
        public Rigidbody2D Rigidbody2D => rb2D;

        private float timer;

        /// <summary>
        /// Makes Player Jump.
        /// </summary>
        public void Jump()
        {
            rb2D.velocity = Vector2.up * velocity;
        }
        
        private void FixedUpdate()
        {
            if (timer >= fitnessInterval)
            {
                learner.AddFitness(fitnessToAdd);
                timer = 0;
            }
            else
            {
                timer += Time.fixedDeltaTime;
            }
        }
    }
}
