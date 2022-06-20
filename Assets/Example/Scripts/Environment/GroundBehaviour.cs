using Controller;
using UnityEngine;

namespace Example.Scripts.Environment
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class GroundBehaviour : MonoBehaviour
    {
        [SerializeField] private float speed;

        private Vector3 _startPos;
        private float _resetPos;
        private float _currenSpeed;

        #region Unity Methods

        private void Awake()
        {
            _startPos = transform.position;
        }

        private void Start()
        {
            ResetValues();
        }

        private void Update()
        {
            CheckRest();
            Move();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Learner")) 
                return;
            var learner = other.GetComponent<Learner>();
            learner.Alive = false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Moves Ground to left
        /// </summary>
        private void Move()
        {
            transform.position += Vector3.left * (_currenSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Resets Position
        /// </summary>
        private void CheckRest()
        {
            if (!(transform.position.x < _resetPos)) 
                return;
            
            var myTransform = transform;
            myTransform.position = new Vector3(_startPos.x, _startPos.y, myTransform.position.z);
        }

        /// <summary>
        /// Reset Speed and Position
        /// </summary>
        private void ResetValues()
        {
            transform.position = _startPos;
            _currenSpeed = speed;
        }

        #endregion
    }
}