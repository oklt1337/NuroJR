using System.Collections.Generic;
using Controller;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Example.Scripts.Pipes
{
    public class PipeManager : MonoBehaviour
    {
        public static PipeManager Instance;

        [SerializeField, Tooltip("If false all pipes will have different spawn Heights")]
        private bool pipesAtSameHeight;
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

        #region Unity Methods

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }

            Instance = this;

            NetworkHandler.OnNewGenerationCreated += Restart;
            InstantiatePipe();
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

        #endregion

        #region Private Methods

        /// <summary>
        /// Instantiate Pipe at Same Level
        /// </summary>
        private void InstantiatePipe()
        {
            Vector3 pos;
            var position = spawnPos.position;
            if (pipesAtSameHeight)
                pos = position + Vector3.down;
            else
                pos = position + Vector3.down + new Vector3(0, Random.Range(minHeight, maxHeight), 0);

            var pipesMovementBehaviour = Instantiate(prefab, pos, Quaternion.identity, parent)
                .GetComponent<PipesMovementBehaviour>();
            pipesMovementBehaviour.Initialize(speed);
            pipesMovementBehaviour.OnDelete += behaviour => pipes.Remove(behaviour);
            pipes.Add(pipesMovementBehaviour);
        }

        /// <summary>
        /// Restart
        /// Destroy all pipes and ResetValues
        /// </summary>
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

        /// <summary>
        /// Reset Time
        /// </summary>
        private void ResetValues()
        {
            _timer = spawnTime;
        }

        #endregion
    }
}