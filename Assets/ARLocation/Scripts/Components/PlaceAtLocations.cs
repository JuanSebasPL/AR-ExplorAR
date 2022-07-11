using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
// ReSharper disable CollectionNeverQueried.Local
// ReSharper disable MemberCanBePrivate.Global

namespace ARLocation
{

    /// <summary>
    /// This class instantiates a prefab at the given GPS locations. Must
    /// be in the `ARLocationRoot` GameObject with a `ARLocatedObjectsManager`
    /// Component.
    /// </summary>
    [AddComponentMenu("AR+GPS/Place At Locations")]
    [HelpURL("https://http://docs.unity-ar-gps-location.com/guide/#placeatlocations")]
    public class PlaceAtLocations : MonoBehaviour
    {
        [Serializable]
        public class Entry
        {
            public LocationData ObjectLocation;
            public OverrideAltitudeData OverrideAltitude = new OverrideAltitudeData();
        }

        [Tooltip("The locations where the objects will be instantiated.")]
        public List<PlaceAtLocation.LocationSettingsData> Locations;

        public PlaceAtLocation.PlaceAtOptions PlacementOptions;

        /// <summary>
        /// The game object that will be instantiated.
        /// </summary>
        [FormerlySerializedAs("prefab")] [Tooltip("The game object that will be instantiated.")]
        public GameObject Prefab;
        public TextMesh textoPrefab;

        [Space(4.0f)]

        [Header("Debug")]
        [Tooltip("When debug mode is enabled, this component will print relevant messages to the console. Filter by 'PlateAtLocations' in the log output to see the messages.")]
        public bool DebugMode;

        [Space(4.0f)]

        private readonly List<Location> locations = new List<Location>();
        private readonly List<GameObject> instances = new List<GameObject>();
        public List<Lugar> listaLugares = new List<Lugar>();

        private void Start()
        {
            //trae los items del playerprefs
            for (int i=0; i< PlayerPrefs.GetInt("cantidadItems"); i++)
            {
                string json = PlayerPrefs.GetString("listaLugares"+i);
                Lugar lugarsito = new Lugar();
                lugarsito = JsonUtility.FromJson<Lugar>(json);
                var newLoc = lugarsito.ubicacion;
                textoPrefab.text = lugarsito.titulo;
                AddLocation(newLoc);
                listaLugares.Add(lugarsito);
            }
            
        }

        public void AddLocation(Location location)
        {
            Prefab.SetActive(true);
            var instance = PlaceAtLocation.CreatePlacedInstance(Prefab, location, PlacementOptions, DebugMode);
            Prefab.SetActive(false);
            instance.name = $"{gameObject.name} - {locations.Count}";

            locations.Add(location);
            instances.Add(instance);
        }

        public void atras()
        {
            Destroy(GameObject.Find("TextureBufferCamera"));
            SceneManager.LoadScene("Lista_Lugares");
        }
    }
}
