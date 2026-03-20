using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemControlTest : MonoBehaviour
{
    [SerializeField] LayerMask ingredientLayer;
    
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            print("ÀÔ·Â");
            Ingredient item = ItemCk();
            if (item!=null)
            {
                print("item exist");
                if (item.CanStatusAdd(CookBehaivior.chop))
                {
                    
                    item.AddStatus(CookBehaivior.chop);
                }
            }
        }
    }

    Ingredient ItemCk()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward,out hit, 2f))
        {
            print("ÀÎ½ÄµÊ");
            Ingredient ing=hit.transform.root.gameObject.GetComponent<Ingredient>();
            return ing;
        }
        return null;

    }
}
