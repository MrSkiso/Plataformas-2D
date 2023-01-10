using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMovement : MonoBehaviour
{
    public List<Transform> puntos;
    int puntoActual;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = puntos[0].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, puntos[puntoActual].position) < 0.1f)
        {
            puntoActual++;
            if(puntoActual >= puntos.Count)
            {
                puntoActual = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, puntos[puntoActual].position, speed * Time.deltaTime);
    }
}
