using UnityEngine;

[CreateAssetMenu(fileName = "NewOffer", menuName = "Reigns/Offer Card")]
public class OfferCard : ScriptableObject
{
    public string dealerName;
    
    [TextArea(3, 5)]
    public string offerText; // Dealer'ın kapıdaki teklifi/sorusu

    [Header("EVET Seçimi Stat Etkileri")]
    [Tooltip("Sırasıyla: Para, Güvenlik, Akıl Sağlığı, Erzak vb.")]
    public int[] yesStatChanges = new int[4]; 

    [Header("HAYIR Seçimi Stat Etkileri")]
    [Tooltip("Sırasıyla: Para, Güvenlik, Akıl Sağlığı, Erzak vb.")]
    public int[] noStatChanges = new int[4];
}