using TMPro;
using UI.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Effects;

namespace UI
{
    public class GameOverUIManager : MonoBehaviour
    {
        public static GameOverUIManager Instance;

        [Header("UI References")]
        public GameObject gameplayUI;
        public GameObject gameOverPanel;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI reasonText;
        public TextMeshProUGUI daysSurvivedText;
        public TypewriterEffect reasonTypewriter;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (reasonText != null && reasonTypewriter == null)
            {
                reasonTypewriter = reasonText.GetComponent<TypewriterEffect>();
                if (reasonTypewriter == null) reasonTypewriter = reasonText.gameObject.AddComponent<TypewriterEffect>();
            }
        }

        public void ShowGameOver(StatType stat, bool isZero)
        {
            if (DayEndUIManager.Instance != null && DayEndUIManager.Instance.dayEndPanel != null)
            {
                DayEndUIManager.Instance.dayEndPanel.SetActive(false);
            }


            if (HUDManager.Instance != null && HUDManager.Instance.bars != null)
            {
                HUDManager.Instance.bars.gameObject.SetActive(false);
            }
            if (PendingEffectPopup.Instance != null && PendingEffectPopup.Instance.popupRoot != null)
            {
                PendingEffectPopup.Instance.popupRoot.SetActive(false);
            }

            if (gameplayUI)
            {
                gameplayUI.SetActive(false);
            }

            if (gameOverPanel != null)
            {
                gameOverPanel.transform.DOKill();
                gameOverPanel.transform.localScale = Vector3.zero;
                gameOverPanel.SetActive(true);
                gameOverPanel.transform.DOScale(Vector3.one, 0.45f).SetEase(Ease.OutBack);
            }
            if (reasonText) reasonText.gameObject.SetActive(true);

            if (daysSurvivedText && TimeManager.Instance != null)
            {
                daysSurvivedText.text = $"Survived: {TimeManager.Instance.CurrentDay - 1} Days";
            }

            (string title, string reason) = GetGameOverReason(stat, isZero);
            if (titleText) titleText.text = title;

            if (reasonTypewriter != null)
            {
                reasonTypewriter.Play(reason);
            }
            else if (reasonText)
            {
                reasonText.text = reason;
            }
        }

        private (string title, string reason) GetGameOverReason(StatType stat, bool isZero)
        {
            switch (stat)
            {
                case StatType.Cash:
                    return isZero
                        ? ("BANKRUPT", "You ran out of money and couldn't pay your debts.")
                        : ("GREED TARGET", "Excessive wealth attracted tax auditors and thieves who seized everything.");

                case StatType.Health:
                    return isZero
                        ? ("COLLAPSED", "Severe exhaustion sent you to the hospital.")
                        : ("OVEREXERTED", "Extreme hyper-fixation crashed your physical stamina.");

                case StatType.Happiness:
                    return isZero
                        ? ("BURNOUT", "Depression overwhelmed you. You gave up running the shop.")
                        : ("DELUSION", "Unchecked mania led to reckless and catastrophic decisions.");

                case StatType.Storage:
                    return isZero
                        ? ("EMPTY WAREHOUSE", "Your storage emptied completely, leaving no inventory to sell.")
                        : ("WAREHOUSE EXPLOSION", "Overstocked inventory crushed your building structure!");

                default:
                    return ("GAME OVER", "Your stats fell out of balance.");
            }
        }

        public void SkipReasonText()
        {
            if (reasonTypewriter != null && reasonTypewriter.IsTyping)
            {
                reasonTypewriter.Skip();
            }
        }

        public void RestartGame()
        {
            AudioManager.Instance?.StopChallengeSFX();

            if (reasonTypewriter != null && reasonTypewriter.IsTyping)
            {
                reasonTypewriter.Skip();
            }

            if (gameOverPanel) gameOverPanel.SetActive(false);
            if (gameplayUI) gameplayUI.SetActive(true);
            TimeManager.Instance?.ResetTime();
            StatManager.Instance?.ResetStats();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void GoToMainMenu()
        {
            AudioManager.Instance?.StopChallengeSFX();
            TimeManager.Instance?.ResetTime();
            StatManager.Instance?.ResetStats();
            SceneManager.LoadScene(0);
        }
    }
}
