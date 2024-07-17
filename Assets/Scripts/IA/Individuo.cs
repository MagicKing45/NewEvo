using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class Individuo : MonoBehaviour
{
    public float life = 10;
    public float age;
    public float size;
    public float expectedSize;
    public float weight;
    public float calories;
    public float water;
    public float power;
    public float fitness;
    public float speed;
    public float reproductionCooldown;
    public ADN adn;
    public float minimumWeight;

    public float timeAlive = 0;
    private float secs;
    private const float STANDAR_DENSITY = 950f;
    private const float CALORIES_PER_KG = 2370f;
    private float effort = 1f;
    public void Start()
    {
        //InitializeIndividuo();
        //this.GetComponent<MeshRenderer>().material.color = adn.color;
    }
    public void InitializeIndividuo() 
    {
        if (adn.defaultSize > 0)
        {
            float volume = Mathf.Pow(adn.defaultSize, 1f / 3f);
            transform.localScale = new Vector3(volume, volume, volume);
            this.transform.GetChild(1).GetComponent<MeshRenderer>().material.color = adn.color;
        }
        size = transform.localScale.x * transform.localScale.y * transform.localScale.z;
        weight = size * STANDAR_DENSITY;
        calories = weight * CALORIES_PER_KG;
        water = size * 100f / 2f;
        power = weight * (fitness/10f);
        speed = (size + fitness / 10000f) * 2000f / weight;
        minimumWeight = size * 600;
        reproductionCooldown = 0;
        timeAlive = 0;
    }
    public void Update()
    {
        reproductionCooldown += Time.deltaTime;
        secs += Time.deltaTime;
        timeAlive += Time.deltaTime;
        if (secs >= 1.5f) 
        {
            calories -= 2600 * effort * size;
            water -= 0.5f * effort * size;
            secs = 0;
            weight = calories / CALORIES_PER_KG;
            power = weight * fitness;
            speed = (size + fitness / 10000f) * 3000f / weight;
            age = timeAlive / 60;
        }
    }
    public void SetEffort(float newEffort) 
    {
        effort = newEffort;
    }
    public void SetDNA(ADN dna) 
    {
        adn.defaultSize = dna.defaultSize;
        adn.diet = dna.diet;
        adn.reproductionCompatibility = dna.reproductionCompatibility;
        adn.color = dna.color;
        InitializeIndividuo();
    }
    public ADN CreateRandomDNA(int Diet = 0)
    {
        adn.defaultSize = Random.Range(0.01f, 3f);
        adn.diet = Diet;
        adn.reproductionCompatibility = Random.Range(0, 5);
        float r = Random.value; // Componente rojo
        float g = Random.value; // Componente verde
        float b = Random.value; // Componente azul

        // Crea un nuevo color aleatorio
        Color randomColor = new Color(r, g, b);
        adn.color = randomColor;
        InitializeIndividuo();
        return adn;
    }
    public ADN GenerateMixedDNA(Individuo partner)
    {
        float newSize = ((size + partner.size) / 2f) + ((size + partner.size) / 2f) * Random.Range(-0.15f, 0.15f);
        int newDiet;
        if (Random.Range(0, 2) == 0)
            newDiet = adn.diet;
        else
            newDiet = partner.adn.diet;
        int newRepCompatibility;
        if (Random.Range(0, 2) == 0)
            newRepCompatibility = adn.reproductionCompatibility;
        else
            newRepCompatibility = partner.adn.reproductionCompatibility;
        Color newColor = Color.Lerp(adn.color, partner.adn.color, Random.Range(0f, 1f));
        ADN sonDNA = new ADN(newSize, newDiet, newRepCompatibility, newColor);
        return sonDNA;
    }
}
