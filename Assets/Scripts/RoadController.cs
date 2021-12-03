using System.Collections.Generic;
using UnityEngine;

public partial class RoadController : MonoBehaviour
{
    private const int DefaultRoadWidth = 3;
    private const int StartGroundWidth = 3;

    public GameObject crystalPrefab;
    public GameObject tilePrefab;
    public int segmentsLimit = 20;

    private Queue<GameObject> _segmentsQueue;
    private GameObject _lastAddedSegment;
    
    private float _tileWidth;
    private bool _crystalSpawnAvailable;
    private bool _isPlayedDead = true;
    private int _segmentsToSpawnUntilCrystal;
    private int _segmentsSpawningCounter;
    private int _blocksSpawnedCounter;
    private int _roadWidth;

    private int SegmentsSpawningCounter
    {
        get => _segmentsSpawningCounter;

        set
        {
            _segmentsSpawningCounter = value;
            if (_segmentsSpawningCounter > 4)
            {
                _segmentsSpawningCounter = 0;
                _blocksSpawnedCounter++;
                if (GameController.Instance.crystalsShouldSpawnRandomly)
                    _segmentsToSpawnUntilCrystal = Random.Range(0, 5);
                else
                    _segmentsToSpawnUntilCrystal = _blocksSpawnedCounter % 5;
            }
            if (_segmentsToSpawnUntilCrystal > 0)
            {
                _segmentsToSpawnUntilCrystal--;
                if (_segmentsToSpawnUntilCrystal == 0)
                    _crystalSpawnAvailable = true;
            }
        }
    }

    private void Start()
    {
        GenerateFirstSegment();
        Subscribe();
    }

    private void Update()
    {
        if (_isPlayedDead)
            return;

        TryToSpawnNewSegment();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void TryToSpawnNewSegment()
    {
        if (_segmentsQueue.Count <= segmentsLimit)
            GenerateCommonSegment();
    }

    private void GenerateFirstSegment()
    {
        var firstRoadSegment = new GameObject();
        var firstRoadSegmentScript = firstRoadSegment.AddComponent<RoadSegment>();

        _tileWidth = tilePrefab.transform.localScale.x;
        _roadWidth = DefaultRoadWidth - (int)GameController.Instance.difficulty;
        _segmentsQueue = new Queue<GameObject>(segmentsLimit);
        _segmentsQueue.Enqueue(firstRoadSegment);
        _lastAddedSegment = _segmentsQueue.Peek();
        _lastAddedSegment.transform.parent = transform;
        transform.position = new Vector3(0, 0, (_tileWidth * (StartGroundWidth + _roadWidth)) / 2);

        firstRoadSegmentScript.SetupRoadSegment(tilePrefab, transform.position, _roadWidth);
        firstRoadSegmentScript.OnDestroyCallback += OnRoadSegmentDestroyed;
        SegmentsSpawningCounter++;
    }

    private void GenerateCommonSegment()
    {
        var joinFromForward = Random.Range(1, 3) % 2 == 0;
        var offsetVec = (joinFromForward ? transform.forward : transform.right) * _roadWidth * _tileWidth;
        var roadSegment = new GameObject("RoadSegment");
        var roadSegmentScript = roadSegment.AddComponent<RoadSegment>();

        roadSegmentScript.SetupRoadSegment(tilePrefab, _lastAddedSegment.transform.position + offsetVec, _roadWidth);
        roadSegmentScript.OnDestroyCallback += OnRoadSegmentDestroyed;
        _segmentsQueue.Enqueue(roadSegment);
        _lastAddedSegment = roadSegment;
        roadSegment.transform.parent = transform;
        SegmentsSpawningCounter++;

        if (_crystalSpawnAvailable)
        {
            var crystal = Instantiate(crystalPrefab, roadSegment.transform);

            crystal.transform.Translate(Vector3.up * crystal.transform.localScale.y * 1.5f);
            _crystalSpawnAvailable = false;
        }
    }

    private void Subscribe()
    {
        GameController.Instance.OnGameStartedEvent += OnGameStarted;
        GameController.Instance.OnPlayerDiedEvent += OnPlayerDied;
    }

    private void Unsubscribe()
    {
        GameController.Instance.OnGameStartedEvent -= OnGameStarted;
        GameController.Instance.OnPlayerDiedEvent -= OnPlayerDied;
    }

    private void OnRoadSegmentDestroyed()
    {
        if (_segmentsQueue.Count > 0)
            _segmentsQueue?.Dequeue();
    }

    private void OnGameStarted()
    {
        GenerateFirstSegment();
        _isPlayedDead = false;
    }

    private void OnPlayerDied()
    {
        while (_segmentsQueue.Count > 0)
            Destroy(_segmentsQueue.Dequeue());
        _isPlayedDead = true;
        _segmentsToSpawnUntilCrystal = 0;
        _segmentsSpawningCounter = 0;
        _blocksSpawnedCounter = 0;
    }
}
