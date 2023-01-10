// Este script controla el funcionamineto de la puerta para que el player gane

using UnityEngine;

public class Door : MonoBehaviour
{
	Animator anim;			//Referencia al componente del Animator
	int openParameterID;	//El ID del parámetro del animator que abre la puerta


	void Start()
	{
		//Obtenemos una referencia al Animator component
		anim = GetComponent<Animator>();

		//Obtenemos el hash del parámetro "Open"
		openParameterID = Animator.StringToHash("Open");

		//Registramos la puerta en el Game Manager
		GameManager.RegisterDoor(this);
	}

	public void Open()
	{
		//Reproducimos la animación de la puerta abriéndose
		anim.SetTrigger(openParameterID);
		AudioManager.PlayDoorOpenAudio();
	}
}
