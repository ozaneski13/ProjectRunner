using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    [Header("Time Scale")]
    [SerializeField] private float _timeScale = 0.1f;

    private bool _isButtonAlreadyPressed = false;

    private void OnEnable()
    {
        Time.timeScale = _timeScale;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    public void RetryLevel()
    {
        if (_isButtonAlreadyPressed)
            return;

        _isButtonAlreadyPressed = true;

        Time.timeScale = 1f;

        FadeUI.Instance.FadeTo(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        if (_isButtonAlreadyPressed)
            return;

        _isButtonAlreadyPressed = true;

        Time.timeScale = 1f;

        FadeUI.Instance.FadeTo(0);
    }
}