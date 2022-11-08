using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class IntegerVariable : ScriptableObject
{
    public int Value;

    public void SetValue(int value)
    {
        Value = value;
    }

    public void SetValue(IntegerVariable value)
    {
        Value = value.Value;
    }

    public void ApplyChange(int amount)
    {
        Value += amount;
    }

    public void ApplyChange(IntegerVariable amount)
    {
        Value += amount.Value;
    }
}
