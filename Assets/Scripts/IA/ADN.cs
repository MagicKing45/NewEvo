using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ADN
{
    [SerializeField]
    public float defaultSize;
    [SerializeField]
    public int diet;
    [SerializeField]
    public int reproductionCompatibility;
    [SerializeField]
    public Color color;

    public ADN(int Diet, Color clr) 
    {
        diet = Diet;
        color = clr;
    }
    public ADN(float DefSize, int Diet, int repCompatibility, Color clr)
    {
        defaultSize = DefSize;
        diet = Diet;
        reproductionCompatibility = repCompatibility;
        color = clr;
    }
}