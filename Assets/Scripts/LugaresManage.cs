using System;
using System.Collections;
using System.Collections.Generic;
using ARLocation;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using System.Threading.Tasks;

public class LugaresManage  
{

    public List<Lugar> lista_lugares = new List<Lugar>();
    public Lugar lugar = new Lugar();
    public byte [] fileContents; 
    public DatabaseReference reference;
    public bool procesoCompletado = false;
    public bool procesoLugaresCompletado = false;
    public bool procesoImgCompletado = false;
    public bool traerTodaInfo = false;
    public bool inicio=false;
    public string path;

    //inciar base de datos
    public void initialiceBd()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://data-explorar.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        

        //verificar la conexion con la base de datos
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
            } else {
                UnityEngine.Debug.LogError(System.String.Format(
                "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
                Debug.Log("hubo un error al iniciar la data base");
            }
            inicio = true;
        });

    }

    public void ruta(){
        path = Application.persistentDataPath;
    }

    public void traerLugar(int num)
    {   
        FirebaseDatabase.DefaultInstance.GetReference("Lugares").Child("Lugar "+ num).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                Debug.Log("error al obtener referencia");
            // Handle the error...
            }
            else if (task.IsCompleted) {
            // Do something with snapshot... 
            DataSnapshot snapshot = task.Result;
            string json = snapshot.GetRawJsonValue();
            lugar = JsonUtility.FromJson<Lugar>(json);
            Debug.Log("trajó el lugar: "+ lugar.titulo);
            procesoCompletado = true;
            }
        });
    }

    public void traerLugares()
    {    
        ;
        FirebaseDatabase.DefaultInstance.GetReference("Lugares").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                Debug.Log("error al obtener referencia");
            // Handle the error...
            }
            else if (task.IsCompleted) {
            // Do something with snapshot... 
            DataSnapshot snapshot = task.Result;
            Debug.Log("entró al snapshot traer lugares");
            addLugarToList(snapshot);
            }
        });
    }

    //agregr cada lugar a la lista de lugares traidos de la database
    public void addLugarToList (DataSnapshot _dataSnapshot)
    {
        foreach (DataSnapshot _child in _dataSnapshot.Children )
        { 
           // Location ubicacion = (Location)_child.Child("ubicacion");
            string json = _child.GetRawJsonValue();
            Lugar _Lugar = JsonUtility.FromJson<Lugar>(json);
            lista_lugares.Add(_Lugar);
        }   
        procesoLugaresCompletado = true;
        Debug.Log("Añadió un total de " + lista_lugares.Count + " elementos a la lista");  
    }

    //escribir un lugar nuevo
    public void writeNewLugar(Lugar _location, string n)
    { 
        string json = JsonUtility.ToJson(_location);
        Debug.Log(json);
        reference.Child("Lugares").Child("Lugar "+ n).SetRawJsonValueAsync(json);
    }

    public void traer_imagen(int refer)
    {
        Debug.Log("entro a traer la imagen "+ refer);
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        Firebase.Storage.StorageReference images_ref = storage.GetReferenceFromUrl(
        "gs://data-explorar.appspot.com/imagenes_lugares");

        Firebase.Storage.StorageReference image = images_ref.Child("Lugar "+refer+".jpg");
        const long maxAllowedSize = 10 * 1024 * 1024;
       image.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) => 
        {

            if (task.IsFaulted || task.IsCanceled) {
            Debug.Log(task.Exception.ToString());
            Debug.Log("termino de traer imagenes");
            traerTodaInfo = true;
            // Uh-oh, an error occurred!
        } else {
            fileContents = task.Result;
            Debug.Log("Finished downloading!");
            procesoImgCompletado = true;
        }
        });
    }

    public void traer_archivo(int refer){

        Debug.Log("entro a traer la imagen "+ refer);
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        Firebase.Storage.StorageReference images_ref = storage.GetReferenceFromUrl(
        "gs://data-explorar.appspot.com/imagenes_lugares");

        Firebase.Storage.StorageReference image = images_ref.Child("Lugar "+refer+".jpg");

        string filePath = Application.persistentDataPath;

        if (!System.IO.Directory.Exists(path + "Lugares/")){
            System.IO.Directory.CreateDirectory(path + "Lugares/");
        }
        // Create local filesystem URL
        string local_url = path + "Lugares/Lugar"+ refer +".jpg";

        // Download to the local filesystem
        image.GetFileAsync(local_url).ContinueWith(task => {
            if (!task.IsFaulted && !task.IsCanceled) {
                procesoImgCompletado = true;
                Debug.Log("File downloaded.");
            }else if (task.IsFaulted){
                Debug.Log("termino de traer imagenes");
                traerTodaInfo = true;
            }
        });
    }

}
