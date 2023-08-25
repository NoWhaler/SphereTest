using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main.Scripts.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Canvas _winCanvas;

        [SerializeField] private Canvas _loseCanvas;

        private MainSphere _mainSphere;

        private void Awake()
        {
            _mainSphere = FindObjectOfType<MainSphere>();
        }

        private void OnEnable()
        {
            _mainSphere.onWinLevel += ShowWinCanvas;
            _mainSphere.onLoseLevel += ShowLoseCanvas;
        }

        private void OnDisable()
        {
            _mainSphere.onWinLevel -= ShowWinCanvas;
            _mainSphere.onLoseLevel -= ShowLoseCanvas;
        }

        private void ShowWinCanvas()
        {
            _winCanvas.enabled = true;
        }
        
        private void ShowLoseCanvas()
        {
            _loseCanvas.enabled = true;
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}