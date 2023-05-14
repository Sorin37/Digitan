using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThiefCircle : MonoBehaviour
{
    [SerializeField] GameObject thiefPrefab;
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

        //Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Thief Circle"));

        var thiefPiece = GameObject.Find("Thief");
        if (thiefPiece != null)
        {
            //dispaly the circle the thief is taken from
            var colliders = Physics.OverlapSphere(
                thiefPiece.transform.position,
                1f,
                (int)Mathf.Pow(2, LayerMask.NameToLayer("Unvisible Thief Circle")));

            foreach (var collider in colliders)
            {
                collider.gameObject.layer = LayerMask.NameToLayer("Thief Circle");
            }

            thiefPiece.transform.position = new Vector3(transform.position.x, 0.75f, transform.position.z);

            //hide the circle the thief is placed on
            colliders = Physics.OverlapSphere(
                thiefPiece.transform.position,
                1f,
                (int)Mathf.Pow(2, LayerMask.NameToLayer("Thief Circle")));

            foreach (var collider in colliders)
            {
                collider.gameObject.layer = LayerMask.NameToLayer("Unvisible Thief Circle");
            }

        }
        else
        {
            var newThief = Instantiate(
                thiefPrefab,
                new Vector3(transform.position.x, 0.75f, transform.position.z),
                Quaternion.Euler(0, 0, 0));

            newThief.name = "Thief";


            //hide the circle the thief is placed on
            var colliders = Physics.OverlapSphere(
                thiefPiece.transform.position,
                1f,
                (int)Mathf.Pow(2, LayerMask.NameToLayer("Thief Circle")));

            foreach (var collider in colliders)
            {
                collider.gameObject.layer = LayerMask.NameToLayer("Unvisible Thief Circle");
            }
        }
    }
}
