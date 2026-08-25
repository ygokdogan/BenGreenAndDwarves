using System;
using UnityEngine;
using TMPro; 
using System.Collections.Generic;
using ScriptableObjects;
using Stats;
using VendorAppearance;
using Random = UnityEngine.Random; // Senin oluşturduğun verilere ulaşmak için gerekli

public class CardManager : MonoBehaviour
{
    [Header("Paneller (Gruplar)")]
    public GameObject offerPanel;  // Dealer, soru ve Evet/Hayır butonlarını taşıyan ana obje
    public GameObject resultPanel; // Sonuç metni ve Devam butonunu taşıyan ana obje

    [Header("Teklif Ekranı UI (Offer)")]
    public TextMeshProUGUI dealerNameText;
    public TextMeshProUGUI offerBodyText;
    
    [Header("Sonuç Ekranı UI (Result)")]
    public TextMeshProUGUI resultBodyText;

    [Header("Teklif Veritabanı")]
    public List<EncounterData> allEncounters;
    private EncounterData currentEncounter;
    
    [Header("Vendor Generator")]
    public VendorGenerator vendorGenerator;

    public bool waitingForPendingEffects = false;

    private void Start()
    {
        PendingEffects.Instance.OnEffectsTriggered += ShowPendingEffects;
        LoadRandomOffer();
    }

    private void OnDestroy()
    {
        if (!PendingEffects.Instance) return;
        
        PendingEffects.Instance.OnEffectsTriggered -= ShowPendingEffects;
    }

    public void LoadRandomOffer()
    {
        if (allEncounters.Count == 0) return;

        int randomIndex = Random.Range(0, allEncounters.Count);
        currentEncounter = allEncounters[randomIndex];
        
        var generatedVendor = vendorGenerator.Generate();
        currentEncounter.vendor = generatedVendor;
        
        offerPanel.SetActive(true);
        resultPanel.SetActive(false);
        
        dealerNameText.text = currentEncounter.vendor.vendorName;
        offerBodyText.text = currentEncounter.saleText;
    }
    
    public void OnYesButtonClicked()
    {
        GameManager.Instance.ResolveEncounter(currentEncounter, true);
        
        ShowResult(currentEncounter.acceptResultText); 
    }
    
    public void OnNoButtonClicked() //[cite: 6]
    {
        // GameManager üzerinden statları güncelle
        GameManager.Instance.ResolveEncounter(currentEncounter, false);
        
        ShowResult(currentEncounter.rejectResultText); 
    }
    
    private void ShowResult(string resultMessage)
    {
        offerPanel.SetActive(false);
        resultPanel.SetActive(true);
        
        resultBodyText.text = resultMessage;
    }
    
    public void OnContinueButtonClicked()
    {
        TimeManager.Instance.AdvanceHour();
        if (!waitingForPendingEffects)
        {
            LoadRandomOffer();
        }
    }

    private void ShowPendingEffects(List<StatEffect> effectsTriggered)
    {
        waitingForPendingEffects = true;
    }
}