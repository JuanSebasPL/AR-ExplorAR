using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Usuario : Persona
{
    public int puntos;
    public List<string> listaLugaresFav = new List<string>();
    public List<string> listaLugaresVisitados = new List<string>();

    //constructor, getter y setter

    public Usuario() {
    }

    public Usuario(string username, List<string> listafav, List<string> listavis, string country) {
        //depronto agregar como setter
        this.nombre = username;
        this.pais = country;
        this.puntos = 0;
        this.listaLugaresFav = listafav;
        this.listaLugaresVisitados = listavis;

    }

    public int Puntos {
        get{ return this.puntos; }
        set { this.puntos = value; }
    }

    //agregar toda la lista
    public List<string> ListaLugaresFav {
        get { return this.listaLugaresFav; }
        set { 
            this.listaLugaresFav = value;
            }
    }

    //agregar dato a la lista
    public string AgregarListaLugaresFav {
        set {this.listaLugaresFav.Add (value);
        }

    }

    public List<string> ListaLugaresVisitados {
        get { return this.listaLugaresVisitados; }
        set { this.listaLugaresVisitados = value; }

    }
}
