using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuffBoxOpen : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] Transform _rayPoint;
    [SerializeField] private float _interactionDistance = 3.0f; // 상호작용 가능 거리

    void Start()
    {
        // 만약 에디터에서 할당하지 않았다면 자동으로 가져옴
        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckForBox();
        }
    }

    private void CheckForBox()
    {
        // 메인 카메라의 위치와 정면 방향을 기준으로 레이 생성
        RaycastHit hit;
        

        // 레이를 쏴서 물체에 맞았는지 확인
        if (Physics.Raycast(_rayPoint.position, _rayPoint.forward, out hit, _interactionDistance))
        {
            // 맞은 물체가 현재 이 스크립트가 붙은 오브젝트인지 확인
            if (hit.transform == this.transform)
            {
                _animator.SetBool("Open", true);
                Debug.Log("상자를 열었습니다!");
            }
        }
    }
}
