using UnityEngine;
using System.Collections;

public class Termometro : MonoBehaviour {

	public enum NivelFrio { Normal, PocoFrio, Frio, MuchoFrio }
	public NivelFrio nivelFrio, nivelFrioAnterior;
	private Transform liquido, liquidoBase;
	public float r, g, b, vel, escalaY;
	private float escalaYInicial;
	private bool pausa;
	private bool activo = true;
	[Range(0.0f, 1f)]
	public float velocidad;
	[Range(0.0f, 1f)]
	public float velocidadTope;

	void Awake(){
		liquido = transform.FindChild("liquido");
		liquidoBase = transform.FindChild("base");
		NotificationCenter.DefaultCenter().AddObserver(this, "disminuirFrio");
		NotificationCenter.DefaultCenter().AddObserver(this, "actualizarTermometro");
		NotificationCenter.DefaultCenter().AddObserver(this, "activarPausa");
		NotificationCenter.DefaultCenter().AddObserver(this, "desactivarPausa");
	}

	void Start () {
		escalaYInicial = liquido.transform.localScale.y;
		escalaY = escalaYInicial;
		colorInicial();
		iTween.ValueTo(gameObject, iTween.Hash("from", velocidad, "to", velocidadTope, "time", 100, "easeType", iTween.EaseType.linear, "onupdate", "aumentarVelocidad"));
	}
	
	void OnLevelWasLoaded(){
		pausa = false;
	}
	
	void Update () {
		if (activo == true){
			if (pausa == false){
				escalaY = liquido.transform.localScale.y;
				if (escalaY > 0)
					liquido.transform.localScale = new Vector3(liquido.transform.localScale.x, escalaY - velocidad, liquido.transform.localScale.z);
				else if (escalaY <= 0){
					activo = false;
					NotificationCenter.DefaultCenter().PostNotification(this, "monoMuerto");
					NotificationCenter.DefaultCenter().PostNotification(this, "activarMenuMuerto");
				}
				controlarColores();
			}
		}
	}
	
	private void colorInicial(){
		iTween.ColorTo(liquido.gameObject, iTween.Hash ("color", Color.red, "time", 0.1f));
		iTween.ColorTo(liquidoBase.gameObject, iTween.Hash ("color", Color.red, "time", 0.1f));
	}
	
	private void controlarColores(){
		iTween.ColorUpdate(liquido.gameObject, iTween.Hash ("r", r, "g", g, "b", b, "time", 1));
		iTween.ColorUpdate(liquidoBase.gameObject, iTween.Hash ("r", r, "g", g, "b", b, "time", 1));
		if (escalaY >= 5.8f){
			nivelFrio = NivelFrio.Normal;
			r = 1; g = 0; b = 0;
		} else if (escalaY > 4.1f && escalaY < 5.8f){
			nivelFrio = NivelFrio.PocoFrio;
			r = 1; g = 0.92f; b = 0.016f;
		} else if (escalaY > 2.3f && escalaY < 4.1f){
			nivelFrio = NivelFrio.Frio;
			r = 0; g = 1; b = 1;
		} else if (escalaY > 0 && escalaY < 2.3f){
			nivelFrio = NivelFrio.MuchoFrio;
			r = 0; g = 1; b = 1;
		}
		
		if (nivelFrioAnterior != nivelFrio)
			NotificationCenter.DefaultCenter().PostNotification(this, "actualizarMonoFrio", nivelFrio);
		nivelFrioAnterior = nivelFrio;
	}
	
	private void disminuirFrio(Notification notification){
		if (liquido.transform.localScale.y < 4.5f)
			liquido.transform.localScale = new Vector3(liquido.transform.localScale.x, liquido.transform.localScale.y + 2.5f, liquido.transform.localScale.z);
	}
	
	private void actualizarTermometro(Notification notificacion){
		colorInicial();
		liquido.transform.localScale = new Vector3(liquido.transform.localScale.x, escalaYInicial, liquido.transform.localScale.z);
	}
	
	private void aumentarVelocidad(float nuevaVelocidad){
		velocidad = nuevaVelocidad;
	}

	void activarPausa(Notification notification){
		pausa = true;
	}
	
	void desactivarPausa(Notification notification){
		pausa = false;
	}
}
