using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public CanvasGroup gameOverGroup;
    public GameObject startButton;
    public TMP_Text crystalsCounter;

    private void Start()
    {
        crystalsCounter.text = "0";
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubcribe();
    }

    public void ButtonClick_StartButton()
    {
        gameOverGroup.alpha = 0f;
        gameOverGroup.blocksRaycasts = false;
        gameOverGroup.interactable = false;
        startButton.SetActive(false);
        GameController.Instance.StartGame();
    }

    private void Subscribe()
    {
        GameController.Instance.OnCrystalsCountChangedEvent += OnCrystalsCountChanged;
        GameController.Instance.OnPlayerDiedEvent += OnPlayerDied;
    }

    private void Unsubcribe()
    {
        GameController.Instance.OnCrystalsCountChangedEvent -= OnCrystalsCountChanged;
        GameController.Instance.OnPlayerDiedEvent -= OnPlayerDied;
    }

    private void OnCrystalsCountChanged(int newAmount)
    {
        crystalsCounter.text = $"{newAmount}";
    }

    private void OnPlayerDied()
    {
        gameOverGroup.alpha = 1f;
        gameOverGroup.blocksRaycasts = true;
        gameOverGroup.interactable = true;
    }
}
