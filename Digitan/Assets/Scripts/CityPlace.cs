using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CityPlace : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMouseDown()
    {
        //necessary piece of code in order to prevent clicking through UI elements
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        AddResourcesToDict();
        DestroyNearbySetllements();

        Destroy(this.gameObject);

        Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("City Place"));
        Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("Settlement"));
    }

    private void DestroyNearbySetllements()
    {
        var colliders = Physics.OverlapSphere(
            transform.position,
            1,
           (int)Mathf.Pow(2, LayerMask.NameToLayer("Settlement"))
        );

        foreach (var collider in colliders)
        {
            Destroy(collider.gameObject);
        }
    }

    private void AddResourcesToDict()
    {
        print("Inca nu e implementat tehe :P");
    }
}
