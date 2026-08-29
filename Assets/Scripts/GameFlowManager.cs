using System;
using Challenges;
using Effects;
using UI;
using UnityEngine;
using VendorAppearance;

public enum GameFlowState
{
    ChallengeSelection,
    PlayingEncounter,
    PendingEffects,
    DaySummary,
    GameOver,
    GameWon
}

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }
    public GameFlowState CurrentState { get; private set; }

    private VendorController vendor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        if (Camera.main != null) vendor = Camera.main.GetComponentInChildren<VendorController>();
        Instance = this;
    }

    private void Start()
    {
        if (PendingEffects.Instance != null)
            PendingEffects.Instance.OnEffectsTriggered += ShowPendingEffects;

        
        BeginChallengeSelection();
    }

    private void OnDestroy()
    {
        if (PendingEffects.Instance != null)
            PendingEffects.Instance.OnEffectsTriggered -= ShowPendingEffects;

        if (Instance == this)
            Instance = null;
    }

    public void BeginChallengeSelection()
    {
        CurrentState = GameFlowState.ChallengeSelection;
        SetGameplayUIActive(false);
        SetHUD(false);
        EncounterManager.Instance?.HideEncounterUI();
        ChallengeDealer.Instance?.ShowDealer();
    }

    public void BeginGameplay()
    {
        CurrentState = GameFlowState.PlayingEncounter;
        SetGameplayUIActive(true);
        SetHUD(true);
        EncounterManager.Instance?.LoadNextEncounter();
    }

    public void ContinueEncounter()
    {
        if (CurrentState != GameFlowState.PlayingEncounter)
            return;

        if (StatManager.Instance != null && StatManager.Instance.CheckGameOver())
            return;

        TimeManager.Instance.AdvanceHour();
        
        if (CurrentState == GameFlowState.PendingEffects)
            return;

        if (TimeManager.Instance.CurrentHour >= TimeManager.Instance.endHour)
        {
            ShowDaySummary();
            return;
        }

        EncounterManager.Instance?.LoadNextEncounter();
    }

    private void ShowPendingEffects(System.Collections.Generic.List<PendingEffect> effects)
    {
        CurrentState = GameFlowState.PendingEffects;
        EncounterManager.Instance?.SetWaitingForPendingEffects(true);
        PendingEffectPopup.Instance?.Show(effects, OnPendingEffectsClosed);
    }

    private void OnPendingEffectsClosed()
    {
        EncounterManager.Instance?.SetWaitingForPendingEffects(false);

        if (StatManager.Instance != null && StatManager.Instance.CheckGameOver())
            return;

        if (TimeManager.Instance.CurrentHour >= TimeManager.Instance.endHour)
        {
            ShowDaySummary();
            return;
        }

        BeginGameplay();
    }

    public void ShowDaySummary()
    {
        if (CurrentState == GameFlowState.GameOver || CurrentState == GameFlowState.GameWon)
            return;

        CurrentState = GameFlowState.DaySummary;
        SetGameplayUIActive(false);
        GameManager.Instance.EndDay(TimeManager.Instance.CurrentDay);
    }

    public void StartNextDay()
    {
        if (CurrentState != GameFlowState.DaySummary)
            return;

        if (DayEndUIManager.Instance != null && DayEndUIManager.Instance.dayEndPanel != null)
            DayEndUIManager.Instance.dayEndPanel.SetActive(false);

        if (StatManager.Instance != null && StatManager.Instance.CheckGameOver())
            return;

        ChallengeManager.Instance?.OnDayResolved(TimeManager.Instance.CurrentDay);
        TimeManager.Instance.StartNextDay();
        BeginGameplay();
    }

    public void ShowGameOver(StatType stat, bool isZero)
    {
        CurrentState = GameFlowState.GameOver;
        SetGameplayUIActive(false);
        SetHUD(false);
        GameOverUIManager.Instance?.ShowGameOver(stat, isZero);
    }

    public void ShowGameWon()
    {
        CurrentState = GameFlowState.GameWon;
        SetGameplayUIActive(false);
        SetHUD(false);
        GameWonUIManager.Instance?.ShowGameWon();
    }

    public void SetHUD(bool show)
    {
        HUDManager.Instance?.SetHUD(show);
    }

    private void SetGameplayUIActive(bool isActive)
    {
        if (DayEndUIManager.Instance != null && DayEndUIManager.Instance.gameplayUI != null)
            DayEndUIManager.Instance.gameplayUI.SetActive(isActive);
        
        if(vendor != null) vendor.gameObject.SetActive(isActive);
    }
}
