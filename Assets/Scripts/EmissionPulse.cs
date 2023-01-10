// Este script controla la intensidad de los pulsos de luz de los elementos del background, como las estatuas o los logos de Unity
//es puramente estético

using UnityEngine;

public class EmissionPulse : MonoBehaviour
{
	public float maxIntensity = 15f;	//La emisión máxima de intensidad
	public float damping = 2f;			//Variable que controla la velocidad de los pulsos de luz

	Material material;					//Referencia al material para poder cambiarle propiedades
	int emissionColorProperty;			//El id del campo de la propiedad de emisión


	void Start ()
	{
		//Obtenemos la refrencia al componente Renderer y gracias a esto accedemos al material
		Renderer renderer = GetComponent<Renderer>();
		material = renderer.material;

		//Luego obtenemos el identificador de la propiedad emission color
		emissionColorProperty = Shader.PropertyToID("_EmissionColor");
	}

	void Update()
	{
		//Calculamos el valor de emisión basandonos en el tiempo y la intensidad
		float emission = Mathf.PingPong(Time.time * damping, maxIntensity);

		//Y convertimos en un valor de emisión de color
		Color finalColor = Color.white * emission;

		//Y aplicamos el color al material
		material.SetColor(emissionColorProperty, finalColor);
	}
}
