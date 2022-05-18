using Controller;
using UnityEngine;

namespace Test_Environment.Scripts.Pipes
{
    public class PipesMovementBehaviour : MonoBehaviour
    {
        private const float MaxLifetime = 10f;
        private bool _init;
        private float _timer;

        private float Speed { get; set; }

        private void Update()
        {
            if (!_init)
                return;
            if (_timer > MaxLifetime)
                Destroy(gameObject);

            _timer += Time.deltaTime;
            transform.position += Vector3.left * (Speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Learner")) 
                return;

            var learner = other.GetComponent<Learner>();
            learner.Alive = false;
        }

        public void Initialize(float givenSpeed)
        {
            Speed = givenSpeed;
            _init = true;
        }
    }
}
