// Este sript controla la entrada a la puerta para que el game manager pueda indicar que has ganado

using UnityEngine;

public class WinZone : MonoBehaviour
{
	int playerLayer;    //La capa del player y su game object


	void Start()
	{
		//Obtenemos la referencia a la capa "Player" 
		playerLayer = LayerMask.NameToLayer("Player");
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		//Si la colisión no ha sido con el jugador, retrocedemos
		if (collision.gameObject.layer != playerLayer)
			return;

		//Escribimos "Player Won" en la consola y le decimos al Game Manager que el player ha ganado
		Debug.Log("Player Won!");
		GameManager.PlayerWon();
	}
}
