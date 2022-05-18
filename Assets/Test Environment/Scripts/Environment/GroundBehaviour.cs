using System;
using Controller;
using UnityEngine;

namespace Test_Environment.Scripts.Environment
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class GroundBehaviour : MonoBehaviour
    {
        [SerializeField] private float speed;

        private Vector3 _startPos;
        private float _restPos;
        private float _currenSpeed;

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

        private void Move()
        {
            transform.position += Vector3.left * (_currenSpeed * Time.deltaTime);
        }

        private void CheckRest()
        {
            if (!(transform.position.x < _restPos))
                return;
            var myTransform = transform;
            myTransform.position = new Vector3(_startPos.x, _startPos.y, myTransform.position.z);
        }

        private void ResetValues()
        {
            transform.position = _startPos;
            _currenSpeed = speed;
        }
    }
}