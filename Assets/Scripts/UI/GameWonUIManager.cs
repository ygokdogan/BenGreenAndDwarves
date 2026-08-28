using System;
using DG.Tweening;
using UI.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameWonUIManager : MonoBehaviour
    {
        public static GameWonUIManager Instance;

        public GameObject gameWonPanel;
        public GameObject gameplayUI;

        public TypewriterEffect typewriter;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void ShowGameWon()
        {
            gameplayUI.SetActive(false);
            if(GameOverUIManager.Instance) GameOverUIManager.Instance.gameOverPanel.SetActive(false);
            if(DayEndUIManager.Instance) DayEndUIManager.Instance.dayEndPanel.SetActive(false);
            
            gameWonPanel.transform.DOKill();
            gameWonPanel.transform.localScale = Vector3.zero;
            gameWonPanel.SetActive(true);
            gameWonPanel.transform.DOScale(Vector3.one, 0.45f).SetEase(Ease.OutBack);
        }
        
        
        public void RestartGame()
        {
            if (typewriter != null && typewriter.IsTyping)
            {
                typewriter.Skip();
            }

            if (gameWonPanel) gameWonPanel.SetActive(false);
            if (gameplayUI) gameplayUI.SetActive(true);
            TimeManager.Instance?.ResetTime();
            StatManager.Instance?.ResetStats();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void GoToMainMenu()
        {
            TimeManager.Instance?.ResetTime();
            StatManager.Instance?.ResetStats();
            SceneManager.LoadScene(0);
        }
    }
}