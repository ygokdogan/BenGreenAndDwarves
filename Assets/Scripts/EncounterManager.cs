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
    public EncounterUIManager encounterUI;
    
    private PendingEffects pendingEffects;
    private TimeManager timeManager;
    
    [Header("Encounter Database")]
    public List<EncounterData> allEncounters;
    private EncounterData currentEncounter;
    
    [Header("Vendor Generator")]
    public VendorGenerator vendorGenerator;

    public bool waitingForPendingEffects = false;

    private void Awake()
    {
        timeManager = GetComponent<TimeManager>();
        pendingEffects = GetComponent<PendingEffects>();
        pendingEffects.OnEffectsTriggered += ShowPendingEffects;
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
        if (allEncounters.Count == 0) return;

        int randomIndex = Random.Range(0, allEncounters.Count);
        currentEncounter = allEncounters[randomIndex];
        
        var generatedVendor = vendorGenerator.Generate();
        currentEncounter.vendor = generatedVendor;
        
        encounterUI.ShowEncounterPanel(currentEncounter);
    }
    
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
        if (!waitingForPendingEffects)
        {
            LoadRandomEncounter();
        }
    }

    private void ShowPendingEffects(List<StatEffect> effectsTriggered)
    {
        waitingForPendingEffects = true;
        PendingEffectPopup.Instance.Show(effectsTriggered, LoadRandomEncounter);
    }
}