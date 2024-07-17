using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class HervivoreController : Agent
{
    public float TimeElapsed = 0f;
    public GameObject Meat;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Individuo individuo;
    [SerializeField] public EnvController enviroment;
    public int Attitude;
    public int Actuate;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        individuo = GetComponent<Individuo>();
        enviroment = gameObject.transform.parent.transform.GetChild(0).GetComponent<EnvController>();
    }
    public override void OnEpisodeBegin()
    {
        individuo.life = 10;
        individuo.InitializeIndividuo();
        TimeElapsed = 0;
        transform.localPosition = new Vector3(Random.Range(-enviroment.mapSize.x*5, enviroment.mapSize.x * 5), 0.35f, Random.Range(-enviroment.mapSize.x * 5, enviroment.mapSize.x * 5));
        transform.localRotation = Quaternion.Euler(0, Random.Range(0, 359), 0);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.forward);
        sensor.AddObservation(individuo.life);
        sensor.AddObservation(individuo.age);
        sensor.AddObservation(individuo.weight);
        sensor.AddObservation(individuo.minimumWeight);
        sensor.AddObservation(individuo.calories);
        sensor.AddObservation(individuo.water);
        sensor.AddObservation(individuo.power);
        sensor.AddObservation(individuo.speed);
        sensor.AddObservation(individuo.fitness);
        sensor.AddObservation(individuo.adn.diet);
        sensor.AddObservation(individuo.reproductionCooldown);
        RaycastHit hit;
        if (Physics.Raycast(transform.forward, transform.forward, out hit, 30f) && hit.collider.gameObject.tag == "Animal")
        {
            Individuo animal = hit.collider.gameObject.GetComponent<Individuo>();
            sensor.AddObservation(animal.life);
            sensor.AddObservation(animal.size);
            sensor.AddObservation(animal.speed);
            sensor.AddObservation(animal.power);
            sensor.AddObservation(animal.adn.diet);
            sensor.AddObservation(animal.adn.reproductionCompatibility);
        }
        else
        {
            sensor.AddObservation(-1);
            sensor.AddObservation(-1);
            sensor.AddObservation(-1);
            sensor.AddObservation(-1);
            sensor.AddObservation(-1);
            sensor.AddObservation(-1);
        }

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float Forward = actions.ContinuousActions[0];
        float Rotation = actions.ContinuousActions[1];
        float Effort = actions.ContinuousActions[2];
        Attitude = actions.DiscreteActions[0];
        Actuate = actions.DiscreteActions[1];
        rb.MovePosition(transform.position + transform.forward * Forward * (individuo.speed*(Effort+0.5f)) * Time.deltaTime);
        individuo.SetEffort((Effort + 1f) * 3f/4f);
        transform.Rotate(new Vector3(0f, Rotation * individuo.speed, 0f), Space.Self);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Actuate == 1)
        {
            if (individuo.adn.diet == 0)
            {
                if (other.gameObject.tag == "Plant")
                {
                    AddReward(2f);
                    enviroment.ResetlePlant(other.gameObject);
                    //enviroment.ActivePlants.Remove(other.gameObject);
                    //Destroy(other.gameObject);
                    individuo.calories += other.gameObject.GetComponent<Food>().calories;
                }
                else if (other.gameObject.tag == "Meat")
                {
                    AddReward(-2f);
                    enviroment.ResetlePlant(other.gameObject);
                }
            }
            else if (individuo.adn.diet == 1)
            {
                if (other.gameObject.tag == "Meat")
                {
                    individuo.calories += other.gameObject.GetComponent<Food>().calories;
                    AddReward(3f);
                    Destroy(other.gameObject);
                }
                else if (other.gameObject.tag == "Plant")
                {
                    AddReward(-2f);
                    enviroment.ResetlePlant(other.gameObject);
                }
            }
        }
        if (other.gameObject.GetComponent<Individuo>() != null && individuo.reproductionCooldown > 360 && other.gameObject.GetComponent<Individuo>().reproductionCooldown > 360 &&
            individuo.adn.reproductionCompatibility == other.gameObject.GetComponent<Individuo>().adn.reproductionCompatibility && Attitude == 0)
        {
            individuo.reproductionCooldown = 0;
            other.gameObject.GetComponent<Individuo>().reproductionCooldown = 0;
            GameObject child = Instantiate(this.gameObject, this.transform.position, Quaternion.identity, transform.parent);
            child.GetComponent<Individuo>().adn = individuo.GenerateMixedDNA(other.gameObject.GetComponent<Individuo>());
            child.GetComponent<HervivoreController>().enviroment = this.enviroment;
            other.gameObject.GetComponent<Individuo>().calories -= 3500;
            individuo.calories -= 3500;
            Debug.Log("Reproducción");
            AddReward(55f);
        }
        if (other.gameObject.name == "Fall")
        {
            AddReward(-30);
            EndEpisode();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Water" && Actuate == 1)
        {
            if (individuo.water <= individuo.size * 50)
            {
                individuo.water += Time.deltaTime / 1.5f;
                if (individuo.water <= individuo.size * 50)
                {
                    AddReward(Time.deltaTime / 5f);
                }
            }
            else 
            {
                AddReward(-Time.deltaTime / 5f);
            }
        }
        if (other.gameObject.tag == "Animal" && individuo.adn.diet == 1 && Attitude == 1)
        {
            other.gameObject.GetComponent<Individuo>().life -= individuo.power / 5000f * Time.deltaTime;
            if (individuo.adn.diet == 1)
            {
                if (individuo.weight <= (individuo.minimumWeight * 5))
                {
                    AddReward(6f * Time.deltaTime);
                }
            }
        }
    }
    private void Update()
    {
        //AddReward(Time.deltaTime/15f);
        if (individuo.life <= 0)
        {
            GameObject DeadMeat = Instantiate(Meat, new Vector3(transform.position.x, 0.15f, transform.position.z), Quaternion.identity, enviroment.transform);
            DeadMeat.GetComponent<Food>().calories = individuo.calories * 0.4f;
            AddReward(-25f / (individuo.age + 1));
            EndEpisode();
        }
        if (individuo.weight < individuo.minimumWeight || individuo.water<=0)
        {
            //GameObject DeadMeat = Instantiate(Meat, transform.position, Quaternion.identity, enviroment.transform);
            //DeadMeat.GetComponent<Food>().calories = individuo.calories;
            AddReward(-35f / (individuo.age + 1));
            EndEpisode();
        }
    }
}
