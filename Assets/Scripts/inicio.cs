using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class inicio : MonoBehaviour
{   
    public GameObject Todos;
    public GameObject fav;
    public GameObject visitados;
    public GameObject Lugares;
    public Text titulo;
    public Text descripcion;
    public RawImage image;
    public Text numero;
    public Text seleccionNumero;
    public Dropdown seleccion;
    public int cantItems;
    public int cantItems2;
    public int cantItems3;
    public Text nombre_user;
    public Text puntos_user;

    public Objeto _objeto;

    private FirebaseDatabase _database;

    private string displayName;

    public AuthManager _authmanage = new AuthManager();
    public LugaresManage _lugarManage = new LugaresManage();
    public List<Lugar> listaLugares = new List<Lugar>();
    public List<byte[]> listaimages = new List<byte[]>();

    public DatabaseReference reference;

    public bool decision= true;
    public GameObject carga ;
    public Text loading;


    

    // Start is called before the first frame update
    void Start()
    {
        cantItems=1;
        cantItems2=1;
        cantItems3=1;
        //iniciando la base de datos auth
        _authmanage.InitializeFirebase();

        _lugarManage.ruta();
        if(!System.IO.File.Exists(_lugarManage.path+ "Lugares/Lugar1.jpg"))
        {
          decision = true;
        }else
        {
            decision = false;
        }
        carga.SetActive(decision);
        iniciardatos();
    }

    public void iniciardatos(){

        if (decision)
        {
            Debug.Log("si, esta vacio");
            iniciarDescarga();
        }else
        {
            Debug.Log("no esta vacio");

            //trae el usuario del player prefs
            string jsonu = PlayerPrefs.GetString("usuario");
            _authmanage.usuario = JsonUtility.FromJson<Usuario>(jsonu);
            nombre_user.text = _authmanage.usuario.nombre;
            puntos_user.text = ("Puntos: "+ _authmanage.usuario.puntos.ToString());

            //trae los items del playerprefs
            for (int i=0; i< PlayerPrefs.GetInt("cantidadItems"); i++)
            {
                string json = PlayerPrefs.GetString("listaLugares"+i);
                Lugar lugarsito = new Lugar();
                lugarsito = JsonUtility.FromJson<Lugar>(json);
                agregarDatos(lugarsito);
            }

            
        }
       
    }

    public void iniciarDescarga(){

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
            _lugarManage.traerLugares();
            _lugarManage.traerLugar(cantItems);
            _lugarManage.traer_archivo(cantItems);
            
        });
    }

    // Update is called once per frame
    void Update()
    {
        //comprobar si ya hay lugares
        if(_lugarManage.procesoImgCompletado && _lugarManage.procesoLugaresCompletado && _lugarManage.procesoCompletado)  
        {   
            _lugarManage.procesoImgCompletado=false;
            _lugarManage.procesoCompletado = false;

            loading.text = ("DESCARGANDO.."+ System.Environment.NewLine + 
            cantItems+ "/" + _lugarManage.lista_lugares.Count + System.Environment.NewLine + "*porfavor no salirse de la App mientras se efectua la descarga*" );
            agregarDatos(_lugarManage.lugar);
        }

        //comprobar si ya hay usuario admin
        if(_authmanage.procesoadminCompletado){
            _authmanage.procesoadminCompletado = false;
            if (_authmanage.admin.cedula != "")
            {
                PlayerPrefs.SetInt("admin", 1);
                SceneManager.LoadScene("Gestionar_Lugares");
            }
            
        }

        //comprobar si hay usuario user
        if (_authmanage.procesoCompletado)
        {
            _authmanage.procesoCompletado = false;
            nombre_user.text = _authmanage.usuario.nombre;
            puntos_user.text = ("Puntos: "+ _authmanage.usuario.puntos.ToString());
            string jsonuser = JsonUtility.ToJson(_authmanage.usuario);
            PlayerPrefs.SetString("usuario", jsonuser);    
        }

        //cuando ya se descarguen todos los lugares
        if(_lugarManage.traerTodaInfo)
        {
            _lugarManage.traerTodaInfo = false;
            Debug.Log("SE COMPLETO LA DESCARGA");
            decision = false;
            carga.SetActive(false);

            for (int i=0; i<listaimages.Count; i++)
            {
                Lugar b = new Lugar();
                b = listaLugares[i];
                string json = JsonUtility.ToJson(b);
                PlayerPrefs.SetString("listaLugares"+ i, json);
                Debug.Log(json);
                Debug.Log("guardo el: "+ i);
            }
            PlayerPrefs.SetInt("cantidadItems", listaimages.Count);
            //PlayerPrefs.Save();
        }


        //comprobar si presionaron un objeto
        if (seleccionNumero.text != ""){
            int x = int.Parse(seleccionNumero.text) -1;
            _objeto.numItem = int.Parse(seleccionNumero.text);
            _objeto.imagen = listaimages[_objeto.numItem-1];
            _objeto.lugar = listaLugares[_objeto.numItem-1];
            _objeto.u = _authmanage.usuario;
            _objeto.iduser = _authmanage.user.UserId;
            DontDestroyOnLoad(_objeto.gameObject);
            SceneManager.LoadScene("Detalles_Lugar");
        }


    }

    public void ValueChanged(){
         //comprobar si cambiaron de seleccion (todos,fav,visitados)
        if(seleccion.value == 0 )
        {
            Todos.SetActive(true);
            fav.SetActive(false);
            visitados.SetActive(false);

        }else if(seleccion.value == 1)
        {
            Todos.SetActive(false);
            fav.SetActive(true);
            visitados.SetActive(false);
        }else
        {
            Todos.SetActive(false);
            fav.SetActive(false);
            visitados.SetActive(true);
        }
    }

    // boton de actualizar datos
    public void Actualizardatos()
    {
        _lugarManage.ruta();
        _lugarManage.procesoImgCompletado=false;
        _lugarManage.procesoCompletado = false;
        for(int i=1; i<= cantItems; i++){
            System.IO.File.Delete(_lugarManage.path+ "Lugares/Lugar"+ i +".jpg");
        }

        cantItems = 1;
        cantItems2 = 1;
        cantItems3 = 1;
        int numeroHijos1 = Todos.transform.childCount;
        int numeroHijos2 = fav.transform.childCount;
        int numeroHijos3 = visitados.transform.childCount;

        for (int i=0; i<numeroHijos1; i++){
           Destroy(Todos.transform.GetChild(i).gameObject);
        }
        for (int i=0; i<numeroHijos2; i++){
           Destroy(fav.transform.GetChild(i).gameObject);
        }
        for (int i=0; i<numeroHijos3; i++){
           Destroy(visitados.transform.GetChild(i).gameObject);
        }
        
        carga.SetActive(true);
        decision = true;
        listaLugares.Clear();
        listaimages.Clear();
        iniciarDescarga();
    }

    //agrega todos los datos a un gam object y lo instancia
    public void agregarDatos(Lugar _lugar)
    {
        Debug.Log("AGREGO EL DATO "+ cantItems);

        byte[] _file = System.IO.File.ReadAllBytes(_lugarManage.path + "Lugares/Lugar"+ cantItems +".jpg"); 
        
        //anade lugar a la lista
        listaLugares.Add(_lugar);
        listaimages.Add(_file);

        titulo.text =  _lugar.titulo;
        descripcion.text = "Dirección: " + _lugar.direccion ;
        numero.text = cantItems.ToString();

        //mapeando el file a textura
        Texture2D img = new Texture2D(210, 180);
        img.LoadImage(_file);         
        img.Apply();
        image.texture = img;

        
       
        Debug.Log("anadio textura");

        GameObject var = Instantiate(Lugares);
        var.transform.position = new Vector3(0, -140 -(250*(cantItems-1)), 0);    
        var.transform.SetParent(Todos.transform, false);
        var.SetActive(true); 

        
        foreach(string dato in _authmanage.usuario.listaLugaresFav )
        {
            if (dato.Equals(cantItems.ToString()))
            {

                GameObject var2 = Instantiate(var);
                var2.transform.position = new Vector3(0, -140-(250*(cantItems2-1)), 0);    
                var2.transform.SetParent(fav.transform, false);
                cantItems2 ++;
            }
        }

        foreach(string dato in _authmanage.usuario.listaLugaresVisitados )
        {
            if (dato.Equals(cantItems.ToString()))
            {
                GameObject var3 = Instantiate(var);
                var3.transform.position = new Vector3(0, -140-(250*(cantItems3-1)), 0);    
                var3.transform.SetParent(visitados.transform, false);
                cantItems3 ++;
            }
        }       

        cantItems ++;
        if (decision){
        _lugarManage.traer_archivo(cantItems);
        _lugarManage.traerLugar(cantItems);
       }
        
    }

    //cambia de scena enviando un gameobject con la info
    public void CambiarScena(string scena){
        
        _objeto.u = _authmanage.usuario;
        _objeto.iduser = _authmanage.user.UserId;
        _objeto.correo = _authmanage.user.Email;
        DontDestroyOnLoad(_objeto.gameObject);
        SceneManager.LoadScene(scena);
    }


}
