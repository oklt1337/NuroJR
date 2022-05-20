using Controller;
using UnityEngine;

namespace Test_Environment.Scripts.Pipes
{
    public class Done : MonoBehaviour
    {
        [SerializeField] private PipesMovementBehaviour pipesMovementBehaviour;
        [SerializeField] private float reward = 1f;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Learner")) 
                return;
            var learner = other.GetComponent<LearnerSkeleton>();
            learner.AddFitness(reward);
            pipesMovementBehaviour.Delete();
        }
    }
}
