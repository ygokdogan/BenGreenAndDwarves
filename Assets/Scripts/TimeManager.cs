using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    
    private int startHour = 9;
    private int endHour = 21;
    public int CurrentDay { get; private set; } = 1;
    public int CurrentHour { get; private set; }
    public int TotalHoursElapsed { get; private set; } = 0;

    public event Action<int> OnDayChanged;
    public event Action<int> OnHourChanged;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        CurrentHour = startHour;
        CurrentDay = 1;
        TotalHoursElapsed = 0;
    }

    public void AdvanceHour(int amount = 1)
    {
        CurrentHour += amount;
        TotalHoursElapsed += amount;

        if (CurrentHour >= endHour)
        {
            CurrentHour = startHour;
            CurrentDay++;
            OnDayChanged?.Invoke(CurrentDay);
        }
        
        OnHourChanged?.Invoke(CurrentHour);
    }
}