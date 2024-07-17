using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public int type;
    public float calories;
    public bool perishable;
    private float temp = 0;
    private void Update()
    {
        if (calories <= 0) 
        {
            Destroy(this.gameObject);
        }
        if (perishable) 
        {
            temp += Time.deltaTime;
            if (temp > 5)
            {
                calories = Mathf.FloorToInt(calories / 2);
                temp = 0;
            }
        }
    }
}
