using UnityEngine;
using Random = UnityEngine.Random;

namespace Test_Environment.Scripts.Pipes
{
    public class PipeManager : MonoBehaviour
    {
        [SerializeField] private float spawnTime;
        [SerializeField] private float minHeight;
        [SerializeField] private float maxHeight;
        [SerializeField] private GameObject prefab;
        [SerializeField] private Transform parent;
        [SerializeField] private Transform spawnPos;
        [SerializeField] private float speed;
        
        private float _timer;
        private bool _spawn;

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
            var pos = spawnPos.position + new Vector3(0, Random.Range(minHeight, maxHeight), 0);
            var pipesMovementBehaviour = Instantiate(prefab, pos, Quaternion.identity, parent)
                .GetComponent<PipesMovementBehaviour>();
            pipesMovementBehaviour.Initialize(speed);
        }
    }
}
