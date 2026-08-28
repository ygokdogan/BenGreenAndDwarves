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
    public event Action<int> OnDayEnded;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        ResetTime();
    }

    public void ResetTime()
    {
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
            OnDayEnded?.Invoke(CurrentDay);
        }
        else
        {
            OnHourChanged?.Invoke(CurrentHour);
        }
    }

    public void StartNextDay()
    {
        CurrentHour = startHour;
        CurrentDay++;
        OnDayChanged?.Invoke(CurrentDay);
        OnHourChanged?.Invoke(CurrentHour);
    }
}