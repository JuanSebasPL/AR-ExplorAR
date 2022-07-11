using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Recompensa : MonoBehaviour
{
    public Button btn;
    public GameObject padre;
    public DatabaseReference reference;
    AuthManager _authmanage = new AuthManager();

    public Objeto lugar;

    List<string> lista= new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        //iniciando la base de datos
        _authmanage.InitializeFirebase();
       
        //se crea la realtimedatabase
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://data-explorar.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        initialiceBd();

        lugar = GameObject.FindGameObjectWithTag("milugar").GetComponent<Objeto>();

        lista = lugar.u.listaLugaresVisitados;

        foreach(string x in lista){
            if (x == padre.name){
                btn.interactable = false;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void onclick()
    {
        lista.Add(padre.name);          
        btn.interactable = false;
        lugar.u.listaLugaresVisitados = lista;
        lugar.u.puntos = lugar.u.puntos + 200;
        writeList();
    }

    //escribir el usuario en la base de datos
    private void writeList()
    {
        string jsonuser = JsonUtility.ToJson(lugar.u);
        PlayerPrefs.SetString("usuario", jsonuser);
        string json = JsonUtility.ToJson(lugar.u);
        Debug.Log(json);
        reference.Child("Users").Child(lugar.iduser).SetRawJsonValueAsync(json);

    }

    //inciar base de datos
    public void initialiceBd()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
        var dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available) {

        } else {
            btn.interactable = false;
            UnityEngine.Debug.LogError(System.String.Format(
            "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            // Firebase Unity SDK is not safe to use here.
        }
        });
    }

    public void CambiarScena(){
        Destroy(GameObject.FindGameObjectWithTag("milugar"));
        SceneManager.LoadScene("Lista_Lugares"); 
    }
}
