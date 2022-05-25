using System;
using Controller;
using UnityEngine;

namespace Test_Environment.Scripts.Pipes
{
    public class PipesMovementBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform top;
        [SerializeField] private Transform bottom;
        
        private const float MaxLifetime = 10f;
        private bool _init;
        private float _timer;
        
        public Action<PipesMovementBehaviour> OnDelete;

        #region Properties

        public Vector2 Top => top.position;
        public Vector2 Bottom => bottom.position;
        private float Speed { get; set; }

        #endregion

        #region Unity Methods

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

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize Pipe
        /// </summary>
        /// <param name="givenSpeed"></param>
        public void Initialize(float givenSpeed)
        {
            Speed = givenSpeed;
            _init = true;
        }

        /// <summary>
        /// Destroy Pipe
        /// Invoke OnDelete Event
        /// </summary>
        public void Delete()
        {
            OnDelete?.Invoke(this);
            Destroy(gameObject);
        }

        #endregion
    }
}
