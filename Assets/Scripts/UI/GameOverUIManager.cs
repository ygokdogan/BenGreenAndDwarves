using Stats;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameOverUIManager : MonoBehaviour
    {
        public static GameOverUIManager Instance;

        [Header("UI References")]
        public GameObject gameOverPanel;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI reasonText;
        public TextMeshProUGUI daysSurvivedText;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void ShowGameOver(StatType stat, bool isZero)
        {
            if (DayEndUIManager.Instance != null && DayEndUIManager.Instance.dayEndPanel != null)
            {
                DayEndUIManager.Instance.dayEndPanel.SetActive(false);
            }

            if (PendingEffectPopup.Instance != null && PendingEffectPopup.Instance.popupRoot != null)
            {
                PendingEffectPopup.Instance.popupRoot.SetActive(false);
            }

            if (gameOverPanel) gameOverPanel.SetActive(true);

            if (daysSurvivedText && TimeManager.Instance != null)
            {
                daysSurvivedText.text = $"Survived: {TimeManager.Instance.CurrentDay} Days";
            }

            (string title, string reason) = GetGameOverReason(stat, isZero);
            if (titleText) titleText.text = title;
            if (reasonText) reasonText.text = reason;
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

        public void RestartGame()
        {
            if (gameOverPanel) gameOverPanel.SetActive(false);
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
