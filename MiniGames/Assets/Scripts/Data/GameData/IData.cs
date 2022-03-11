using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public interface IData
{
    void Create(string json);
    void Create(Dictionary<string,object> dic);
    Task<Dictionary<string,object>> Load(Action callback = null);
    void Save<T>(string key, T value);
}
