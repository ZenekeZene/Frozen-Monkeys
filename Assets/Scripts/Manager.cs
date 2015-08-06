using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Manager : MonoBehaviour {

    private List<GameObject> colaMonos;
    public GameObject monoPrefab;	
	private int contadorMonos;
	public Text txtPuntuacion;
	private Transform nodoOrigen;
	
	void Awake () {
		
	}

	void Start () {
		empezar ();
	}
	
	private void empezar(){
		colaMonos = new List<GameObject>();
		nodoOrigen = transform.FindChild("nodoOrigen");
		generarMonos(2);
		activarMono();
		NotificationCenter.DefaultCenter().AddObserver(this, "monoVestido");
		contadorMonos = 0;
		txtPuntuacion.text  = contadorMonos.ToString();
		NotificationCenter.DefaultCenter().PostNotification(this, "desactivarPausa");
	}

    private void generarMonos(int tam){
        for (int i = 0; i < tam; i++){
            GameObject mono = Instantiate(monoPrefab, nodoOrigen.position, Quaternion.identity) as GameObject;
			NotificationCenter.DefaultCenter().PostNotification(this, "actualizarTermometro");
            colaMonos.Add(mono);
			contadorMonos += 1;
			actualizarMarcador();
        }
    }

    private void activarMono(){
        colaMonos[0].GetComponent<ControlMono>().activar();
        colaMonos.Remove(colaMonos[0]);
    }

    private void monoVestido(Notification notification){
        activarMono();
        generarMonos(1);
        audio.Play();
    }

	private void actualizarMarcador (){
		txtPuntuacion.text = contadorMonos.ToString();
	}
}
