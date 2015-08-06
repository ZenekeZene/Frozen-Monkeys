using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlJuego : MonoBehaviour {

	private bool pausa;
	private GameObject camaraPausa, camaraMuerto;
	private AudioSource[] sonidos;
	private AudioSource sonidoMonoVestido, sonidoGameOver;
	
	void Awake(){
		sonidos = GetComponents<AudioSource>();
		if (sonidos.Length > 0){
			sonidoMonoVestido = sonidos[0];
			sonidoGameOver = sonidos[1];
		}
	}
	
	void Start () {
		NotificationCenter.DefaultCenter().AddObserver(this, "activarMenuMuerto");
		pausa = false;
		Time.timeScale = 1;
		camaraPausa = GameObject.Find("CamaraPause");
		if (camaraPausa != null)
			camaraPausa.SetActive(false);
		camaraMuerto = GameObject.Find("CamaraMuerto");
		if (camaraMuerto != null)
			camaraMuerto.SetActive(false);
	}
	
	public void controlarPausar(){
		if (pausa == false){
			NotificationCenter.DefaultCenter().PostNotification(this, "activarPausa");
			activarPausa();
			pausa = true;
		} else {
			desactivarPausa();
			NotificationCenter.DefaultCenter().PostNotification(this, "desactivarPausa");
			pausa = false;
		}
	}
	
	void activarPausa(){
		iTween.Pause();
		camaraPausa.SetActive(true);
		Time.timeScale = 0;
		
	}
	
	void desactivarPausa(){
		iTween.Resume();
		camaraPausa.SetActive(false);
		Time.timeScale = 1;
		
	}
	
	public void continuar(){
		desactivarPausa();
		NotificationCenter.DefaultCenter().PostNotification(this, "desactivarPausa");
	}
	
	public void salirAmenu(){
		Time.timeScale = 1;
		iTween.Destructor();
		Application.LoadLevel ("menu");
	}
	
	public void jugar(){
		iTween.Destructor();
		Application.LoadLevel ("escena");
	}
	
	private void activarMenuMuerto(Notification notification){
		iTween.Destructor();
		sonidoGameOver.Play ();		
		camaraMuerto.SetActive(true);
		GameObject.Find ("btnPausa").SetActive(false);
		Time.timeScale = 0;
		int puntuacion = int.Parse(GameObject.Find ("txtPuntuacion").GetComponent<Text>().text);
		GameObject.Find ("txtPuntuacionFinal").GetComponent<Text>().text = puntuacion.ToString();
		int puntuacionMaximaAnterior = PlayerPrefs.GetInt("puntuacionMaxima");
		if (puntuacionMaximaAnterior < puntuacion){
			puntuacionMaximaAnterior = puntuacion;
			PlayerPrefs.SetInt("puntuacionMaxima", puntuacionMaximaAnterior);
			PlayerPrefs.Save ();
		}
		GameObject.Find ("txtPuntuacionMaxima").GetComponent<Text>().text = puntuacionMaximaAnterior.ToString();
	}
	
	public void quitarJuego(){
		Application.Quit();
	}
}
