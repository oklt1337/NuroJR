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
        [SerializeField] public List<PipesMovementBehaviour> pipes = new();

        private float _timer;
        private bool _spawn;
        private int pipesindex;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }

            Instance = this;

            NetworkHandler.OnNewGeneration += Restart;
            InstantiatePipe();
        }

        private void ResetValues()
        {
            pipesindex = 0;
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
            var pos = spawnPos.position + Vector3.down; //+ new Vector3(0, Random.Range(minHeight, maxHeight), 0);
            var pipesMovementBehaviour = Instantiate(prefab, pos, Quaternion.identity, parent)
                .GetComponent<PipesMovementBehaviour>();

            pipesMovementBehaviour.Initialize(speed);
            pipesMovementBehaviour.OnDelete += behaviour => pipes.Remove(behaviour);
            pipes.Add(pipesMovementBehaviour);
            pipesindex++;
        }

        private void Restart()
        {
            for (var i = pipes.Count - 1; i >= 0; i--)
            {
                if (pipes[i] != null)
                {
                    Destroy(pipes[i].gameObject);
                }
            }

            pipes.Clear();
            ResetValues();
        }
    }
}