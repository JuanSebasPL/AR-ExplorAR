using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backup 
{
    public Lugar miLugar;   
    public Texture2D miImagen;

    public Backup(){

    }

    public Backup(Lugar l, Texture2D b ){
        this.miLugar = l;
        this.miImagen = b;
    }

    public Lugar Milugar
    {
        get { return this.miLugar; }
        set { this.miLugar = value; }
    }

}
