using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class administar_lugar : MonoBehaviour
{
    public InputField titulo; 
    public InputField descripcion; 
    public InputField direccion; 
    public InputField altitud; 
    public InputField latitud; 
    public InputField longitud;
    public InputField item; 

    public Button btn_guardar;

    public Objeto lugar;
    Lugar l = new Lugar();
    public bool find=false;
    public int result;

    public DatabaseReference reference;

    LugaresManage _lugarManage = new LugaresManage();

    // Start is called before the first frame update
    void Start()
    {
        _lugarManage.initialiceBd();
        lugar= GameObject.FindGameObjectWithTag("milugar").GetComponent<Objeto>();

        l = lugar.lugar; 
        titulo.text = l.titulo;
        descripcion.text = l.descripcion;
        direccion.text = l.direccion;
        altitud.text = l.ubicacion.Altitude.ToString();
        latitud.text = l.ubicacion.Latitude.ToString();
        longitud.text = l.ubicacion.Longitude.ToString();
        item.text = lugar.numItem.ToString();
       

    }

    // Update is called once per frame
    void Update()
    {
        //activar boton cuando todos los campos esten llenos
        if(titulo.text != "" && descripcion.text != "" && direccion.text != "" && latitud.text != "" 
        && longitud.text != "" && item.text != "" && find ){
            btn_guardar.interactable = true;
        }

        if (_lugarManage.inicio == true){
            _lugarManage.inicio = false;
            _lugarManage.traerLugares();
        }

        if(_lugarManage.procesoCompletado){
            find = true;
            if(item.text == ""){
                result = _lugarManage.lista_lugares.Count + 1;
                item.text = result.ToString();
            }
        }
    }

    // guarda el lugar cuando se presione el boton
    public void GuardarLugar(){
        Lugar _lugar = new Lugar(titulo.text, descripcion.text, direccion.text, 
        double.Parse(altitud.text), double.Parse(latitud.text), double.Parse(longitud.text));
        _lugarManage.writeNewLugar(_lugar, item.text);
        titulo.text = "";
        descripcion.text = "";
        direccion.text = "";
        altitud.text = 0.ToString();
        latitud.text = "";
        longitud.text = "";
        result ++;
        item.text = result.ToString();
       
    }

    public void salir(){
        Destroy(lugar.gameObject);
        SceneManager.LoadScene("Gestionar_Lugares");
    }



}
