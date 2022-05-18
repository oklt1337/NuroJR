using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Test_Environment.Scripts.Pipes
{
    public class PipeManager : MonoBehaviour
    {
        public static PipeManager Instance;
        
        [SerializeField] private float spawnTime;
        [SerializeField] private float minHeight;
        [SerializeField] private float maxHeight;
        [SerializeField] private GameObject prefab;
        [SerializeField] private Transform parent;
        [SerializeField] private Transform spawnPos;
        [SerializeField] private float speed;

        private readonly List<PipesMovementBehaviour> pipes = new();

        private float _timer;
        private bool _spawn;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }

            Instance = this;
            
            NetworkHandler.OnNewGeneration += Restart;
        }

        private void Start()
        {
            ResetValues();
        }

        private void ResetValues()
        {
            _timer = spawnTime;
        }

        private void Update()
        {
            if (_timer > spawnTime)
            {
                _timer = 0;

                InstantiatePipe();
            }
            else
            {
                _timer += Time.deltaTime;
            }
        }

        private void InstantiatePipe()
        {
            var pos = spawnPos.position;// + new Vector3(0, Random.Range(minHeight, maxHeight), 0);
            var pipesMovementBehaviour = Instantiate(prefab, pos, Quaternion.identity, parent)
                .GetComponent<PipesMovementBehaviour>();
            pipesMovementBehaviour.Initialize(speed);
            
            pipes.Add(pipesMovementBehaviour);
        }

        private void Restart()
        {
            for (var i = pipes.Count - 1; i >= 0; i--)
            {
                Destroy(pipes[i].gameObject);
            }
            pipes.Clear();
            ResetValues();
        }

        public Vector2 GetFirstPipe()
        {
            if (pipes != null && pipes.Any())
                return pipes.First().transform.position;
            return new Vector2(0, 0);
        }
    }
}
