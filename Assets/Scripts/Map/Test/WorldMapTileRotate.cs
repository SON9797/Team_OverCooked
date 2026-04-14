using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapTileRotate : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _duration = 1.0f; // 뒤집히는 데 걸리는 시간
    [SerializeField] private float _jumpHeight = 1.5f;
    [SerializeField] private Material _nextMaterial; // 바뀔 머테리얼
    [SerializeField] private List<WorldMapBuilding> _myBuildings = new List<WorldMapBuilding>();
    [SerializeField] private int _parentStageIndex;
    [SerializeField] private int _parentMainChapter = 1;

    public bool _isFlipping = false;
    private bool _isActivated = false;
    private MeshRenderer _meshRenderer;
    private Vector3 _initialPosition;
    void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _initialPosition = transform.position;
    }
    private void Start()
    {
        CheckIfAlreadyFlipped();
    }
    public void Flip()
    {
        Debug.Log($"[TileRotate] Flip 호출됨: {gameObject.name}, isFlipping: {_isFlipping}, isActivated: {_isActivated}");

        if (_isFlipping || _isActivated)
        {
            Debug.Log($"[TileRotate] Flip 스킵됨 - isFlipping: {_isFlipping}, isActivated: {_isActivated}");
            return;
        }
        StartCoroutine(FlipRoutine());
    }

    private IEnumerator FlipRoutine()
    {
        _isFlipping = true;
        _isActivated = true;

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 0, 180f);

        float elapsed = 0f;
        bool materialChanged = false;

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _duration;


            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

            float yOffset = Mathf.Sin(t * Mathf.PI) * _jumpHeight;
            transform.position = _initialPosition + new Vector3(0, yOffset, 0);

            if (!materialChanged && t >= 0.5f)
            {
                _meshRenderer.material = _nextMaterial;
                materialChanged = true;

                if (_myBuildings != null && _myBuildings.Count > 0)
                {
                    foreach (var building in _myBuildings)
                    {
                        if (building != null)
                        {
                            building.Appear();
                        }
                    }
                }
            }
            yield return null;
        }

        transform.rotation = endRotation;
        transform.position = _initialPosition;
        _isFlipping = false;
    }

    private void CheckIfAlreadyFlipped()
    {
        string stageKey = $"{_parentMainChapter}-{_parentStageIndex}";
        if (SaveLoad.instance == null) return;

        SaveData data = SaveLoad.instance.currentData;

        if (data.bestScores.ContainsKey(stageKey))
        {
            // 클리어한 스테이지 - 완전히 뒤집힌 상태, _isActivated = true
            SetTargetStateImmediate();
        }
        else if (data.unlockedStages.Contains(stageKey))
        {
            // 해금만 된 스테이지 - 뒤집혀야 하지만 _isActivated = false (다시 Flip 가능)
            SetUnlockedStateImmediate();
        }
    }

    private void SetTargetStateImmediate()
    {
        _isActivated = true;
        _isFlipping = false;

        transform.rotation = Quaternion.Euler(0, 0, 180f);

        if (_nextMaterial != null)
        {
            _meshRenderer.material = _nextMaterial;
        }

        if (_myBuildings != null)
        {
            foreach (var building in _myBuildings)
            {
                if (building != null)
                {
                    building.AppearImmediate();
                }
            }
        }
    }
    // 해금만 된 스테이지용
    private void SetUnlockedStateImmediate()
    {
        _isActivated = false; // ★ Flip 허용
        _isFlipping = false;

        transform.rotation = Quaternion.Euler(0, 0, 180f);
        if (_nextMaterial != null)
        {
            _meshRenderer.material = _nextMaterial;
        }
        if (_myBuildings != null)
        {
            foreach (var building in _myBuildings)
            {
                if (building != null)
                    building.AppearImmediate();
            }
        }
    }
}