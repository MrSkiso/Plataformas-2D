// Este script es puramente estético, sirve para determinar el comportamiento de las antorchas de pared

using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public float amount;	//Cantidad máxima de luz
    public float speed;		//Velocidad a la que cambia la intensidad de la luz
    
    Light localLight;		//Variable que referencia el componente luz de un objeto hijo
    float intensity;		//La intensidad del componente luz
	float offset;			//Un offset para que todas las luces parpadeen de forma diferente(es un valor aleatorio)

	void Awake()
	{
		
	}

	void Start()
    {
		//Cuandos se inicia se obtiene la referencia de la luz en su objeto hijo
		localLight = GetComponentInChildren<Light>();

		//Guarda la intensidad inicial de la luz(la actual) y le da un valor random entre 0 y 10000
        intensity = localLight.intensity;
        offset = Random.Range(0, 10000);
    }

	void Update ()
	{
		//Usando el PerlinNoise(obteniendo el valor en la coordenada X e Y de ese PerlinNoise) determinamos una cantidad random de intensidad de luz
		float amt = Mathf.PerlinNoise(Time.time * speed + offset, Time.time * speed + offset) * amount;
		localLight.intensity = intensity + amt;
	}
}
