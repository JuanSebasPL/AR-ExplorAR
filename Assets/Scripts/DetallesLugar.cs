using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DetallesLugar : MonoBehaviour
{
    Objeto lugar;
    public Text titulo;
    public Text descripcion;
    public RawImage image;
    public RawImage btn_fav;
    bool contiene;
    List<string> lista= new List<string>();

    public DatabaseReference reference;
    
    // Start is called before the first frame update
    void Start()
    {   
        //se crea la realtimedatabase
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://data-explorar.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        initialiceBd();

        lugar= GameObject.FindGameObjectWithTag("milugar").GetComponent<Objeto>();
        titulo.text = lugar.lugar.titulo;

        lista = lugar.u.listaLugaresFav;
        
        Texture2D img = new Texture2D(450, 400);
        img.LoadImage(lugar.imagen);         
        img.Apply();
        image.texture = img;

        descripcion.text = "Dirección: " + lugar.lugar.direccion 
        + System.Environment.NewLine +System.Environment.NewLine  + lugar.lugar.descripcion;

        contiene = false;
        //verificar si el usuario tiene como favorito el lugar
        foreach(string x in lista){
            if (x == lugar.numItem.ToString()){
                contiene = true;
            }
        }

        if(contiene)
        {
            btn_fav.color = Color.white;
        }else
        {
            btn_fav.color = Color.black;
        }

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void CambiarScena(){
        Destroy(GameObject.FindGameObjectWithTag("milugar"));
        SceneManager.LoadScene("Lista_Lugares"); 
    }

    public void onclick()
    {
        if (contiene)
        {
            lista.Remove(lugar.numItem.ToString());
            btn_fav.color = Color.black; 
            contiene = false;
            lugar.u.listaLugaresFav = lista;
            writeList();  

        }else{
            Debug.Log("anadio el item " + lugar.numItem);
            lista.Add(lugar.numItem.ToString());  
            contiene = true;         
            btn_fav.color = Color.white;
            lugar.u.listaLugaresFav = lista;
            writeList();
        }
        
    }

        //inciar base de datos
    public void initialiceBd()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
        var dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available) {

        } else {
            UnityEngine.Debug.LogError(System.String.Format(
            "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            // Firebase Unity SDK is not safe to use here.
        }
        });
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
}
