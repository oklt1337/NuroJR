using Controller;
using UnityEngine;

namespace Test_Environment.Scripts.Environment
{
    public class Dead : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Learner")) 
                return;
            var learner = other.GetComponent<Learner>();
            learner.Alive = false;
        }
    }
}
