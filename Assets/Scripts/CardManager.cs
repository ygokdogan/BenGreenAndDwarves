using UnityEngine;
using TMPro; 
using System.Collections.Generic;
using ScriptableObjects; // Senin oluşturduğun verilere ulaşmak için gerekli

public class CardManager : MonoBehaviour
{
    [Header("Paneller (Gruplar)")]
    public GameObject offerPanel;  // Dealer, soru ve Evet/Hayır butonlarını taşıyan ana obje
    public GameObject resultPanel; // Sonuç metni ve Devam butonunu taşıyan ana obje

    [Header("Teklif Ekranı UI (Offer)")]
    public TextMeshProUGUI dealerNameText; //[cite: 6]
    public TextMeshProUGUI offerBodyText; //[cite: 6]
    
    [Header("Sonuç Ekranı UI (Result)")]
    public TextMeshProUGUI resultBodyText; // Oyuncuya sonucu açıklayacak metin

    [Header("Teklif Veritabanı")]
    public List<EncounterData> allEncounters; // Sahnede kullanacağın tüm kartlar[cite: 6]
    private EncounterData currentEncounter; //[cite: 6]

    private void Start()
    {
        LoadRandomOffer(); //[cite: 6]
    }

    // Yeni bir teklif yükler
    public void LoadRandomOffer() //[cite: 6]
    {
        if (allEncounters.Count == 0) return; //[cite: 6]

        int randomIndex = Random.Range(0, allEncounters.Count); //[cite: 6]
        currentEncounter = allEncounters[randomIndex]; //[cite: 6]

        // Yeni dealer geldiğinde teklif ekranını aç, sonuç ekranını gizle
        offerPanel.SetActive(true);
        resultPanel.SetActive(false);

        // Arayüzü güncelle
        dealerNameText.text = currentEncounter.vendor.vendorName; //[cite: 6]
        offerBodyText.text = currentEncounter.saleText; //[cite: 6]
    }

    // EVET butonuna basıldığında çağrılacak
    public void OnYesButtonClicked() //[cite: 6]
    {
        // GameManager üzerinden senin yazdığın stat güncelleme fonksiyonunu çalıştır
        GameManager.Instance.ResolveEncounter(currentEncounter, true); //[cite: 6]
        
        // Sonuç ekranına geç ve kabul etme metnini (acceptResultText) göster
        ShowResult(currentEncounter.acceptResultText); 
    }

    // HAYIR butonuna basıldığında çağrılacak
    public void OnNoButtonClicked() //[cite: 6]
    {
        // GameManager üzerinden statları güncelle
        GameManager.Instance.ResolveEncounter(currentEncounter, false); //[cite: 6]
        
        // Sonuç ekranına geç ve reddetme metnini (rejectResultText) göster
        ShowResult(currentEncounter.rejectResultText); 
    }

    // Arayüzü sonuç ekranına çeviren özel fonksiyon
    private void ShowResult(string resultMessage)
    {
        offerPanel.SetActive(false); // Soru sorma panelini kapat
        resultPanel.SetActive(true); // Sonuç gösterme panelini aç
        
        resultBodyText.text = resultMessage; // Ekrana sonucu yaz
    }

    // "Devam Et" butonuna basıldığında tetiklenecek fonksiyon
    public void OnContinueButtonClicked()
    {
        LoadRandomOffer(); // Sonraki dealer'ı çağır ve döngüyü başa sar
    }
}