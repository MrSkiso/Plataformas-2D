// Este script controla los orbes coleccionables, es el responsable de detectar la colisión This script controls the orb collectables. It is responsible for detecting collision
// del orbe con el player y mandárselo al game manager
using UnityEngine;

public class Orb : MonoBehaviour
{
	public GameObject explosionVFXPrefab;	//Los efectos visuales del orbe siendo tocado por el playerThe visual effects for orb collection

	int playerLayer;						//La capa Player del gameobject


	void Start()
	{
		//Obetenemos la referencia a la capa "Player"
		playerLayer = LayerMask.NameToLayer("Player");

		//Registramos el orbe en el game manager 
		GameManager.RegisterOrb(this);
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		//Si el objeto no esta en contacto con la capa Player, no hacemos nada
		if (collision.gameObject.layer != playerLayer)
			return;

		//El orbe ha sido tocado por eñ player, instanciamos la explosion
		Instantiate(explosionVFXPrefab, transform.position, transform.rotation);
		
		//Decimos al audio manager que reproduzca el sonido del orbe siendo tocado
		AudioManager.PlayOrbCollectionAudio();

		//Decimos al game manager qye el orbe ha sido tocado y recolectado
		GameManager.PlayerGrabbedOrb(this);

		//Desactivamos el orbe que ha sido recolectado
		gameObject.SetActive(false);
	}
}
