using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Challenges;
using Effects;
using ScriptableObjects;
using UI;
using VendorAppearance;
using Random = UnityEngine.Random;

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager Instance;

    public EncounterUIManager encounterUI;
    private PendingEffects pendingEffects;
    private TimeManager timeManager;
    
    [Header("Encounter Database")]
    public List<EncounterData> allEncounters = new List<EncounterData>();
    private List<EncounterData> seenEncounters = new List<EncounterData>();
    private EncounterData currentEncounter;
    
    private VendorGenerator vendorGenerator;
    
    public bool IsWaitingForPendingEffects => waitingForPendingEffects;
    private bool waitingForPendingEffects = false;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        timeManager = GetComponent<TimeManager>();
        pendingEffects = GetComponent<PendingEffects>();
        vendorGenerator = GetComponent<VendorGenerator>();
        
        allEncounters.Clear();
        seenEncounters.Clear();
        allEncounters = Resources.LoadAll<EncounterData>("Encounters/Truths").ToList();
        allEncounters.AddRange(Resources.LoadAll<EncounterData>("Encounters/Lies").ToList());
        
        if (pendingEffects) pendingEffects.OnEffectsTriggered += ShowPendingEffects;
    }

    private void Start()
    {
        LoadRandomEncounter();
    }

    private void OnDestroy()
    {
        if (!pendingEffects) return;
        
        pendingEffects.OnEffectsTriggered -= ShowPendingEffects;
    }

    private void LoadRandomEncounter()
    {
        waitingForPendingEffects = false;
        if (allEncounters.Count == 0) return;
        if (seenEncounters.Count == allEncounters.Count)
            seenEncounters.Clear();

        do
        {
            int randomIndex = Random.Range(0, allEncounters.Count);
            currentEncounter = allEncounters[randomIndex];
        } while (seenEncounters.Contains(currentEncounter));
        
        seenEncounters.Add(currentEncounter);
        
        var generatedVendor = vendorGenerator.Generate();
        generatedVendor.vendorName = currentEncounter.vendorName;
        currentEncounter.vendor = generatedVendor;
        
        encounterUI.ShowEncounterPanel(currentEncounter);
    }
    
    public EncounterData GetCurrentEncounter() => currentEncounter;
    public void LoadNextEncounter() => LoadRandomEncounter();
    
    private void ResolveEncounter(bool accepted)
    {
        currentEncounter.accepted = accepted;

        var effects = accepted
            ? currentEncounter.GetActualEffects()
            : currentEncounter.rejectedEffects;

        foreach (StatEffect effect in effects)
        {
            if (effect.Instant)
                StatManager.Instance.ApplyEffect(effect, checkGameOver: false);
            else
                PendingEffects.Instance.Schedule(currentEncounter, effect, effect.revealDelay);
        }
    }

    public void AcceptOffer()
    {
        ResolveEncounter(true);
        encounterUI.ShowResultPanel(currentEncounter.acceptResultText);
        
        GameManager.Instance.dailyAccepts++;
        ChallengeManager.Instance.OnOfferAccepted();
    }
    
    public void RejectOffer()
    {
        ResolveEncounter(false);
        encounterUI.ShowResultPanel(currentEncounter.rejectResultText);
        
        GameManager.Instance.dailyRejects++;
        ChallengeManager.Instance.OnOfferRejected();
    }
    
    public void OnContinueButtonClicked()
    {
        if (StatManager.Instance != null && StatManager.Instance.CheckGameOver())
        {
            return;
        }

        timeManager.AdvanceHour();
        
        if (!waitingForPendingEffects && timeManager.CurrentHour < 21)
        {
            LoadRandomEncounter();
        }
    }

    private void ShowPendingEffects(List<PendingEffect> effectsTriggered)
    {
        waitingForPendingEffects = true;
        PendingEffectPopup.Instance.Show(effectsTriggered, OnPendingEffectsClosed);
    }

    private void OnPendingEffectsClosed()
    {
        waitingForPendingEffects = false;

        if (StatManager.Instance != null && StatManager.Instance.CheckGameOver())
        {
            return;
        }

        if (timeManager != null && timeManager.CurrentHour < 21)
        {
            LoadRandomEncounter();
        }
        else if (timeManager != null && timeManager.CurrentHour >= 21)
        {
            GameManager.Instance.EndDay(timeManager.CurrentDay);
        }
    }
}
