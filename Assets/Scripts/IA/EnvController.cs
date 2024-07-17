using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class EnvController : MonoBehaviour
{
    public float temperature = 15f;
    public int numberPlants = 0;
    public int numberWater = 0;
    public int numberTrees = 0;
    public GameObject[] Plants;
    public GameObject[] Trees;
    public List<GameObject> ActivePlants;
    public List<GameObject> especies;
    public List<int> numberEspecies;
    public GameObject Water;
    public float timer = 0f;
    public Vector2 mapSize = new Vector2(50, 50);
    public bool isRandom = false;
    public bool isTraining = false;
    public float maxTimeHerv = 0f;
    public float maxTimeCarnv = 0f;
    public GameObject Hervivore;
    public GameObject Carnivore;
    public void Start()
    {
        if (!isTraining)
        {
            GameHandler handler = GameObject.Find("GameHandler").GetComponent<GameHandler>();
            if (handler != null)
            {
                especies.Clear();
                for (int i = 0; i < handler.SpeciesDNA.Count; i++)
                {
                    GameObject Especie;
                    for (int j = 0; j < handler.SpeciesCount[i]; j++)
                    {
                        if (handler.SpeciesDNA[i].diet == 0)
                        {
                            Especie = Instantiate(Hervivore, new Vector3(Random.Range(-mapSize.x * 5, mapSize.x * 5), 0, Random.Range(-mapSize.y * 5, mapSize.y * 5)), Quaternion.identity, transform.parent);
                        }
                        else
                        {
                            Especie = Instantiate(Carnivore, new Vector3(Random.Range(-mapSize.x * 5, mapSize.x * 5), 0, Random.Range(-mapSize.y * 5, mapSize.y * 5)), Quaternion.identity, transform.parent);
                        }
                        Especie.transform.GetChild(1).GetComponent<MeshRenderer>().sharedMaterials = handler.Models[i].GetComponent<MeshRenderer>().sharedMaterials;
                        Especie.transform.GetChild(1).GetComponent<MeshRenderer>().materials = handler.Models[i].GetComponent<MeshRenderer>().sharedMaterials;
                        Especie.transform.GetChild(1).GetComponent<MeshFilter>().mesh = handler.Models[i].GetComponent<MeshFilter>().sharedMesh;
                        Especie.transform.GetChild(1).transform.localRotation = Quaternion.Euler(new Vector3(-90f, Especie.transform.GetChild(1).transform.localRotation.eulerAngles.y, 90f));
                        Especie.GetComponent<Individuo>().SetDNA(handler.SpeciesDNA[i]);
                    }
                }
            }
        }
        else
        {
            GenerateIndividuos();
        }
        GenerateWater();
        GeneratePlants();
        GenerateTrees();
    }
    private void Update()
    {
        /*timer += Time.deltaTime;
        if (timer > 5f) 
        {
            GameObject actualPlant = Instantiate(Plants[Random.Range(0, Plants.Length - 1)], this.transform);
            actualPlant.transform.localPosition = new Vector3(Random.Range(-mapSize.x*5, mapSize.x * 5), 0, Random.Range(-mapSize.y * 5, mapSize.y * 5));
            actualPlant.transform.localRotation = Quaternion.Euler(-90, 0, Random.Range(0, 359));
            this.ActivePlants.Add(actualPlant);
            timer = 0;
        }*/
    }
    public void GenerateWater() 
    {
        for (int i = 0; i < numberWater; i++) 
        {
            GameObject actualWater = Instantiate(Water, new Vector3(Random.Range((-mapSize.x+1) * 5, (mapSize.x - 1) * 5), 0.02f, Random.Range((-mapSize.x + 1) * 5, (mapSize.x - 1) * 5)), Quaternion.identity);
        }
    }
    public void GeneratePlants() 
    {
        for (int i = 0; i < numberPlants; i++) 
        {
            GameObject actualPlant = Instantiate(Plants[Random.Range(0, Plants.Length - 1)], this.transform);
            ResetlePlant(actualPlant);
            actualPlant.transform.localRotation = Quaternion.Euler(-90, 0, Random.Range(0, 359));
            this.ActivePlants.Add(actualPlant);
        }
    }
    public void GenerateTrees()
    {
        for (int i = 0; i < numberTrees; i++)
        {
            GameObject actualTree = Instantiate(Trees[Random.Range(0, Trees.Length - 1)], this.transform);
            ResetlePlant(actualTree);
            actualTree.transform.localRotation = Quaternion.Euler(-90, 0, Random.Range(0, 359));
        }
    }
    public void SetWater(Vector3 pos) 
    {
        GameObject actualWater = Instantiate(Water, pos, Quaternion.identity);
    }
    public void SetPlant(Vector3 pos)
    {
        GameObject actualPlant = Instantiate(Plants[Random.Range(0, Plants.Length - 1)], this.transform);
        actualPlant.transform.localRotation = Quaternion.Euler(-90, 0, Random.Range(0, 359));
        actualPlant.transform.localPosition = pos;
    }
    public void GenerateIndividuos()
    {
        if (!isRandom)
        {
            for (int i = 0; i < especies.Count; i++)
            {
                for (int j = 0; j < numberEspecies[i]; j++)
                {
                    GameObject nuevoIndividuo = Instantiate(especies[i], new Vector3(Random.Range(-mapSize.x * 5, mapSize.x * 5), 0, Random.Range(-mapSize.y * 5, mapSize.y * 5)), Quaternion.identity, transform.parent);
                    nuevoIndividuo.GetComponent<HervivoreController>().enviroment = this;
                }
            }
        }
        else 
        {
            for (int i = 0; i < especies.Count; i++)
            {
                ADN newDNA = null;
                for (int j = 0; j < numberEspecies[i]; j++)
                {
                    GameObject nuevoIndividuo = Instantiate(especies[i], new Vector3(Random.Range(-mapSize.x * 5, mapSize.x * 5), 0, Random.Range(-mapSize.y * 5, mapSize.y * 5)), Quaternion.identity, transform.parent);
                    if (newDNA == null)
                    {
                        
                        if (especies[i].name == "ProtoHevivoro")
                        {
                            newDNA = nuevoIndividuo.GetComponent<Individuo>().CreateRandomDNA(0);
                        }
                        else if (especies[i].name == "ProtoCarnivoro") 
                        {
                            newDNA = nuevoIndividuo.GetComponent<Individuo>().CreateRandomDNA(1);
                        }
                    }
                    else 
                    {
                        nuevoIndividuo.GetComponent<Individuo>().adn = newDNA;
                    }
                    nuevoIndividuo.transform.position = new Vector3(nuevoIndividuo.transform.position.x, nuevoIndividuo.GetComponent<Individuo>().adn.defaultSize / 2f + 0.3f, nuevoIndividuo.transform.position.z);
                    nuevoIndividuo.GetComponent<HervivoreController>().enviroment = this;
                }
            }
        }
    }
    public void ResetlePlant(GameObject plant)
    {
        plant.transform.localPosition = new Vector3(Random.Range(-mapSize.x * 5, mapSize.x * 5), plant.transform.localPosition.y, Random.Range(-mapSize.y * 5, mapSize.y * 5));
        RaycastHit hit;
        Physics.Raycast(new Vector3(plant.transform.localPosition.x, 2f, plant.transform.localPosition.z), Vector3.down, out hit);
        if (hit.transform.gameObject.name != "Ground")
        {
            ResetlePlant(plant);
        }
    }
    public void ResetlePlants()
    {
        foreach (GameObject plant in ActivePlants) 
        {
            ResetlePlant(plant);
        }
    }
}
