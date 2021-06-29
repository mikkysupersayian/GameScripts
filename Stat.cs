using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat Data", menuName = "Stats/StatsData")]
public class Stat : ScriptableObject  
{
    public int baseValue;

    public int GetValue() 
    {
        return baseValue;
    }

    //add stats
    public void AddValue(int val) {

        baseValue += val;
    }

    //remove stats
    public void RemoveValue(int val) {
        baseValue -= val;
    }

    //in case we actually need to set a flat value
    public void SetValue(int val) {
        baseValue = val;
    }
}
