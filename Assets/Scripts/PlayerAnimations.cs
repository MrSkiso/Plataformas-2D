// Este script controla las animaciones del player, con el fin de tenerlo todo más ordenador, lo he hecho en un script separado

using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
	//VAriables privadas
	PlayerMovement movement;	//Referencia al componente del script PlayerMovement 
	Rigidbody2D rigidBody;		//Referencia al componente Rigidbody2D						
	PlayerInput input;			//Referencia al componente del script PlayerInput
	Animator anim;				//Referencia al componente del Animator

	int hangingParamID;			//ID del parámetro IsHanging of the isHanging parameter
	int groundParamID;			//ID del parámetro isOnGround of the isOnGround parameter
	int crouchParamID;			//ID del parámetro isCrouching of the isCrouching parameter
	int speedParamID;			//ID del parámetro speed
	int fallParamID;			//ID del parámetro verticalVelocity


	void Start()
	{
		//Obtenemos los hashes de los parámetros, esto es más eficiente que pasarle los strings al animator
		hangingParamID = Animator.StringToHash("isHanging");
		groundParamID = Animator.StringToHash("isOnGround");
		crouchParamID = Animator.StringToHash("isCrouching");
		speedParamID = Animator.StringToHash("speed");
		fallParamID = Animator.StringToHash("verticalVelocity");

		//Aqui hacemos referencia al transform del objeto padre para acceder al GetComponent
		Transform parent = transform.parent;

		//Obtenemos las referencias de los componentes necesarios
		movement	= parent.GetComponent<PlayerMovement>();
		rigidBody	= parent.GetComponent<Rigidbody2D>();
		input		= parent.GetComponent<PlayerInput>();
		anim		= GetComponent<Animator>();
		
		//Si alguno de los componentes necesarios no existe...
		if(movement == null || rigidBody == null || input == null || anim == null)
		{
			//...Guardamos el error y borramos el componente
			Debug.LogError("A needed component is missing from the player");
			Destroy(this);
		}
	}

	void Update()
	{
		//Actualizamos el Animator con los valores apropiados
		anim.SetBool(hangingParamID, movement.isHanging);
		anim.SetBool(groundParamID, movement.isOnGround);
		anim.SetBool(crouchParamID, movement.isCrouching);
		anim.SetFloat(fallParamID, rigidBody.velocity.y);

		//Use the absolute value of speed so that we only pass in positive numbers
		anim.SetFloat(speedParamID, Mathf.Abs(input.horizontal));
	}

    //Estos métodos son llamados desde las animaciones en si, esto mantiene el sonido de andar sincronizado con los visuales
    public void StepAudio()
	{
		//Le decimos al Audio Manager que reproduzca el sonido de andar
		AudioManager.PlayFootstepAudio();
	}

	//Estos métodos son llamados desde las animaciones en si, esto mantiene el sonido de andar sincronizado con los visuales
	public void CrouchStepAudio()
	{
		//Le decimos al Audio Manager que reproduzca el sonido de andar agachados
		AudioManager.PlayCrouchFootstepAudio();
	}
}
