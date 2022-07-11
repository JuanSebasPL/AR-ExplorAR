using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class administrar_lugares : MonoBehaviour
{
    public GameObject padre;
    public GameObject Lugares;
    public Text titulo;
    public Text descripcion;
    public RawImage image;
    public Text numero;
    public Text seleccionNumero;
    public int cantItems;
    public Text nombre_user;
    public Text cedula_user;

    private FirebaseDatabase _database;

    private string displayName;

    public Objeto objeto;

    public AuthManager _authmanage = new AuthManager();
    public LugaresManage _lugarManage = new LugaresManage();
    public List<Lugar> listaLugares = new List<Lugar>();

    public DatabaseReference reference;

 
    // Start is called before the first frame update
    void Start()
    {
        //iniciando la base de datos auth
        _authmanage.InitializeFirebase();
        cantItems = 1;
        //iniciar database de lugar
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://data-explorar.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
            } else {
                UnityEngine.Debug.LogError(System.String.Format(
                "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
                Debug.Log("hubo un eror al iniciar la data base");
            }
            _lugarManage.traerLugar(cantItems);
            _lugarManage.traer_imagen(cantItems);
            _authmanage.traerAdmin();
 

            
        });
    }

    // Update is called once per frame
    void Update()
    {
        //comprobar si ya hay lugares
        if(_lugarManage.procesoImgCompletado)  
        {   
            _lugarManage.procesoImgCompletado=false;
            agregarDatos(_lugarManage.lugar, _lugarManage.fileContents);
        }

        //comprobar si ya hay usuario
        if (_authmanage.procesoadminCompletado)
        {
            _authmanage.procesoadminCompletado = false;
            nombre_user.text = _authmanage.admin.nombre;
            cedula_user.text = ("ID: "+ _authmanage.admin.cedula + " - " + _authmanage.admin.pais);
           
            if(nombre_user.text == ""){
                PlayerPrefs.SetInt("admin", 0);
                SceneManager.LoadScene("Interfaz_Principal");
            }
        }


        //comprobar si presionaron editar un objeto
        if (seleccionNumero.text != ""){
            
            if (seleccionNumero.text == "0")
            {
            }else
            {
                int x = int.Parse(seleccionNumero.text) -1;
                objeto.numItem = int.Parse( seleccionNumero.text);
                objeto.lugar = listaLugares[objeto.numItem-1];
            }
            DontDestroyOnLoad(objeto.gameObject);
            SceneManager.LoadScene("Anadir_Lugar");            
        }

    }


    public void agregarDatos(Lugar _lugar, byte[] _file)
    {
        //anade lugar a la lista
        listaLugares.Add(_lugar);

        titulo.text =  _lugar.titulo;
        descripcion.text = "Direccion: " + _lugar.direccion ;
        numero.text = cantItems.ToString();

        //mapeando el file a textura
        Texture2D img = new Texture2D(355, 355);
        img.LoadImage(_file);         
        img.Apply();
        image.texture = img;
        Debug.Log("anadio textura");

        GameObject var = Instantiate(Lugares);
        var.transform.position = new Vector3(0, -120-(250*(cantItems-1)), 0);    
        var.transform.SetParent(padre.transform, false);
        var.SetActive(true); 

        cantItems ++;
        _lugarManage.traerLugar(cantItems);
        _lugarManage.traer_imagen(cantItems);
    }

    // funcion para anadir un nuevo lugar
    public void Nuevo(){
        seleccionNumero.text = "0";
    }

    // cuando oprimer boton cerrar sesion
    public void cerrarSesion(){
        _authmanage.auth.SignOut();
        PlayerPrefs.SetInt("admin", 0);
        SceneManager.LoadScene("Interfaz_principal");
    }


}
