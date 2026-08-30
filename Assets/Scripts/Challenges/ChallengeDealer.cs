using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Challenges
{
    public class ChallengeDealer : MonoBehaviour
    {
        public static ChallengeDealer Instance;
        
        [Header("Database")]
        public List<ChallengeData> allChallenges = new List<ChallengeData>();
        
        [Header("UI and References")]
        public GameObject cardPrefab;
        public Transform deckTransform;
        public Transform[] cardSlots;
        public GameObject title;

        private readonly List<GameObject> dealtCards = new List<GameObject>();
        private bool isSelectingChallenge;
        private Vector3 initialScale;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            initialScale = transform.localScale;
        }

        public void ShowDealer()
        {
            gameObject.SetActive(true);
            title.gameObject.SetActive(true);
            DealRandomChallenges();
        }

        public void DealRandomChallenges()
        {
            transform.localScale = initialScale;
            isSelectingChallenge = false;
            foreach (GameObject card in dealtCards)
            {
                if (card != null) Destroy(card);
            }
            dealtCards.Clear();

            List<ChallengeData> tempDeck = new List<ChallengeData>();
            foreach (ChallengeData challenge in allChallenges)
            {
                if (challenge != null && !tempDeck.Contains(challenge))
                    tempDeck.Add(challenge);
            }

            List<ChallengeData> selectedChallenges = new List<ChallengeData>();
            
            int dealCount = cardSlots.Length;
            
            for (int i = 0; i < dealCount; i++)
            {
                if (tempDeck.Count == 0) return;
                
                ChallengeData selectedChallenge = DrawWeightedChallenge(tempDeck);
                if (selectedChallenge == null) return;

                selectedChallenges.Add(selectedChallenge);
                tempDeck.Remove(selectedChallenge);
            }

            for (int i = 0; i < selectedChallenges.Count; i++)
            {
                ChallengeData currentChallenge = selectedChallenges[i];
                
                GameObject cardObj = Instantiate(cardPrefab, deckTransform.parent.position, Quaternion.identity, cardSlots[i]);
                cardObj.GetComponent<ChallengeCardUI>().Setup(currentChallenge);
                dealtCards.Add(cardObj);

                Button cardButton = cardObj.GetComponent<Button>();
                if (cardButton != null)
                    cardButton.onClick.AddListener(() => SelectChallenge(currentChallenge, cardObj));

                float moveDuration = 0.4f;
                float delay = i * 0.3f;
                
                cardObj.transform.DOMove(cardSlots[i].position, moveDuration).SetDelay(delay).SetEase(Ease.OutCubic);
                cardObj.transform.localScale = Vector3.zero;
                cardObj.transform.DOScale(Vector3.one, moveDuration).SetDelay(delay).SetEase(Ease.OutBack);
            }
        }

        private static ChallengeData DrawWeightedChallenge(IReadOnlyList<ChallengeData> challenges)
        {
            float totalWeight = 0f;
            foreach (ChallengeData challenge in challenges)
            {
                if (challenge != null)
                    totalWeight += GetSelectionWeight(challenge);
            }

            if (totalWeight <= 0f)
                return challenges[Random.Range(0, challenges.Count)];

            float roll = Random.value * totalWeight;
            foreach (ChallengeData challenge in challenges)
            {
                if (challenge == null) continue;

                roll -= GetSelectionWeight(challenge);
                if (roll <= 0f)
                    return challenge;
            }

            return challenges[challenges.Count - 1];
        }

        private static float GetSelectionWeight(ChallengeData challenge)
        {
            // Existing assets created before this field was added deserialize as zero.
            return challenge.selectionWeight > 0f ? challenge.selectionWeight : 1f;
        }

        private void SelectChallenge(ChallengeData challenge, GameObject selectedCard)
        {
            if (isSelectingChallenge || challenge == null)
                return;

            isSelectingChallenge = true;
            ChallengeManager.Instance?.SelectChallenge(challenge);

            foreach (GameObject card in dealtCards)
            {
                if (card == null) continue;

                card.transform.DOKill();

                if (card == selectedCard) continue;

                card.transform.DOMove(deckTransform.parent.position, 0.35f).SetEase(Ease.InBack);
                card.transform.DOScale(Vector3.one * 0.7f, 0.35f).SetEase(Ease.InBack);
                card.transform.DORotate(Vector3.zero, 0.2f);
            }

            if (selectedCard == null)
            {
                DOVirtual.DelayedCall(0.35f, CloseAndBeginGameplay);
                return;
            }

            selectedCard.transform.SetAsLastSibling();

            Sequence shakeSequence = DOTween.Sequence();
            shakeSequence.Append(selectedCard.transform
                .DORotate(new Vector3(0f, 0f, 8f), 0.1f)
                .SetEase(Ease.OutCubic));
            shakeSequence.Append(selectedCard.transform
                .DORotate(new Vector3(0f, 0f, -8f), 0.1f)
                .SetEase(Ease.InOutCubic));
            shakeSequence.Append(selectedCard.transform
                .DORotate(new Vector3(0f, 0f, 8f), 0.1f)
                .SetEase(Ease.OutCubic));
            shakeSequence.Append(selectedCard.transform
                .DORotate(new Vector3(0f, 0f, -8f), 0.1f)
                .SetEase(Ease.InOutCubic));
            shakeSequence.Append(selectedCard.transform
                .DORotate(Vector3.zero, 0.05f)
                .SetEase(Ease.InCubic));

            Sequence selectionSequence = DOTween.Sequence();
            selectionSequence.Append(selectedCard.transform
                .DOScale(Vector3.one * 1.3f, 0.45f)
                .SetEase(Ease.OutBack));
            selectionSequence.Join(shakeSequence);
            selectionSequence.AppendInterval(0.15f);
            selectionSequence.AppendCallback(CloseAndBeginGameplay);
        }

        private void CloseAndBeginGameplay()
        {
            transform.DOKill();
            title.gameObject.SetActive(false);
            transform.DOScale(Vector3.zero, 0.3f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    GameFlowManager.Instance?.BeginGameplay();
                });
        }
    }
}
