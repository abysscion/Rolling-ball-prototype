using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public GameDifficulty difficulty;
    public bool crystalsShouldSpawnRandomly;

    public System.Action<int> OnCrystalsCountChangedEvent;
    public System.Action OnGameStartedEvent;
    public System.Action OnPlayerDiedEvent;

    private GameObject _player;
    private int _crystalsCount;

    public int CrystalsCount
    {
        get => _crystalsCount;
        set
        {
            _crystalsCount = value;
            OnCrystalsCountChangedEvent?.Invoke(_crystalsCount);
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        Time.timeScale = 0f;
        Application.targetFrameRate = 60;
        _player = GameObject.FindGameObjectWithTag("Player");
        _player.GetComponent<PlayerController>().OnPlayerDiedEvent += OnPlayerDied;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        CrystalsCount = 0;
        OnGameStartedEvent?.Invoke();
    }

    private void OnPlayerDied()
    {
        Time.timeScale = 0f;
        OnPlayerDiedEvent?.Invoke();
    }
}
