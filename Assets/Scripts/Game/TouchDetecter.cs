using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDetecter : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.GetComponent<Individuo>() != null)
                {
                    gameObject.GetComponent<ButtonHandler>().ChangeShowLateralBar(true, hit.collider.gameObject.GetComponent<Individuo>());
                }
                else 
                {
                    switch (gameObject.GetComponent<ButtonHandler>().activeEffect) 
                    {
                        case 0:
                            gameObject.GetComponent<ButtonHandler>().ChangeShowLateralBar(false, null);
                            break;
                        case 1:
                            GameObject actualPlant = Instantiate(GameObject.Find("EnvHandler").GetComponent<EnvController>().Plants[0], GameObject.Find("EnvHandler").transform);
                            actualPlant.transform.localPosition = new Vector3(hit.point.x, actualPlant.transform.localPosition.y, hit.point.z);
                            actualPlant.transform.localRotation = Quaternion.Euler(-90, 0, Random.Range(0, 359));
                            break;
                        case 2:
                            GameObject actualWater = Instantiate(GameObject.Find("EnvHandler").GetComponent<EnvController>().Water, new Vector3(hit.point.x, 0.02f, hit.point.z), Quaternion.identity);
                            break;

                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            gameObject.GetComponent<ButtonHandler>().SelectEffect(0);
        }
    }
}
