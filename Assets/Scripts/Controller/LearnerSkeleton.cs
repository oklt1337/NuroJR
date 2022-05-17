using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(Learner))]
    public class LearnerSkeleton : MonoBehaviour
    {
        private Learner learner;
        public float lifeTime = 10;

        private void Start()
        {
            learner = GetComponent<Learner>();
            lifeTime += Time.time; // This is to give all the learners a maximum life time
        }

        private void AddFitness(float fitness)
        {
            if (learner != null) // Checks that there actually is a learner script assigned to the learner variable
            {
                learner.fitness += fitness; // Adds a float value to the fitness every time it does something correct (Such as crossing a checkpoint or surviving for some time)
            }
        }

        private void FixedUpdate()
        {
            if (learner.alive)
            {
                float[] inputs = new float[3]; // Creates array of size 3 to hold 3 inputs. This can be changed to fit whatever amount of inputs you want. (REMEMBER TO ALSO CHANGE THE AMOUNT OF INPUT NEURONS TO MATCH THE INPUTS ARRAY)

                inputs[0] = 1; // This just assigns a 1 to all inputs for the example but here you should add what the inputs would actually be (such as speed, raycast distances, directions, etc)
                inputs[1] = 1;
                inputs[2] = 1;

                float[] outputs = learner.Think(inputs); // This calls the think function in the learner script which passes the inputs to the neural network and returns the outputs (REMEMBER TO ADJUST THESE FOR WHAT YOU NEED IN THE NEURAL NETWORK)
                transform.position += new Vector3(outputs[0], 0, outputs[1]) * Time.fixedDeltaTime; // This is an example of how you could use the output (the output is a float between -1 and 1)

                if (lifeTime < Time.time)
                {
                    learner.alive = false; // This sets the learner to not alive. When all the learners are not alive the manager will automatically create the next generation
                }
            }
        }
    }
}