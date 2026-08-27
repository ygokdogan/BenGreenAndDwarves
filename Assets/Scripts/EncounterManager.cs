using System;
using UnityEngine;
using System.Collections.Generic;
using ScriptableObjects;
using Stats;
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
    public List<EncounterData> allEncounters;
    private EncounterData currentEncounter;
    
    private VendorGenerator vendorGenerator;
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
        
        int randomIndex = Random.Range(0, allEncounters.Count);
        currentEncounter = allEncounters[randomIndex];
        
        var generatedVendor = vendorGenerator.Generate();
        generatedVendor.vendorName = currentEncounter.vendorName;
        currentEncounter.vendor = generatedVendor;
        
        encounterUI.ShowEncounterPanel(currentEncounter);
    }
    
    public EncounterData GetCurrentEncounter() => currentEncounter;

    public void LoadNextEncounter() => LoadRandomEncounter();

    public void OnYesButtonClicked()
    {
        GameManager.Instance.ResolveEncounter(currentEncounter, true);
        encounterUI.ShowResultPanel(currentEncounter.acceptResultText); 
    }
    
    public void OnNoButtonClicked()
    {
        GameManager.Instance.ResolveEncounter(currentEncounter, false);
        encounterUI.ShowResultPanel(currentEncounter.rejectResultText); 
    }
    
    public void OnContinueButtonClicked()
    {
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
        if (StatManager.Instance != null && StatManager.Instance.CheckGameOver())
        {
            return;
        }

        if (timeManager != null && timeManager.CurrentHour < 21)
        {
            LoadRandomEncounter();
        }
    }
}