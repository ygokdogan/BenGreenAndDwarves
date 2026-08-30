using TMPro;
using UI.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Effects;
using UnityEngine.UI;

namespace UI
{
    public class GameOverUIManager : MonoBehaviour
    {
        public static GameOverUIManager Instance;

        [Header("UI References")]
        public GameObject gameplayUI;
        public GameObject gameOverPanel;
        public Image statusImage;
        public TextMeshProUGUI reasonText;
        public TextMeshProUGUI daysSurvivedText;
        public TypewriterEffect reasonTypewriter;

        [Tooltip("0-1 Cash, 2-3 Health, 4-5 Happiness, 6-7 Storage")]
        public Sprite[] statusImages;

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
            
            (string title, string reason, Sprite image) = GetGameOverReason(stat, isZero);


            if (image)
            {
                statusImage.sprite = image;
                statusImage.gameObject.SetActive(true);
            }
            else
            {
                statusImage.gameObject.SetActive(false);
            }
            
            if (reasonTypewriter != null)
            {
                reasonTypewriter.Play(reason);
            }
            else if (reasonText)
            {
                reasonText.text = reason;
            }
        }

        private (string title, string reason, Sprite image) GetGameOverReason(StatType stat, bool isZero)
        {
            switch (stat)
            {
                case StatType.Cash:
                    return isZero
                        ? ("BANKRUPT", "You went so broke that your wallet physically rejected you. You couldn't even afford to breathe the air, so you just starved to death on the floor.", statusImages[0])
                        : ("GREED TARGET", "Your bank account got so fat the government glitched. You were arrested for aggressive money laundering before you could even buy a yacht.", statusImages[1]);

                case StatType.Health:
                    return isZero
                        ? ("COLLAPSED", "Your biology just straight-up gave up. You caught a mild sniffle from a dwarf, immediately collapsed into bed, and expired three minutes later.", statusImages[2])
                        : ("OVEREXERTED", "Your body reached peak human perfection, realized it had no more challenges, and initiated self-destruct. You died of being way too healthy.", statusImages[3]);

                case StatType.Happiness:
                    return isZero
                        ? ("BURNOUT", "You achieved terminal depression. You locked the door, ignored the world, and slowly dissolved into your couch cushions until society completely forgot you existed.", statusImages[4])
                        : ("DELUSION", "Your dopamine receptors completely fried themselves. You stared at a blank wall, laughed so hard you triggered a joy-induced seizure, and died with a terrifyingly huge smile.", statusImages[5]);

                case StatType.Storage:
                    return isZero
                        ? ("EMPTY WAREHOUSE", "You sold every single thing you owned. The house emptied, the furniture vanished, and eventually, so did your will to live. At least there’s plenty of legroom for your corpse.", statusImages[6])
                        : ("WAREHOUSE EXPLOSION", "Your hoarding reached critical mass. You stepped on a rogue Lego in the dark, lost your balance, and were violently crushed under four tons of useless garbage.", statusImages[7]);

                default:
                    return ("GAME OVER", "Your stats fell out of balance.", null);
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
