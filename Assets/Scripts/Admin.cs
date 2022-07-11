using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Admin : Persona
{
    public string cedula;
    public int celular;

       //constructor, getter y setter
    public Admin(string username, string country, string id, int phone) {
        this.nombre = username;
        this.pais = country;
        this.cedula = id;
        this.celular = phone;

    }

    public Admin(){
        
    }
 
    public string Cedula
    {
        get { return this.cedula; }
        set { this.cedula = value; }
    }

    public int Celular
    {
        get { return this.celular; }
        set { this.celular = value; }
    }
}
