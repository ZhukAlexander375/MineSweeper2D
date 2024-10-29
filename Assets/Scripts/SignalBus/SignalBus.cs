using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SignalBus
{
    private static Dictionary<int, List<object>> _dictionarySignals = new Dictionary<int, List<object>>(64);

    public static void Subscribe<T>(Action<T> signal) where T : struct
    {
        var key = typeof(T).GetHashCode();

        if (_dictionarySignals.ContainsKey(key))
            _dictionarySignals[key].Add(signal);
        else
            _dictionarySignals.Add(key, new List<object> { signal });
    }

    public static void Unsubscribe<T>(Action<T> signal) where T : struct
    {
        var key = typeof(T).GetHashCode();

        if (_dictionarySignals.ContainsKey(key))
            _dictionarySignals[key].Remove(signal);
    }

    public static void Fire<T>(T signal = default) where T : struct
    {
        var key = typeof(T).GetHashCode();

        if (!_dictionarySignals.ContainsKey(key) || !_dictionarySignals[key].Any())
        {
            Debug.LogWarning($"There are currently no subscribers for a signal of the type {typeof(T).Name}. If you are not sure that this signal is processed correctly - check signatures and signal calls.");
            return;
        }

        var actions = _dictionarySignals[key].Select(obj => obj as Action<T>).ToList();
        foreach (var action in actions)
            action?.Invoke(signal);
    }
}
