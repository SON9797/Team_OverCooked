using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodModel : MonoBehaviour
{
    [SerializeField] Transform root;
    public void GotoPosWithRoot(Vector3 pos)
    {
        Vector3 diff = transform.position - root.position;
        transform.position = pos+diff;
        
    }
}
