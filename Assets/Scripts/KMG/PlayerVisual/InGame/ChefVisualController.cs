using System;
using System.Collections.Generic;
using UnityEngine;

namespace Overcooked
{
    public enum ChefHatType
    {
        None,
        BaseballCap,
        Cap,
        Fancy,
        Tall
    }

    [Serializable]
    public class ChefVisualEntry
    {
        public ChefType chefType;
        public GameObject chefObject;
        public ChefHatType hatType;
    }

    public class ChefVisualController : MonoBehaviour
    {
        [Header("공용 바디")]
        [SerializeField] private GameObject _body;

        [Header("로봇 전용 바디")]
        [SerializeField] private GameObject _robotBody;

        [Header("쉐프 오브젝트 목록")]
        [SerializeField] private List<ChefVisualEntry> _chefVisuals = new List<ChefVisualEntry>();

        [Header("공용 모자 4종")]
        [SerializeField] private GameObject _hatBaseballCap;
        [SerializeField] private GameObject _hatCap;
        [SerializeField] private GameObject _hatFancy;
        [SerializeField] private GameObject _hatTall;

        private readonly Dictionary<ChefType, ChefVisualEntry> _chefMap = new Dictionary<ChefType, ChefVisualEntry>();

        private bool _isBuilt;

        public GameObject CurrentChefObject { get; private set; }

        private void Awake()
        {
            BuildMap();
            ResetVisualState();
        }

        public void ApplyChef(ChefType chefType)
        {
            EnsureBuilt();
            ResetVisualState();

            bool isRobot = chefType == ChefType.Robot;

            if (_body != null)
            {
                _body.SetActive(!isRobot);
            }

            if (_robotBody != null)
            {
                _robotBody.SetActive(isRobot);
            }

            if (_chefMap.TryGetValue(chefType, out ChefVisualEntry entry))
            {
                if (entry.chefObject != null)
                {
                    entry.chefObject.SetActive(true);
                    CurrentChefObject = entry.chefObject;
                }

                ApplyHat(entry.hatType);
            }
            else
            {
                Debug.LogWarning($"{name}: {chefType}에 해당하는 쉐프 오브젝트가 등록되지 않았습니다.");
            }
        }

        public void ResetVisualState()
        {
            DisableAllChefObjects();
            DisableAllHats();

            if (_body != null)
            {
                _body.SetActive(false);
            }

            if (_robotBody != null)
            {
                _robotBody.SetActive(false);
            }

            CurrentChefObject = null;
        }

        private void EnsureBuilt()
        {
            if (_isBuilt)
                return;

            BuildMap();
        }

        private void BuildMap()
        {
            _chefMap.Clear();

            for (int i = 0; i < _chefVisuals.Count; i++)
            {
                ChefVisualEntry entry = _chefVisuals[i];

                if (entry == null)
                    continue;

                if (entry.chefObject == null)
                    continue;

                if (_chefMap.ContainsKey(entry.chefType))
                {
                    Debug.LogWarning($"{name}: {entry.chefType}가 중복 등록되어 있습니다.");
                    continue;
                }

                _chefMap.Add(entry.chefType, entry);
            }

            _isBuilt = true;
        }

        private void DisableAllChefObjects()
        {
            for (int i = 0; i < _chefVisuals.Count; i++)
            {
                ChefVisualEntry entry = _chefVisuals[i];

                if (entry == null)
                    continue;

                if (entry.chefObject == null)
                    continue;

                entry.chefObject.SetActive(false);
            }
        }

        private void DisableAllHats()
        {
            if (_hatBaseballCap != null) _hatBaseballCap.SetActive(false);
            if (_hatCap != null) _hatCap.SetActive(false);
            if (_hatFancy != null) _hatFancy.SetActive(false);
            if (_hatTall != null) _hatTall.SetActive(false);
        }

        private void ApplyHat(ChefHatType hatType)
        {
            switch (hatType)
            {
                case ChefHatType.BaseballCap:
                    if (_hatBaseballCap != null) _hatBaseballCap.SetActive(true);
                    break;

                case ChefHatType.Cap:
                    if (_hatCap != null) _hatCap.SetActive(true);
                    break;

                case ChefHatType.Fancy:
                    if (_hatFancy != null) _hatFancy.SetActive(true);
                    break;

                case ChefHatType.Tall:
                    if (_hatTall != null) _hatTall.SetActive(true);
                    break;
            }
        }
    }
}