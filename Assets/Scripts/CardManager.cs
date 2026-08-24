using UnityEngine;
using TMPro; // TextMeshPro kullanıyorsan
using System.Collections.Generic;
using ScriptableObjects;

public class CardManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI dealerNameText;
    public TextMeshProUGUI offerBodyText;
    
    [Header("Offer Database")]
    public List<EncounterData> allEncounters; // Oyundaki tüm teklifleri buraya sürükle
    private EncounterData currentEncounter;

    private void Start()
    {
        LoadRandomOffer();
    }

    // Yeni bir teklif yükler
    public void LoadRandomOffer()
    {
        if (allEncounters.Count == 0) return;

        int randomIndex = Random.Range(0, allEncounters.Count);
        currentEncounter = allEncounters[randomIndex];

        // UI'ı güncelle
        dealerNameText.text = currentEncounter.vendor.vendorName;
        offerBodyText.text = currentEncounter.saleText;
    }

    // EVET butonuna basıldığında çağrılacak
    public void OnYesButtonClicked()
    {
        GameManager.Instance.ResolveEncounter(currentEncounter, true);
        LoadRandomOffer(); // Sonraki dealer'ı çağır
    }

    // HAYIR butonuna basıldığında çağrılacak
    public void OnNoButtonClicked()
    {
        GameManager.Instance.ResolveEncounter(currentEncounter, false);
        LoadRandomOffer(); // Sonraki dealer'ı çağır
    }
}