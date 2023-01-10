// Este script maneja las animaciones del elemento UI del fader (La animación de cuando morimos) 

using UnityEngine;

public class SceneFader : MonoBehaviour
{
	Animator anim;		//Referencia al componente del Animator
	int fadeParamID;    //El ID del parámetro del animator del fader

	void Start()
	{
		//Obetenemos la referencia al componente Animator
		anim = GetComponent<Animator>();

		//Obtenemos el hash del parámetro de "Fade"
		fadeParamID = Animator.StringToHash("Fade");

		//Registramos esta escena(Scene Fader) en el game manager
		GameManager.RegisterSceneFader(this);
	}

	public void FadeSceneOut()
	{
		//Reproducimos la animacion de muerte en la UI
		anim.SetTrigger(fadeParamID);
	}
}
