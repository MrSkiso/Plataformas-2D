// Este script maneja los inputs del player.
// Con este script mantengo los inputs de Update() sincronizados con el FixedUpdate()(mantener sincronizadas las pulsaciones de teclas entre ambos)

using UnityEngine;

//Primero nos aseguramos que este script funciona antes que todos los scripts del player para prevenir inputs con lag

[DefaultExecutionOrder(-100)]
public class PlayerInput : MonoBehaviour
{

	[HideInInspector] public float horizontal;		//Float que almacena el input horizontal
	[HideInInspector] public bool jumpHeld;			//Bool que almacena la información del botón de salto manteniendo presionado
	[HideInInspector] public bool jumpPressed;		//Bool que almacena la información de cuando pulsamos el botón de salto
	[HideInInspector] public bool crouchHeld;		//Bool que almacena la información del botón de agachado manteniendo presionado
	[HideInInspector] public bool crouchPressed;	//Bool que almacena la información de cuando pulsamos el botón de agacharse
	
	bool readyToClear;								//Bool que mantiene los input sincronizados


	void Update()
	{
		//Limpiamos los valore inpust existentes(los valores floats y bools de arriba)
		ClearInput();

		//Si el GameManager dice game over, salimos
		if (GameManager.IsGameOver())
			return;

		//Procesamos los inputs del ratón y teclado
		ProcessInputs();

		//Valor que nos dice si nos movemos a la izquierda o derecha segun el input horizontal esté entre -1 y 1
		horizontal = Mathf.Clamp(horizontal, -1f, 1f);
	}

	void FixedUpdate()
	{
		//El FixedUpdate() se actualiza después del Update(), con lo que cuando se active el FixedUpdate() se pondrá el readyToClear en verdadero y
		//la siguiente vez que se active el Update() se resetearán los inputs con el ClearInput()
		readyToClear = true;
	}

	void ClearInput()
	{
		//Al llamar a este método ClearInput,si no esta listo para limpiar los inputs, salimos
		if (!readyToClear)
			return;

		//Reseteamos todos los inputs
		horizontal		= 0f;
		jumpPressed		= false;
		jumpHeld		= false;
		crouchPressed	= false;
		crouchHeld		= false;

		readyToClear	= false;
	}

	void ProcessInputs()
	{
		//Aqui acumulamos el input horizontal axis, que se va incrementando
		horizontal		+= Input.GetAxis("Horizontal");

		//Aqui acumulamos los inputs de las teclas
		jumpPressed		= jumpPressed || Input.GetButtonDown("Jump");
		jumpHeld		= jumpHeld || Input.GetButton("Jump");

		crouchPressed	= crouchPressed || Input.GetButtonDown("Crouch");
		crouchHeld		= crouchHeld || Input.GetButton("Crouch");
	}

}
