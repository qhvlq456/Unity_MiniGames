using System.Collections;
using System.Collections.Generic;
using System;


/*
내가 필요한걸 찾아보자..
1. lastplaytime
2. frontTime
3. visible time
*/

public class TestTime
{
    DateTime lastTime;
    DateTime frontTime;
    // Construct
    public TestTime(){}
    public TestTime(long lastBinaryTime)
    {
        lastTime = DateTime.FromBinary(lastBinaryTime);
    }
    public TestTime(string lastStringTime)
    {
        lastTime = DateTime.Parse(lastStringTime);
    }

    // Set Time
    public void ResetFrontTime(long addTime)
    {
        frontTime = DateTime.Now.AddSeconds(addTime);
    }
    // diff Time
    public string CountDown()
    {
        return TimeSpan.FromSeconds(frontTime.Subtract(DateTime.Now).TotalSeconds).ToString("mm':'ss");
    }
    public long DiffFrontBinaryTime()
    {
        UnityEngine.Debug.Log($"Return Diff front value = {(long)frontTime.Subtract(DateTime.Now).TotalSeconds}");
        return (long)frontTime.Subtract(DateTime.Now).TotalSeconds;
    }
    public long DiffLastBinaryTime()
    {
        return (long)DateTime.Now.Subtract(lastTime).TotalSeconds;
    }

    // Get DateTime
    public static DateTime GetLastDateTime(string time)
    {
        return DateTime.Parse(time);
    }
    public static DateTime GetLastDateTime(long time)
    {
        return DateTime.FromBinary(time);
    }
}
