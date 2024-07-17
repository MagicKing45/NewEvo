using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovCamara : MonoBehaviour
{
    GameObject ground;
    Camera camara;
    public float zoomSpeed = 1f, fwdOffset;
    float camSpeed = 2.5f, camSpeedMax, camSpeedMin;
    float limiteSup, limiteInf, anguloFinal, anguloInicial, alturaInf, alturaSup;
    bool rotating, moveFwd, moveBwd, moveRight, moveLeft;
    int direction, playerID;
    public Vector3 staticPos;

    void Start()
    {
        // Inicialización de variables y límites del movimiento
        direction = 0;
        limiteSup = 0;
        limiteInf = 210; // 15 antes
        anguloInicial = 35;
        anguloFinal = 25;
        alturaSup = transform.position.y;
        alturaInf = 6.5f;
        camSpeedMin = 0.25f;
        camSpeedMax = 2.5f;

        moveFwd = true;
        moveBwd = true;
        moveRight = true;
        moveLeft = true;
        camara = gameObject.GetComponent<Camera>();
        ground = GameObject.Find("Ground");
        this.staticPos = transform.position;
        // Distancia a la que se encuentra el punto máximo de zoom, siendo este valor 28 cuando el zoom está al mínimo y 0 cuando está al máximo
        fwdOffset = Mathf.Lerp(0, 28, (transform.position.y - alturaInf) / (alturaSup - alturaInf));
    }

    void FixedUpdate()
    {
        // Comprobación de límites del mapa en función de a que dirección está mirando, comrueba en que direcciones se puede mover
        switch (direction)
        {
            // Z positivo
            case 0:
                moveFwd = transform.position.z < ground.transform.localScale.x * 5 - limiteSup;
                moveBwd = transform.position.z > -limiteInf;
                moveLeft = transform.position.x > -ground.transform.localScale.x * 5;
                moveRight = transform.position.x < ground.transform.localScale.x * 5 - 5;
                break;
            // X negativo
            case 1:
                moveFwd = transform.position.x > limiteSup;
                moveBwd = transform.position.x < ground.transform.localScale.x * 5 + limiteInf;
                moveLeft = transform.position.z > 5;
                moveRight = transform.position.z < ground.transform.localScale.x * 5 - 5;
                break;
            // Z negativo
            case 2:
                moveFwd = transform.position.z > limiteSup;
                moveBwd = transform.position.z < ground.transform.localScale.x * 5 + limiteInf;
                moveLeft = transform.position.x < ground.transform.localScale.x * 5 - 5;
                moveRight = transform.position.x > 5;
                break;
            // X positivo
            case 3:
                moveFwd = transform.position.x < ground.transform.localScale.x * 5 - limiteSup;
                moveBwd = transform.position.x > -limiteInf;
                moveLeft = transform.position.z < ground.transform.localScale.x * 5 - 5;
                moveRight = transform.position.z > 5;
                break;
        }

        // Mover hacia delante
        if (Input.GetKey(KeyCode.W) && moveFwd)
        {
            Vector3 forwardDirection = transform.forward;
            forwardDirection.y = 0f;
            transform.position += forwardDirection.normalized * camSpeed;
            staticPos = transform.position;
            fwdOffset = Mathf.Lerp(0, 28, (transform.position.y - alturaInf) / (alturaSup - alturaInf));
        }

        // Mover hacia atrás
        if (Input.GetKey(KeyCode.S) && moveBwd)
        {
            Vector3 forwardDirection = transform.forward;
            forwardDirection.y = 0f;
            transform.position += forwardDirection.normalized * -camSpeed;
            staticPos = transform.position;
            fwdOffset = Mathf.Lerp(0, 28, (transform.position.y - alturaInf) / (alturaSup - alturaInf));
        }

        // Mover derecha
        if (Input.GetKey(KeyCode.D) && moveRight)
        {
            transform.position += transform.right * camSpeed;
            staticPos = transform.position;
            fwdOffset = Mathf.Lerp(0, 28, (transform.position.y - alturaInf) / (alturaSup - alturaInf));
        }

        // Mover izquierda
        if (Input.GetKey(KeyCode.A) && moveLeft)
        {
            transform.position += transform.right * -camSpeed;
            staticPos = transform.position;
            fwdOffset = Mathf.Lerp(0, 28, (transform.position.y - alturaInf) / (alturaSup - alturaInf));
        }

        // Input para hacer zoom hacia dentro
        if ((Input.GetKey(KeyCode.Z) || Input.mouseScrollDelta.y > 0) && transform.position.y >= alturaInf)
        {
            ZoomManagement(true, Input.mouseScrollDelta.y);
        }
        // Input para hacer zoom hacia fuera
        if ((Input.GetKey(KeyCode.X) || Input.mouseScrollDelta.y < 0) && transform.position.y <= alturaSup)
        {
            ZoomManagement(false, Input.mouseScrollDelta.y);
        }
    }

    // Gestión del movimiento del zoom
    void ZoomManagement(bool zoomIn, float mouseSpeed)
    {
        // Al hacer zoom movemos la propia cámara, en función de la dirección del zoom
        if (zoomIn) {
            transform.position += transform.forward.normalized * (zoomSpeed + mouseSpeed);
        }
        else
            transform.position += transform.forward.normalized * (-zoomSpeed + mouseSpeed);

        // Dependiendo de la dirección, se comprueba que la posición esté dentro de los límites del zoom tanto en y como en x o z
        /*switch (direction)
        {
            case 0:
                transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, alturaInf, alturaSup), Mathf.Clamp(transform.position.z, staticPos.z - (28 - fwdOffset), staticPos.z + fwdOffset));
                break;
            case 1:
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, staticPos.x - fwdOffset, staticPos.x + (28 - fwdOffset)), Mathf.Clamp(transform.position.y, alturaInf, alturaSup), transform.position.z);
                break;
            case 2:
                transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, alturaInf, alturaSup), Mathf.Clamp(transform.position.z, staticPos.z - fwdOffset, staticPos.z + (28 - fwdOffset)));
                break;
            case 3:
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, staticPos.x - (28 - fwdOffset), staticPos.x + fwdOffset), Mathf.Clamp(transform.position.y, alturaInf, alturaSup), transform.position.z);
                break;
        }

        // Dependiendo de la dirección, se comprueba que la posición esté dentro de los limites del mapa
        switch (direction)
        {
            case 0:
                transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, alturaInf, alturaSup), Mathf.Clamp(transform.position.z, -limiteInf, guardado.partida.ladoMapa - limiteSup));
                break;
            case 1:
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, limiteSup, guardado.partida.ladoMapa + limiteInf), Mathf.Clamp(transform.position.y, alturaInf, alturaSup), transform.position.z);
                break;
            case 2:
                transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, alturaInf, alturaSup), Mathf.Clamp(transform.position.z, limiteSup, guardado.partida.ladoMapa + limiteInf));
                break;
            case 3:
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, -limiteInf, guardado.partida.ladoMapa - limiteSup), Mathf.Clamp(transform.position.y, alturaInf, alturaSup), transform.position.z);
                break;
        }*/
        // Reajuste de las variables de velocidad de cámara, limites de mapa y rotación en x, en función del zoom
        camSpeed = Mathf.Lerp(camSpeedMin, camSpeedMax, (transform.position.y - alturaInf) / (alturaSup - alturaInf));
        //limiteInf = Mathf.Lerp(14.8f, 32.5f, (transform.position.y - alturaInf) / (alturaSup - alturaInf));
        //limiteSup = Mathf.Lerp(35f, 50f, (transform.position.y - alturaInf) / (alturaSup - alturaInf));
        float currentRotationY = transform.localRotation.eulerAngles.y;
        float currentRotationZ = transform.localRotation.eulerAngles.z;
        transform.localRotation = Quaternion.Euler(new Vector3(Mathf.Lerp(anguloFinal, anguloInicial, (transform.position.y - alturaInf) / (alturaSup - alturaInf)), currentRotationY, currentRotationZ));
    }
}
