// Este script detecta las colisiones del player con las trampas y las manda ak game game maneger cuando el player muere  


using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	public GameObject deathVFXPrefab;	//Los efectos visuales de la muerte del jugador

	bool isAlive = true;				//Almacena el estado del player vivo
	int trapsLayer;						//Activamos la capa traps


	void Start()
	{
		//Obtenemos la referencia a la capa "Traps"
		trapsLayer = LayerMask.NameToLayer("Traps");
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		//Si el objeo no esta en la caoa traps o si el jugador no esta vivo previamente, retrocedemos
		if (collision.gameObject.layer != trapsLayer || !isAlive)
			return;

		//Si la trampa ha sido tocada,ponemos el estado del player de vivo en falso
		isAlive = false;

		//Instanciamos las particulas de muerte al player
		Instantiate(deathVFXPrefab, transform.position, transform.rotation);

		//Desactivamos el game object del player
		gameObject.SetActive(false);

		//Decimos al Game Manager que el player ha muerto y al Audio Manager que reproduzca el audio de la muerte
		GameManager.PlayerDied();
		AudioManager.PlayDeathAudio();
	}
}
