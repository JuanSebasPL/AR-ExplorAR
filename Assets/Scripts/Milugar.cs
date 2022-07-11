using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Milugar : MonoBehaviour
{
    public Text guarda;
    public Text miText;


    public void OnPressed(){
        guarda.text = miText.text;
    }

}