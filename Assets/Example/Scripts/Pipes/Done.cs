using Controller;
using UnityEngine;

namespace Example.Scripts.Pipes
{
    public class Done : MonoBehaviour
    {
        [SerializeField] private PipesMovementBehaviour pipesMovementBehaviour;
        [SerializeField] private float reward = 10f;
        
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
