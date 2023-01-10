// Este script controla el movimiento del jugador y las físicas dentro del juego

using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public bool drawDebugRaycasts = true;	//Se dibujan los rayos para ver si estamos en contacto con paredes, suelo etc

	[Header("Movement Properties")]
	public float speed = 8f;				//Velocidad del player
	public float crouchSpeedDivisor = 3f;	//Reducción de velocidad estando agachados
	public float coyoteDuration = .05f;		//Antes de caernos, cuanto tiempo tenemos para saltar en el aire
	public float maxFallSpeed = -25f;		//Velocidad máxima a la que el jugador puede caer

	[Header("Jump Properties")]
	public float jumpForce = 6.3f;			//Fuerza inicial del salto
	public float crouchJumpBoost = 2.5f;	//Salto potenciado estando agachados
	public float hangingJumpForce = 15f;	//Fuerza que se va a aplicar cuando estamos colgados agarrados de un muro
	public float jumpHoldForce = 1.9f;		//Fuerza incrementada cuando el botón de salto se mantiene pulsado
	public float jumpHoldDuration = .1f;	//Cuanto tiempo puede mantenerse la tecla de salto pulsada

	//Todo lo relacionado a la posición de los rayos(RayCast)
	[Header("Environment Check Properties")]
	public float footOffset = .4f;			//El Offset de los pies a izquierda y derecha
	public float eyeHeight = 1.5f;			//Rayos a la altura de los ojos
	public float reachOffset = .7f;			//El offset en X para garrarse a un muro
	public float headClearance = .5f;		//Espacio necesario sobre la cabeza del jugador
	public float groundDistance = .2f;		//La distancia a la que se considera que el jugador está tocando el suelo
	public float grabDistance = .4f;		//La distancia necesaria para poder agarrarse a los muros
	public LayerMask groundLayer;			//Esto contiene la capa para comprobar si hay suelo(La capa platforms)

	//Variables públicas para ver si:
	[Header ("Status Flags")]
	public bool isOnGround;					//El personaje está tocando el suelo
	public bool isJumping;					//Si el personaje esta saltando
	public bool isHanging;					//Si el personaje está agarrado
	public bool isCrouching;				//Si el personaje está agachado
	public bool isHeadBlocked;              //Si tenemos la cabeza bloqueada por tener una plataforma delante estando agachados

	PlayerInput input;						//Aqui guardamos las referencias al script PlayerInput
	BoxCollider2D bodyCollider;				//Aqui llamamos al collider(esto nos sirve por si esamos agachados, que el collider se "agache" tambien)
	Rigidbody2D rigidBody;					//Aqui llamamos al rigidbody(para aplicar la fuerza si estamos moviendonos, saltando etc)
	
	float jumpTime;							//Vaiable que guarda la duración del salto
	float coyoteTime;						//Variable que guarda hasta cuando se nos permite saltar no estando en contacto con ninguna plataforma
	float playerHeight;						//Variable que guarda la altura de nuestro jugador

	float originalXScale;					//Guardamos la dirección original a la que mira el jugador
	int direction = 1;						//Aqui la dirección en la que miramos(1 o -1 si es izquierda o derecha)

	//Aqui guardamos los colliders y los offset estando agachados o de pie
	Vector2 colliderStandSize;				//Tamaño del collider estando de pie
	Vector2 colliderStandOffset;			//El Offset del collider estando de pie
	Vector2 colliderCrouchSize;				//El tamaño del collider estando agachados
	Vector2 colliderCrouchOffset;			//El offset del collider estando de pie

	const float smallAmount = .05f;			//Variable constante que no cambiará nunca usada mas adelante


	void Start ()
	{
		//Obetenmos una referencia a los componentes antes mencionados
		input = GetComponent<PlayerInput>();
		rigidBody = GetComponent<Rigidbody2D>();
		bodyCollider = GetComponent<BoxCollider2D>();

		//Aqui guardamos la escala original del player en x
		originalXScale = transform.localScale.x;

		//Aqui guardamos la altura original del player cogiendola del tamaño del collider en y
		playerHeight = bodyCollider.size.y;

		//Guardamos el tamaño del collider y del offset estando de pie
		colliderStandSize = bodyCollider.size;
		colliderStandOffset = bodyCollider.offset;

		//Calculamos el amaño del collider y del offset estando agachados y lo guardamos
		colliderCrouchSize = new Vector2(bodyCollider.size.x, bodyCollider.size.y / 2f);
		colliderCrouchOffset = new Vector2(bodyCollider.offset.x, bodyCollider.offset.y / 2f);
	}

	void FixedUpdate()
	{
		//Llamamos a este método para comprobar si estamos en contacto con algo, saltando, agachados etc
		PhysicsCheck();

		//Métodos para procesar los movimientos del suelo y el aire
		GroundMovement();		
		MidAirMovement();
	}

	void PhysicsCheck()
	{
		//Empezamos asumiendo que el player no esta en el suelo y no tenemos la cabeza bloqueada
		isOnGround = false;
		isHeadBlocked = false;

		//Justo después lanzamos los raycast del pie derecho e izquierdo
		RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, 0f), Vector2.down, groundDistance);
		RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0f), Vector2.down, groundDistance);

		//Si estos raycast tocan suelo, el player está en el suelo
		if (leftCheck || rightCheck)
			isOnGround = true;

		//Lanzamos los raycast para ver si tenemos un bloque encima de la cabeza
		RaycastHit2D headCheck = Raycast(new Vector2(0f, bodyCollider.size.y), Vector2.up, headClearance);

		//Si los raycast de la cabeza entran en contacto con algo, tenemos la cabeza bloqueada
		if (headCheck)
			isHeadBlocked = true;

		//Determinamos la direccion a la que el personaje se agarra al muro(la dirección en la que estemos mirando)
		Vector2 grabDir = new Vector2(direction, 0f);

		//Aqui lanzamos estos tres raycast(blockedCheck, ledgeCheck y wallCheck) y si se cumple buscamos la oportunidad de agarrarnos
		RaycastHit2D blockedCheck = Raycast(new Vector2(footOffset * direction, playerHeight), grabDir, grabDistance);
		RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset * direction, playerHeight), Vector2.down, grabDistance);
		RaycastHit2D wallCheck = Raycast(new Vector2(footOffset * direction, eyeHeight), grabDir, grabDistance);

		//Si el player no esta en suelo y no esta agarrado previemente y esta cayendo y
		//entramos en contacto con un borde
		if (!isOnGround && !isHanging && rigidBody.velocity.y < 0f && 
			ledgeCheck && wallCheck && !blockedCheck)
		{ 
			//...tenemos un borde al que agarrarnos, con lo que guardamos la posicion actual...
			Vector3 pos = transform.position;
			//...al poder agarrarnos ya, movemos al player en X la distancia necesaria para agarrarnos...
			pos.x += (wallCheck.distance - smallAmount) * direction;
			//...lo mismo pero posicionando al player en la Y...
			pos.y -= ledgeCheck.distance;
			//...aplicamos la posicion en la plataforma donde nos vamos a agarrar...
			transform.position = pos;
			//...activamos el rigidbody a static...
			rigidBody.bodyType = RigidbodyType2D.Static;
			//...finalmente, se activa el isHanging porque se han cumplido todos los requisitos y estamos agarrados
			isHanging = true;
		}
	}

	void GroundMovement()
	{
		//Si estamos agarrados a un borde no podemos hacer nada
		if (isHanging)
			return;

		//Si estamos pulsando agacharche, no estamos agachados todavia y estamos tocando suelo, entonces nos agachamos
		if (input.crouchHeld && !isCrouching && !isJumping)
			Crouch();
		//Si estamos agachados pero ya no estamos manteniendo el botón de agacharse, nos levantamos
		else if (!input.crouchHeld && isCrouching)
			StandUp();
		//si estamos agachados pero no tocando suelo(saltando) nos levantamos
		else if (!isOnGround && isCrouching)
			StandUp();

		//Calculamos la velocidad que tenemos segun el input horizonta 1 o -1 o 0 si izquierda derecha o si no estamos pulsando nada
		float xVelocity = speed * input.horizontal;

		//Si cambiamos de dirección, le damos la vuelta al personaje
		if (xVelocity * direction < 0f)
			FlipCharacterDirection();

		//Si estamos agachados, se reduce la velocidad a la que nos movemos
		if (isCrouching)
			xVelocity /= crouchSpeedDivisor;

		//Aqui aplicamos la velicidad a la que vamos a nuestro rigidbody
		rigidBody.velocity = new Vector2(xVelocity, rigidBody.velocity.y);

		//Si estamos tocando suelo, calculamos hasta cuando vamos a poder saltar una vez no lo estemos tocando
		if (isOnGround)
			coyoteTime = Time.time + coyoteDuration;
	}

	void MidAirMovement()
	{
		//Si estamos agarrados a un borde(boton de agarrado pulsado)...
		if (isHanging)
		{
			//Si pulsamos el boton de agacharse
			if (input.crouchPressed)
			{
				//nos soltamos...
				isHanging = false;
				//ponemos el rigidbody en dynamic para que el personaje caiga
				rigidBody.bodyType = RigidbodyType2D.Dynamic;
				return;
			}

			//Si estamos saltando(boton de salto pulsado)
			if (input.jumpPressed)
			{
				//nos soltamos...
				isHanging = false;
				//ponemos el rigidbody en dynammic y aplicamos fuerza de salto
				rigidBody.bodyType = RigidbodyType2D.Dynamic;
				rigidBody.AddForce(new Vector2(0f, hangingJumpForce), ForceMode2D.Impulse);
				return;
			}
		}

		//Si se ha pulsado saltar y no estamos saltando previamente o si esta tocando suelo o si estamos en el aire pero todavia dentro de la variable
		//coyoteTime, entonces saltamos...
		if (input.jumpPressed && !isJumping && (isOnGround || coyoteTime > Time.time))
		{
			//...primero vamos a ver si estamos agachados y no tenemos la cabeza bloqueada...
			if (isCrouching && !isHeadBlocked)
			{
				//...si es asi, nos levantamos y le aplicamos la fuerza del salto incrementado estando agachados
				StandUp();
				rigidBody.AddForce(new Vector2(0f, crouchJumpBoost), ForceMode2D.Impulse);
			}

			//...el player esta saltando y ya no está tocando suelo...
			isOnGround = false;
			isJumping = true;

			//...guardamos la variable y le sumamos cuanto tiempo vamos a poder estar saltando...
			jumpTime = Time.time + jumpHoldDuration;

			//...le aplicamos la fuerza del salto al rigidbody...
			rigidBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

			//...y le decimos al AudioManager que active el sonido del salto
			AudioManager.PlayJumpAudio();
		}
		//Sino puede saltar, comprobamos si esta en estado de saltar...
		else if (isJumping)
		{
			//...si esta manteniendo la tecla de saltar, se aplica una fuerza incrementada para saltar más...
			if (input.jumpHeld)
				rigidBody.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);

			//...y si el tiempo del salta es menor que el tiempo que definimos al empezar el salto, terminamos el proceso de saltar
			if (jumpTime <= Time.time)
				isJumping = false;
		}

		//Si el player está cayendo en Y, reducimos la velocidad hasta maxFallSpeed
		if (rigidBody.velocity.y < maxFallSpeed)
			rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxFallSpeed);
	}

	void FlipCharacterDirection()
	{
		//Damos la vuelta al player cmabiando la variable de dirección
		direction *= -1;

		//Guardamos la escala actual
		Vector3 scale = transform.localScale;

		//Le cambiamos el valor en X 
		scale.x = originalXScale * direction;

		//Y le aplicamos la nueva escala
		transform.localScale = scale;
	}

	void Crouch()
	{
		//El player está agachado
		isCrouching = true;

		//Cambiamos el tamaño de los colliders si estamos agachados
		bodyCollider.size = colliderCrouchSize;
		bodyCollider.offset = colliderCrouchOffset;
	}

	void StandUp()
	{
		//Si el player tiene la cabeza bloqueada, no podemos levantarnos
		if (isHeadBlocked)
			return;

		//Si el jugador no esta agachado, nos levantamos
		isCrouching = false;
	
		//Restauramos el tamaño del collider que tiene el player estando de pie(el original)
		bodyCollider.size = colliderStandSize;
		bodyCollider.offset = colliderStandOffset;
	}


	//Estos métodos del Raycast() envuelven las físicas 2D y nos dan alguna funcionalidad extra
	RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length)
	{ 
		//LLamamos al método Raycast() y al groundLayer y devolvemos los resultados
		return Raycast(offset, rayDirection, length, groundLayer);
	}

	RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length, LayerMask mask)
	{
		//Guardamos la posición del player
		Vector2 pos = transform.position;

		//Lanzamos los rayos y guardamos el resultado
		RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, mask);

		if (drawDebugRaycasts)
		{
			//...determinamos el color basandonos en donde hacen contacto los raycast...
			Color color = hit ? Color.red : Color.green;
			//...y dibujamos el rayo en la escena
			Debug.DrawRay(pos + offset, rayDirection * length, color);
		}

		//Devolvemos los resultados del raycast
		return hit;
	}
}
