using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persona 
{
    public string nombre;
    public string pais;

    //constructor, getter y setter

    public string Nombre
    {
        get { return this.nombre; }
        set { this.nombre = value; }
    }

    public string Pais
    {
        get { return this.pais; }
        set { this.pais = value; }
    }


}
