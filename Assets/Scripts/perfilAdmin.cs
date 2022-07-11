using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class perfilAdmin : MonoBehaviour
{
    public InputField correo;
    public InputField contraseña;
    public InputField nombre; 
    public InputField pais; 
    public InputField cedula;
    public InputField celular;
    public Button btn_registrarse;
    public Text mensaje_error;
    public bool estado;

    public DatabaseReference reference;

    AuthManager _authmanage = new AuthManager();

    public Objeto perfil;
    // Start is called before the first frame update
    void Start()
    {
        //iniciando la base de datos
        _authmanage.InitializeFirebase();
        mensaje_error.gameObject.SetActive (false);
        estado = true;

        //revisar cuando se conecte con la firebase
        mensaje_error.text = "No tiene conexion a internet";
       
        //se crea la realtimedatabase
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://data-explorar.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        initialiceBd();

        perfil = GameObject.FindGameObjectWithTag("milugar").GetComponent<Objeto>();
        correo.text = perfil.correo;
        contraseña.text = PlayerPrefs.GetString("contrasena");
        nombre.text = perfil.a.nombre;
        pais.text = perfil.a.pais;
        cedula.text = perfil.a.Cedula;
        celular.text = perfil.a.Celular.ToString();

    }

    void Update()
    {
        //verificar que se haya llenado todos los campos
        if (correo.text != "" && contraseña.text != "" && nombre.text != "" && pais.text != "" && 
            cedula.text != "" && celular.text != "" && estado) 
        {
            btn_registrarse.interactable = true;
        } else {
            btn_registrarse.interactable = false;
        }

        //si se inscribió bien el auth que lo registre en la base de datos
        if (_authmanage.resultado_proceso){
           RewriteNewAdmin (perfil.iduser,nombre.text,pais.text, cedula.text, int.Parse(celular.text));  
        }
    }

    public void Actualizar()
    {
        //se registra en el Auth
        Debug.Log("entró a actualizar");
        _authmanage.ActualizarUsuario(correo.text, contraseña.text);
        mensaje_error.gameObject.SetActive(true);
        mensaje_error.text = _authmanage.errorMessage;     
    }
    
    //escribir el usuario en la base de datos
    private void  RewriteNewAdmin (string UserId, string name, string country, string id, int phone)
    {
        Admin admin = new Admin(name,country,id,phone);
        string json = JsonUtility.ToJson(admin);
        Debug.Log(json);
        reference.Child("Admins").Child(UserId).SetRawJsonValueAsync(json);

    }

    //inciar base de datos
    public void initialiceBd()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
        var dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available) {

        } else {
            mensaje_error.text = "texto";
            UnityEngine.Debug.LogError(System.String.Format(
            "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            // Firebase Unity SDK is not safe to use here.
        }
        });
    }

    // cuando oprimer boton cerrar sesion
    public void cerrarSesion(){
        _authmanage.auth.SignOut();
        Destroy(GameObject.FindGameObjectWithTag("milugar"));
        SceneManager.LoadScene("Interfaz_principal");
    }

    // cuando oprimer boton atras
    public void salir(){
        Destroy(GameObject.FindGameObjectWithTag("milugar"));
        SceneManager.LoadScene("Gestionar_Lugares"); 
    }
}
