using Controller;
using UnityEngine;

namespace Example.Scripts.Environment
{
    public class Dead : MonoBehaviour
    {
        /// <summary>
        /// Sets Learner Alive = false on Enter
        /// </summary>
        /// <param name="other">Collider2D</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Learner")) 
                return;
            var learner = other.GetComponent<Learner>();
            learner.Alive = false;
        }
    }
}
