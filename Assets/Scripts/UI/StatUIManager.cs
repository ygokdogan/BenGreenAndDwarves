using UnityEngine;
using UnityEngine.UI; // Slider'ları kullanmak için gerekli
using Stats; // Senin yazdığın StatType enum'ına erişmek için

public class StatUIManager : MonoBehaviour
{
    [Header("UI Barları")]
    public Slider happinessSlider;
    public Slider healthSlider;
    public Slider storageSlider;
    public Slider cashSlider;

    private void Start()
    {
        // StatManager'da belirlediğin gibi barların maksimum sınırını 100 yapıyoruz
        if (happinessSlider) happinessSlider.maxValue = 100;
        if (healthSlider) healthSlider.maxValue = 100;
        if (storageSlider) storageSlider.maxValue = 100;
        if (cashSlider) cashSlider.maxValue = 100;
    }

    private void Update()
    {
        // StatManager'daki dictionary (sözlük) içinden güncel stat değerlerini alıp barlara yansıtıyoruz
        if (StatManager.Instance != null)
        {
            happinessSlider.value = StatManager.Instance.stats[StatType.Happiness];
            healthSlider.value = StatManager.Instance.stats[StatType.Health];
            storageSlider.value = StatManager.Instance.stats[StatType.Storage];
            cashSlider.value = StatManager.Instance.stats[StatType.Cash];
        }
    }
}