using System.Linq;
using Controller;
using Example.Scripts.Pipes;
using UnityEngine;

namespace Example.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float velocity;
        [SerializeField] private Rigidbody2D rb2D;
        [SerializeField] private Transform rayOrigin;
        [SerializeField] private Transform top;
        [SerializeField] private Transform bottom;
        [SerializeField] private LearnerSkeleton learner;
        [SerializeField] private float fitnessInterval = 1f;
        [SerializeField] private float fitnessToAdd = 1f;
        [SerializeField] private float fitnessMultiplier = 2f;

        private float timer;

        #region Proiperties

        public Vector2 RayOrigin => rayOrigin.position;
        public Vector2 Top => top.position;
        public Vector2 Bottom => bottom.position;
        public Rigidbody2D Rigidbody2D => rb2D;

        #endregion

        #region Unity Methods

        private void FixedUpdate()
        {
            if (timer >= fitnessInterval)
            {
                var firstPipe = PipeManager.Instance.pipes.First();
                
                if (transform.position.y < firstPipe.Top.y && 
                    transform.position.y > firstPipe.Bottom.y)
                {
                    learner.AddFitness(fitnessToAdd * fitnessMultiplier);
                }
                else
                {
                    var position = transform.position;
                    var distanceTop = Vector2.Distance(position, new Vector2(position.x, firstPipe.Top.y));
                    var distanceBottom = Vector2.Distance(position, new Vector2(position.x, firstPipe.Top.y));
                    var value = distanceTop > distanceBottom ? distanceBottom : distanceTop;

                    learner.AddFitness(fitnessToAdd / value);
                }
                timer = 0;
            }
            else
            {
                timer += Time.fixedDeltaTime;
            }
        }

        #endregion

        /// <summary>
        /// Makes Player Jump.
        /// </summary>
        public void Jump()
        {
            rb2D.velocity = Vector2.up * velocity;
        }
    }
}
