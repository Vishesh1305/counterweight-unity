using Counterweight.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Counterweight.UI
{
    public class WinScreen : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button playAgainButton;

        private GameManager gameManager;

        private void Awake()
        {
            if (panel == null || playAgainButton == null)
            {
                Debug.LogError($"{name}: WinScreen needs Panel and Play Again Button assigned.", this);
                enabled = false;
                return;
            }

            panel.SetActive(false);
        }

        private void OnEnable()
        {
            if (playAgainButton != null)
                playAgainButton.onClick.AddListener(RestartLevel);
        }

        private void OnDisable()
        {
            if (playAgainButton != null)
                playAgainButton.onClick.RemoveListener(RestartLevel);
        }

        private void Start()
        {
            gameManager = GameManager.Instance;
            if (gameManager == null)
            {
                Debug.LogError($"{name}: no GameManager in the scene.", this);
                enabled = false;
                return;
            }

            gameManager.Won += Show;
        }

        private void OnDestroy()
        {
            if (gameManager != null)
                gameManager.Won -= Show;
        }

        private void Show()
        {
            panel.SetActive(true);

            // Selecting the button lets Enter or Space press it, so the win screen works without a mouse.
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(playAgainButton.gameObject);
        }

        private void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}