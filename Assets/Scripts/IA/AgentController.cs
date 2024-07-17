using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent
{
    public float TimeElapsed = 0f;
    public GameObject Meat;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Individuo individuo;
    [SerializeField] private EnvController enviroment;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        individuo = GetComponent<Individuo>();
        enviroment = gameObject.transform.parent.transform.GetChild(0).GetComponent<EnvController>();
    }
    public override void OnEpisodeBegin()
    {
        individuo.life = 20;
        individuo.calories = 4000;
        TimeElapsed = 0;
        transform.localPosition = new Vector3(Random.Range(-9, 9), 0.35f, Random.Range(-9, 9));
        transform.localRotation = Quaternion.Euler(0, Random.Range(0, 359), 0);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.forward);
        sensor.AddObservation(individuo.adn.diet);
        sensor.AddObservation(individuo.calories);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float Forward = actions.ContinuousActions[0];
        float Rotation = actions.ContinuousActions[1];
        float speed = 3f;
        rb.MovePosition(transform.position + transform.forward * Forward * speed * Time.deltaTime);
        transform.Rotate(new Vector3(0f, Rotation * speed, 0f), Space.Self);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (individuo.adn.diet == 0)
        {
            if (other.gameObject.tag == "Plant")
            {
                individuo.calories += other.gameObject.GetComponent<Food>().calories;
                AddReward(4f);
                enviroment.ResetlePlant(other.gameObject);
            }
            else if(other.gameObject.tag == "Meat")
            {
                AddReward(-5f);
            }
        }
        else if (individuo.adn.diet == 1)
        {
            if (other.gameObject.tag == "Meat")
            {
                individuo.calories += other.gameObject.GetComponent<Food>().calories;
                AddReward(4f);
            }
            else if (other.gameObject.tag == "Plant")
            {
                AddReward(-5f);
                enviroment.ResetlePlant(other.gameObject);
            }
        }
        if (other.gameObject.name == "Fall")
        {
            AddReward(-12f);
            EndEpisode();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Animal" && individuo.adn.diet == 1) 
        {
            AddReward(2f);
            collision.gameObject.GetComponent<Individuo>().life -= 5;
        }
    }
    private void Update()
    {
        if (individuo.life <= 0) 
        {
            GameObject DeadMeat = Instantiate(Meat, transform.position, Quaternion.identity);
            DeadMeat.GetComponent<Food>().calories = 2370f * individuo.weight;
            AddReward(-10f);
            EndEpisode();
        }
        if (individuo.calories <= 0) 
        {
            AddReward(-12f);
            EndEpisode();
        }
    }

}
