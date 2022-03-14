using System.Collections;
using System.Collections.Generic;
using System;
public class CalculatorTime
{
    DateTime standardTime;
    public CalculatorTime()
    {
        standardTime = DateTime.Now;
    }
    public void AddFrontTime(long resetTime)
    {
        standardTime = DateTime.Now.AddSeconds(resetTime);
    }
    public string CountDown()
    {
        return TimeSpan.FromSeconds(standardTime.Subtract(DateTime.Now).TotalSeconds).ToString("mm':'ss");
    }
    public string CountUp()
    {
        string str = TimeSpan.FromSeconds(standardTime.TimeOfDay.TotalSeconds - standardTime.Subtract(DateTime.Now).TotalSeconds).ToString("mm':'ss");
        return str;
    }
    public long DiffSecondTime()
    {
        return (long)standardTime.Subtract(DateTime.Now).TotalSeconds;
    }
    public string DiffStringTime(DateTime front, DateTime back)
    {
        return DateTime.FromBinary(DiffSecondTime()).ToString("mm':'ss");
    }
    public static long GetSecondTime(DateTime time)
    {
        return (long)time.TimeOfDay.TotalSeconds;
    }
    public static string GetStringTime(DateTime time)
    {
        return time.ToString();
    }
    public static DateTime GetDateTime(string time)
    {
        return DateTime.Parse(time);
    }
}
