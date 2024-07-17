using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public string Enviroment;
    public List<ADN> SpeciesDNA;
    public List<GameObject> Models;
    public List<int> SpeciesCount;
    public void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void ApplyDNA(int diet, float size, int repCompatibility, Color color, GameObject Model, int Count) 
    {
        ADN newDNA = new ADN(size, diet, repCompatibility, color);
        SpeciesCount.Add(Count);
        Models.Add(Model);
        SpeciesDNA.Add(newDNA);
    }
}
