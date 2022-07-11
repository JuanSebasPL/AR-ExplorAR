using ARLocation;

public class Lugar 
{
    //public List<string> listaImagenes = new List<string>();
    public string titulo;
    public string descripcion;
    public string direccion;
    public Location ubicacion = new Location();

    //constructor, getter y setter
    public Lugar(){

    }

    public Lugar(string _titulo, string _descripcion, string _direccion, double a, double la, double lo)
    {
        this.ubicacion.Altitude = a;
        this.ubicacion.Latitude = la;
        this.ubicacion.Longitude = lo;
        this.ubicacion.AltitudeMode = 0;
        this.titulo = _titulo;
        this.descripcion = _descripcion;
        this.direccion = _direccion;

    }

    public Location Ubicacion
    {
        get { return this.ubicacion; }
        set { this.ubicacion = value; }
    }

   /* public List<string> ListaImagenes
    {
        get { return this.listaImagenes; }
        set { this.listaImagenes = value; }
    }*/

    public string Titulo
    {
        get { return this.titulo; }
        set  { this.titulo = value; }
    }

    public string Descripcion
    {
        get { return this.descripcion;}
        set { this.descripcion = value;}
    }

}