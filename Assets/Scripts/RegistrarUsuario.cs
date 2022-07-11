using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegistrarUsuario : MonoBehaviour
{
    public InputField correo;
    public InputField contraseña;
    public InputField nombre; 
    public InputField pais; 
    public Button btn_registrarse;
    public Text mensaje_error;
    public GameObject load;
    List<string> lista = new List<string>();

    public DatabaseReference reference;

    AuthManager _authmanage = new AuthManager();
    // Start is called before the first frame update
    void Start()
    {
        //iniciando la base de datos
        _authmanage.InitializeFirebase();

        //revisar cuando se conecte con la firebase

       
        //se crea la realtimedatabase
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://data-explorar.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        initialiceBd();

    }

    void Update()
    {
        //verificar que se haya llenado todos los campos
        if (correo.text != "" && contraseña.text != "" && nombre.text != "" && pais.text != "" ) {
            btn_registrarse.interactable = true;
        } else {
            btn_registrarse.interactable = false;
        }

        //si se inscribió bien el auth que lo registre en la base de datos
        if (_authmanage.resultado_proceso)
        {
            _authmanage.resultado_proceso = false;
            writeNewUser (_authmanage.user.UserId,nombre.text,lista,pais.text);  
            SceneManager.LoadScene("Lista_Lugares");  
           
        }else if (_authmanage.errorMessage != ""){
            load.SetActive(false);
            mensaje_error.text = _authmanage.errorMessage;
        }
    }

    public void registrar()
    {
        //se registra en el Auth
        load.SetActive(true);
        Debug.Log("entró a registrar");
        _authmanage.inputFieldEmail = correo.text;
        _authmanage.inputFieldPasword = contraseña.text;
        _authmanage.CreateUser();
               
    }

    public void atras(){
        SceneManager.LoadScene("Interfaz_Principal");
    }
    
    //escribir el usuario en la base de datos
    private void writeNewUser(string UserId, string name, List<string> list, string country)
    {
        Usuario usuario = new Usuario(name,list, list,country);
        string json = JsonUtility.ToJson(usuario);
        Debug.Log(json);
        reference.Child("Users").Child(UserId).SetRawJsonValueAsync(json);

    }

    //inciar base de datos
    public void initialiceBd()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
        var dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available) {

        } else {
            mensaje_error.text = "No tiene conexion a internet";
            UnityEngine.Debug.LogError(System.String.Format(
            "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            // Firebase Unity SDK is not safe to use here.
        }
        });
    }

    
    
}
