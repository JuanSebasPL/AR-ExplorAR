using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarScena : MonoBehaviour
{


    public void CambiarPantalla (string scn2) {

        SceneManager.LoadScene(scn2);
    }


}
