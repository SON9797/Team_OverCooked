using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class ItemControlTest : MonoBehaviour
{
    [SerializeField] LayerMask ingredientLayer;

    Ingredient currentHandItem;

    //기즈모 체크용
    Vector3 gizmoOrigin;
    Vector3 gizmoHalfExtents;
    float gizmoDistance;
    Quaternion gizmoRotation;
    bool gizmoHasHit;
    //기즈모 체크용

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            //Ingredient item = IngredientCk();
            Ingredient item = Detect(new Vector3(0.5f,0.5f,0.5f), 0.5f);
            if (item!=null)
            {
                if (item.CanStatusAdd(CookBehaivior.chop))
                {
                    item.AddStatus(CookBehaivior.chop);
                }

            }
            
            
        }

        //임시로 재료를 들었다고 치는 코드
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            currentHandItem= Detect(new Vector3(0.5f, 0.5f, 0.5f), 0.5f);
            print(currentHandItem);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Dish dish = DishCk(new Vector3(0.5f, 0.5f, 0.5f), 1f);
            if (dish != null)
            {
                dish.AddIngredient(currentHandItem);
            }
        }
    }

    public Ingredient Detect(Vector3 halfExtents, float distance)
    {
        RaycastHit hit;

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        Quaternion rotation = transform.rotation;

        bool hasHit = Physics.BoxCast(
            transform.position,
            halfExtents,
            transform.forward,
            out hit,
            transform.rotation,
            distance
        );

        // Gizmo용 데이터 저장
        gizmoOrigin = origin;
        gizmoHalfExtents = halfExtents;
        gizmoDistance = distance;
        gizmoRotation = rotation;
        gizmoHasHit = hasHit;

        if (hasHit)
        {
            Debug.DrawLine(origin, hit.point, Color.green, 0.1f);

            Ingredient ing = hit.transform.root.GetComponent<Ingredient>();
            if (ing != null)
                return ing;
        }

        return null;
    }

    private Dish DishCk(Vector3 halfExtents, float distance)
    {
        RaycastHit hit;

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        Quaternion rotation = transform.rotation;

        bool hasHit = Physics.BoxCast(
            transform.position,
            halfExtents,
            transform.forward,
            out hit,
            transform.rotation,
            distance
        );

        // Gizmo용 데이터 저장
        gizmoOrigin = origin;
        gizmoHalfExtents = halfExtents;
        gizmoDistance = distance;
        gizmoRotation = rotation;
        gizmoHasHit = hasHit;
        if (hasHit) { 
            Dish dish = hit.transform.root.gameObject.GetComponent<Dish>();
            if (dish != null)
            {
                return dish;
            }
        }
        return null;
    }

    void OnDrawGizmos()
    {
        // 아직 Detect 안 돌았으면 안 그림
        if (gizmoDistance == 0) return;

        Vector3 origin = gizmoOrigin;
        Quaternion rot = gizmoRotation;

        Gizmos.color = gizmoHasHit ? Color.green : Color.red;

        // 시작 박스
        Gizmos.matrix = Matrix4x4.TRS(origin, rot, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, gizmoHalfExtents * 2);

        // 끝 박스
        Vector3 end = origin + (rot * Vector3.forward) * gizmoDistance;
        Gizmos.matrix = Matrix4x4.TRS(end, rot, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, gizmoHalfExtents * 2);

        // 방향선
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.DrawLine(origin, end);
    }
}
