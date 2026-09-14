using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MultipleImagesTrackingManager : MonoBehaviour
{
    // 1. VARIABLES GLOBALES
    // Esta lista aparece en el Inspector de Unity para que arrastremos los modelos 3D que queremos que aparezcan.
    [SerializeField] private List<GameObject> prefabsToSpawn = new List<GameObject>();

    // El "cerebro" que maneja el escaneo de imágenes de AR Foundation.
    private ARTrackedImageManager _trackedImageManager;

    // Un diccionario es como una agenda. Guardamos el Nombre de la imagen (string) y su Objeto 3D asociado (GameObject).
    private Dictionary<string, GameObject> _arObjects;

    // 2. INICIALIZACIÓN
    private void Start()
    {
        // Buscamos el componente ARTrackedImageManager que tiene que estar en este mismo objeto (el XR Origin).
        _trackedImageManager = GetComponent<ARTrackedImageManager>();

        // Creamos la "agenda" vacía.
        _arObjects = new Dictionary<string, GameObject>();

        // Le decimos al manager: "Avisame (ejecutá OnImagesTrackedChanged) cada vez que la cámara vea, mueva o pierda una imagen".
        _trackedImageManager.trackablesChanged.AddListener(OnImagesTrackedChanged);

        // Llamamos a nuestra función que prepara los objetos invisibles al arrancar.
        SetupSceneElements();
    }

    // 3. SEGURIDAD
    // Si este script se apaga o destruye, le decimos al manager que nos deje de avisar para no generar errores de memoria.
    private void OnDestroy()
    {
        _trackedImageManager.trackablesChanged.RemoveListener(OnImagesTrackedChanged);
    }

    // 4. PREPARACIÓN DE LOS OBJETOS
    private void SetupSceneElements()
    {
        // Recorremos la lista de prefabs que pusimos en el Inspector uno por uno.
        foreach (var prefab in prefabsToSpawn)
        {
            // Creamos (Instanciamos) el objeto 3D en el centro del mundo (Vector3.zero) temporalmente.
            var arObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);

            // Le ponemos exactamente el mismo nombre que tiene el Prefab original (esto es CLAVE para que coincida con la imagen).
            arObject.name = prefab.name;

            // Lo apagamos/hacemos invisible. Solo se va a prender cuando la cámara vea la imagen correcta.
            arObject.gameObject.SetActive(false);

            // Lo anotamos en nuestra agenda: Clave (Nombre del objeto) -> Valor (El objeto 3D real).
            _arObjects.Add(arObject.name, arObject);
        }
    }

    // 5. EL ESCUCHADOR DE EVENTOS
    // Esta función se dispara sola cada vez que AR Foundation nota un cambio en la cámara.
    private void OnImagesTrackedChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        // Si entró una imagen NUEVA a la cámara...
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateTrackedImages(trackedImage);
        }

        // Si una imagen que ya estábamos viendo se MOVIÓ...
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateTrackedImages(trackedImage);
        }

        // Si una imagen SALIÓ del campo de visión de la cámara...
        foreach (var trackedImage in eventArgs.removed)
        {
            UpdateTrackedImages(trackedImage.Value);
        }
    }

    // 6. LA LÓGICA VISUAL
    // Acá decidimos qué hacer físicamente con el objeto 3D dependiendo de lo que le pase a la imagen.
    private void UpdateTrackedImages(ARTrackedImage trackedImage)
    {
        // Si por algún error la imagen viene vacía, no hacemos nada y salimos.
        if (trackedImage == null) return;

        // Si la cámara la está perdiendo de vista (Limited) o la perdió del todo (None)...
        if (trackedImage.trackingState is TrackingState.Limited or TrackingState.None)
        {
            // Buscamos el objeto en nuestra agenda usando el nombre de la imagen y lo APAGAMOS.
            _arObjects[trackedImage.referenceImage.name].gameObject.SetActive(false);
            return;
        }

        // Si la cámara la está viendo perfectamente bien (Tracking)...
        if (prefabsToSpawn != null)
        {
            // Buscamos el objeto en la agenda y lo PRENDEMOS.
            _arObjects[trackedImage.referenceImage.name].gameObject.SetActive(true);

            // Le copiamos la posición exacta de la imagen real en el piso/mesa.
            _arObjects[trackedImage.referenceImage.name].transform.position = trackedImage.transform.position;

            // Le copiamos la rotación exacta de la imagen.
            _arObjects[trackedImage.referenceImage.name].transform.rotation = trackedImage.transform.rotation;
        }
    }
}
