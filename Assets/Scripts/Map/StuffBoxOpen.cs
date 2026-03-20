using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuffBoxOpen : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private bool _isPlayerNearby = false;

    void Start()
    {
        _animator = GetComponent<Animator>();
       
    }
    void Update()
    {
        if (_isPlayerNearby && Input.GetKeyDown(KeyCode.Space))
        {
            _animator.SetBool("Open", true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerNearby = false;
        }
    }
}
