using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform ballTf;
    public float rotationSpeed = 1.5f;
    public float speed = 2f;

    public System.Action OnPlayerDiedEvent;

    private Transform _tf;
    private RaycastHit _raycastHit;
    private Vector3 _initialPos;
    private bool _isGoingForward;
    private bool _isDead;

    private void Start()
    {
        _tf = GetComponent<Transform>();
        _initialPos = transform.position;
        Subscribe();
    }

    private void Update()
    {
        if (_isDead)
            return;
        HandleMouse_LMBPressed();
        HandleMovement();
    }

    private void FixedUpdate()
    {
        if (_isDead)
            return;
        if (!Physics.Raycast(transform.position, -transform.up, out _raycastHit, 1f))
        {
            _isDead = true;
            _isGoingForward = true;
            OnPlayerDiedEvent?.Invoke();
        }
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void HandleMouse_LMBPressed()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        _isGoingForward = !_isGoingForward;
    }

    private void HandleMovement()
    {
        _tf.Translate((_isGoingForward ? transform.forward : transform.right) * speed * Time.deltaTime);
        ballTf.RotateAround(_tf.position, (_isGoingForward ? _tf.right : -_tf.forward), rotationSpeed);
    }

    private void Subscribe()
    {
        GameController.Instance.OnGameStartedEvent += OnGameStarted;
    }

    private void Unsubscribe()
    {
        GameController.Instance.OnGameStartedEvent -= OnGameStarted;
    }

    private void OnGameStarted()
    {
        transform.position = _initialPos;
        _isDead = false;
    }
}
