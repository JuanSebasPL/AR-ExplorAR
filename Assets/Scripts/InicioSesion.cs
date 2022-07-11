using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InicioSesion : MonoBehaviour {

    public InputField correo;
    public InputField contrasena;
    public Text mensaje_error;
    public bool estado;
    public Button btn_iniciarSesion;
    public GameObject load;

    AuthManager _authmanage = new AuthManager();

    private void Start () {

        //iniciando la base de datos
        _authmanage.InitializeFirebase();

        

        //revisar cuando se conecte con la firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread (continuation: task => {
            if (task.Exception != null) {
                mensaje_error.gameObject.SetActive (true);
                mensaje_error.text = "No tiene conexion a internet";
                return;
            }
            mensaje_error.gameObject.SetActive (false);
            estado = true;

        });

    }
    private void Update ()
     {
        //activar o desactivar el boton para iniciar sesion
        if (correo.text != "" && contrasena.text != "" && estado) {
            btn_iniciarSesion.interactable = true;
        } else {
            btn_iniciarSesion.interactable = false;
        }

        //manda a la otra screen si el usuario ya esta logeado
        if (_authmanage.user != null)
        {
           if (PlayerPrefs.GetInt("admin") == 1 ){
               SceneManager.LoadScene("Gestionar_Lugares");
            }else{
              SceneManager.LoadScene("Lista_Lugares");  
            }
            
        }

        if (mensaje_error.text != ""){
            load.SetActive(false);
        }

        
    }

    public void registrarse(){
        SceneManager.LoadScene("Registrar_Usuario");
    }

    public void iniciarSesion()
    {
        load.SetActive(true);
        _authmanage.inputFieldEmail = correo.text;
        _authmanage.inputFieldPasword = contrasena.text;
        _authmanage.LoginSession();
        mensaje_error.gameObject.SetActive(true);
        mensaje_error.text = _authmanage.errorMessage; 
        PlayerPrefs.SetString("contrasena", contrasena.text);
    }

}