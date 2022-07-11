using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthManager 
{
    public Firebase.Auth.FirebaseAuth auth;
    public Firebase.Auth.FirebaseUser user;


    public string displayName;
    public string displayEmailAddress;
    public string inputFieldEmail;
    public string inputFieldPasword;
    public string inputFieldName;
    public string errorMessage;
    public bool resultado_proceso=false;

    public Usuario usuario = new Usuario();
    public Admin admin = new Admin();
    public DatabaseReference reference;
    public bool procesoCompletado = false;
    public bool procesoadminCompletado = false;
   


    public void traerUser()
    {    
        FirebaseDatabase.DefaultInstance.GetReference("Users").Child(user.UserId).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                Debug.Log("error al obtener referencia");
            // Handle the error...
            }
            else if (task.IsCompleted) {
            // Do something with snapshot... 
            DataSnapshot snapshot = task.Result;
            string json = snapshot.GetRawJsonValue();
            usuario = JsonUtility.FromJson<Usuario>(json);
            procesoCompletado = true;
            }
        });
    }

    public void traerAdmin()
    {    
        FirebaseDatabase.DefaultInstance.GetReference("Admins").Child(user.UserId).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                Debug.Log("error al obtener referencia");
            // Handle the error...
            }
            else if (task.IsCompleted) {
            // Do something with snapshot... 
            DataSnapshot snapshot = task.Result;
            string jsona = snapshot.GetRawJsonValue();
            admin = JsonUtility.FromJson<Admin>(jsona);
            Debug.Log("trajo "+ admin.nombre + " + " + admin.cedula);
            procesoadminCompletado = true;
            }
        });
    }

    //iniciar al firebase auth
    public void InitializeFirebase() {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    public void AuthStateChanged(object sender, System.EventArgs eventArgs) {
        if (auth.CurrentUser != user) 
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null) {
            Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn) {
            Debug.Log("Signed in " + user.UserId);
                traerUser();
                traerAdmin();
            }
        }
    }

    //Registrar un usuario
    public void CreateUser()
    {
        string email = inputFieldEmail;
        string password = inputFieldPasword;
        errorMessage = "";
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {

            if (task.IsCanceled) {
                errorMessage = "Intentelo de nuevo";
                Debug.LogError(errorMessage);
                return;
            }
            if (task.IsFaulted) {
                errorMessage = "Verifique los datos ingresados";
                Debug.LogError(errorMessage);
                return;
            }

            resultado_proceso = true;
            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

        });

    }



    //iniciar sesion
    public void LoginSession()
    {
        string email = inputFieldEmail;
        string password = inputFieldPasword;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
        if (task.IsCanceled) {
            errorMessage = "SignInWithEmailAndPasswordAsync was canceled.";
            Debug.LogError(errorMessage);
            return;
        }
        if (task.IsFaulted) {
            errorMessage = "SignInWithEmailAndPasswordAsync is faulted " + task.Exception;
            Debug.LogError(errorMessage);
            return;
        }

        
        Firebase.Auth.FirebaseUser newUser = task.Result;
        Debug.LogFormat("User signed in successfully: {0} ({1})",
            newUser.DisplayName, newUser.UserId);
        });
    }

    //Actualizar correo
    public void ActualizarUsuario(string newCorreo ,string newPassword )
    {
        if (user != null) 
        {
            user.UpdateEmailAsync(newCorreo).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("UpdateEmailAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("UpdateEmailAsync encountered an error: " + task.Exception);
                    return;
                }
                Debug.Log("User email updated successfully.");
                user.UpdatePasswordAsync(newPassword).ContinueWith(taske => {
                    if (taske.IsCanceled) {
                    Debug.LogError("UpdatePasswordAsync was canceled.");
                    return;
                    }
                    if (taske.IsFaulted) {
                    Debug.LogError("UpdatePasswordAsync encountered an error: " + task.Exception);
                    return;
                    }
                    Debug.Log("Password updated successfully.");
                    resultado_proceso = true;
                });
            });
        }



    }
}
