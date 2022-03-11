using System;
using System.Collections;
using System.Collections.Generic;

public class TimeData : ITime
{
    public DateTime coinDelay;
    public string lastPlayTime;
    public long coinTime;
    
    public void SetCoinDelay(long delay)
    {
        long diffTime = (long)DateTime.Now.Subtract(GetDateTimeLastPlayTime()).TotalSeconds;
        
        long nmgTime = diffTime % delay;
    
        long totalTime = coinTime + nmgTime;
        
        totalTime = totalTime >= delay ? totalTime % delay : totalTime;

        coinDelay = DateTime.Now.AddSeconds(delay - totalTime);
    }
    public void GetCoin(long delay,long addPerCoin,long maxCoin, ref long coin)
    {
        long diffTime = (long)DateTime.Now.Subtract(GetDateTimeLastPlayTime()).TotalSeconds;
        // UnityEngine.Debug.Log($"diffTime = {diffTime}");
        long nmgTime = diffTime % delay;
        
        long receiveCoin = diffTime / delay * addPerCoin;

        if(receiveCoin + coin >= maxCoin)
        {
            coin = maxCoin;
        }
        else
        {
            coin += (receiveCoin * addPerCoin);

            if(coinTime + nmgTime >= delay)
            {
                coin = maxCoin;
            }
            else
            {
                coin += (coinTime + nmgTime) / delay * addPerCoin;
            }
        }

        long totalTime = coinTime + nmgTime; // test용 임시 로그
        UnityEngine.Debug.Log($"coinTime Time = {coinTime}");
        UnityEngine.Debug.Log($"nmgTime Time = {nmgTime}");
        UnityEngine.Debug.Log($"Total Time = {totalTime}");
    }
    public string UpdateTime()
    {
        return TimeSpan.FromSeconds((coinDelay - DateTime.Now).TotalSeconds).ToString("mm':'ss");
    }
    public long GetBinaryLastPlayTime()
    {
        return DateTime.Parse(lastPlayTime).ToBinary();
    }

    public DateTime GetDateTimeLastPlayTime()
    {
        return DateTime.FromBinary(GetBinaryLastPlayTime());
    }

}
