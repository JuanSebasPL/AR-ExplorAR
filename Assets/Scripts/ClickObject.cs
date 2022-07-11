using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickObject : MonoBehaviour
{
    public Text numero;
    int anterior;
    
    // Start is called before the first frame update
    void Start()
    {
        anterior = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.touchCount > anterior )
        {
            Touch t = Input.GetTouch(anterior);
            anterior = Input.touchCount;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(t.position);

            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.transform != null){
                    try{
                         numero.text = hit.transform.GetChild(1).GetComponent<TextMesh>().text;
                    }catch{
                        numero.text = "";
                    }
                   
                }
            }

        }

    }
}
