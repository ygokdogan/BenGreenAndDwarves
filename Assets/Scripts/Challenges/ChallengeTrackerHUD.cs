using Challenges;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ChallengeTrackerHUD : MonoBehaviour
    {
        [Header("Existing tracker text")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI objectiveText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private TextMeshProUGUI floatingResultText;
        
        [Header("Result feedback")]
        [SerializeField, Range(0f, 1f)] private float resolvedContentAlpha = 0.45f;
        [SerializeField] private Color completeColor = new Color(0.45f, 0.9f, 0.55f);
        [SerializeField] private Color failedColor = new Color(0.95f, 0.38f, 0.35f);

        private CanvasGroup canvasGroup;
        private RectTransform trackerRect;
        private Vector2 initialPosition;
        private bool isSubscribed;
        private bool isSubscribedToTime;

        public AudioClip completeClip;
        public AudioClip failedClip;

        private void Awake()
        {
            trackerRect = transform as RectTransform;
            if (trackerRect != null) initialPosition = trackerRect.anchoredPosition;

            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

            FindTextReferences();
        }

        private void OnEnable()
        {
            Subscribe();
            SubscribeToTime();
            Refresh();
        }

        private void Start()
        {
            Subscribe();
            SubscribeToTime();
            Refresh();
        }

        private void OnDisable()
        {
            Unsubscribe();
            UnsubscribeFromTime();
        }

        private void Update()
        {
            bool shouldShow = GameFlowManager.Instance != null &&
                              GameFlowManager.Instance.CurrentState != GameFlowState.ChallengeSelection &&
                              GameFlowManager.Instance.CurrentState != GameFlowState.GameOver &&
                              GameFlowManager.Instance.CurrentState != GameFlowState.GameWon;

            if (canvasGroup != null)
                canvasGroup.alpha = shouldShow ? 1f : 0f;
        }

        private void Subscribe()
        {
            if (isSubscribed || ChallengeManager.Instance == null) return;

            ChallengeManager.Instance.OnChallengeSelected += HandleChallengeSelected;
            ChallengeManager.Instance.OnChallengeProgressChanged += Refresh;
            ChallengeManager.Instance.OnChallengeResolved += HandleChallengeResolved;
            isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!isSubscribed || ChallengeManager.Instance == null) return;

            ChallengeManager.Instance.OnChallengeSelected -= HandleChallengeSelected;
            ChallengeManager.Instance.OnChallengeProgressChanged -= Refresh;
            ChallengeManager.Instance.OnChallengeResolved -= HandleChallengeResolved;
            isSubscribed = false;
        }

        private void SubscribeToTime()
        {
            if (isSubscribedToTime || TimeManager.Instance == null) return;

            TimeManager.Instance.OnDayChanged += HandleDayChanged;
            isSubscribedToTime = true;
        }

        private void UnsubscribeFromTime()
        {
            if (!isSubscribedToTime || TimeManager.Instance == null) return;

            TimeManager.Instance.OnDayChanged -= HandleDayChanged;
            isSubscribedToTime = false;
        }

        private void HandleDayChanged(int _) => Refresh();

        private void HandleChallengeSelected(ChallengeData challenge)
        {
            SetResolvedAppearance(false, false);
            Refresh();
        }

        private void HandleChallengeResolved(bool completed)
        {
            Refresh();
            SetResolvedAppearance(true, completed);

            if (!completed && GameFlowManager.Instance != null &&
                GameFlowManager.Instance.CurrentState == GameFlowState.GameOver)
                return;

            PlayResolutionFeedback(completed);
        }

        private void Refresh()
        {
            ChallengeManager manager = ChallengeManager.Instance;
            ChallengeData challenge = manager != null ? manager.activeChallenge : null;
            if (challenge == null) return;

            if (titleText != null) titleText.text = challenge.displayName;
            if (objectiveText != null) objectiveText.text = FormatObjective(challenge);

            if (progressText != null)
                progressText.text = FormatProgress(challenge);
        }

        private void FindTextReferences()
        {
            TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (TextMeshProUGUI text in texts)
            {
                if (text.gameObject.name == "Challenge Title") titleText = text;
                else if (text.gameObject.name == "Definition Text") objectiveText = text;
                else if (text.gameObject.name == "Progress") progressText = text;
                else if (text.gameObject.name == "Challenge Result") resultText = text;
            }
        }

        private void SetResolvedAppearance(bool isResolved, bool completed)
        {
            SetTextAlpha(titleText, isResolved ? resolvedContentAlpha : 1f);
            SetTextAlpha(objectiveText, isResolved ? resolvedContentAlpha : 1f);
            SetTextAlpha(progressText, isResolved ? resolvedContentAlpha : 1f);

            if (resultText == null) return;

            resultText.gameObject.SetActive(isResolved);
            if (!isResolved) return;

            resultText.text = completed ? "CHALLENGE COMPLETE" : "CHALLENGE FAILED";
            resultText.color = completed ? completeColor : failedColor;
        }

        private void PlayResolutionFeedback(bool completed)
        {
            if (trackerRect != null)
            {
                trackerRect.DOKill();
                trackerRect.anchoredPosition = initialPosition;
                trackerRect.DOShakeAnchorPos(completed ? 0.22f : 0.3f, completed ? 7f : 11f, completed ? 14 : 20)
                    .OnComplete(() => trackerRect.anchoredPosition = initialPosition);
            }

            if (progressText == null) return;

            floatingResultText.DOKill();
            floatingResultText.gameObject.SetActive(true);
            floatingResultText.text = completed ? "CHALLENGE COMPLETE" : "CHALLENGE FAILED";
            floatingResultText.color = completed ? completeColor : failedColor;

            AudioManager.Instance.PlayChallengeSFX(completed ? completeClip : failedClip);
            
            RectTransform floatingRect = floatingResultText.rectTransform;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(floatingRect.DOAnchorPosY(90f, 0.75f).SetEase(Ease.OutCubic));
            sequence.Join(floatingResultText.DOFade(0f, 0.75f));
            sequence.OnComplete(() => floatingResultText.gameObject.SetActive(false));
        }

        private static void SetTextAlpha(TextMeshProUGUI text, float alpha)
        {
            if (text == null) return;

            Color color = text.color;
            color.a = alpha;
            text.color = color;
        }

        private static string FormatObjective(ChallengeData challenge)
        {
            return challenge.objectiveType switch
            {
                ObjectiveType.SurviveUntilEnd => "Survive until the final day",
                ObjectiveType.KeepStatInRangeAtEnd => $"Finish with {challenge.trackedStat} between {challenge.minimumValue}-{challenge.maximumValue}",
                ObjectiveType.KeepStatInRangeAlways => $"Keep {challenge.trackedStat} between {challenge.minimumValue}-{challenge.maximumValue}",
                ObjectiveType.KeepStatAboveAlways => $"Keep {challenge.trackedStat} at {challenge.minimumValue} or above",
                ObjectiveType.KeepStatBelowAlways => $"Keep {challenge.trackedStat} at {challenge.maximumValue} or below",
                ObjectiveType.KeepStatAboveAtEnd => $"Finish with {challenge.trackedStat} at {challenge.minimumValue} or above",
                ObjectiveType.KeepStatBelowAtEnd => $"Finish with {challenge.trackedStat} at {challenge.maximumValue} or below",
                ObjectiveType.AcceptOffers => $"Accept {challenge.requiredCount} offers{(challenge.countScope == CountScope.PerDay ? " each day" : string.Empty)}",
                ObjectiveType.RejectOffers => $"Reject {challenge.requiredCount} offers{(challenge.countScope == CountScope.PerDay ? " each day" : string.Empty)}",
                _ => string.Empty
            };
        }

        private static string FormatProgress(ChallengeData challenge)
        {
            string daysSurvived = $"Days Survived: {ChallengeManager.Instance.CompletedChallengeDays} / {challenge.durationInDays}";

            if (challenge.objectiveType != ObjectiveType.AcceptOffers && challenge.objectiveType != ObjectiveType.RejectOffers)
                return daysSurvived;

            int count = ChallengeManager.Instance.GetCurrentOfferCount();
            string label = challenge.objectiveType == ObjectiveType.AcceptOffers ? "Accepted" : "Rejected";
            string scope = challenge.countScope == CountScope.PerDay ? " today" : string.Empty;
            return $"{label}{scope}: {count} / {challenge.requiredCount}  ·  {daysSurvived}";
        }
    }
}
