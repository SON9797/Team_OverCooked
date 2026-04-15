using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overcooked
{
    public class ChefHandVisualController : MonoBehaviour
    {
        private readonly List<GameObject> _openHands = new List<GameObject>();
        private readonly List<GameObject> _gripHands = new List<GameObject>();

        private GameObject _currentChefObject;
        private PlayerAnimationController _animationController;
        private ChefVisualController _chefVisualController;

        private void Awake()
        {
            _animationController = GetComponent<PlayerAnimationController>();
            _chefVisualController = GetComponent<ChefVisualController>();
        }

        private void LateUpdate()
        {
            if (_animationController == null)
            {
                _animationController = GetComponent<PlayerAnimationController>();
            }

            if (_chefVisualController == null)
            {
                _chefVisualController = GetComponent<ChefVisualController>();
            }

            RefreshCurrentChefHandRefs();

            if (_currentChefObject == null)
                return;

            bool isChopping = _animationController != null && _animationController.IsChoppingNow();

            if (isChopping)
            {
                ApplyGripOnly();
            }
            else
            {
                ApplyOpenOnly();
            }
        }

        private void RefreshCurrentChefHandRefs()
        {
            GameObject activeChefObject = null;

            if (_chefVisualController != null)
            {
                activeChefObject = _chefVisualController.CurrentChefObject;
            }

            if (activeChefObject == _currentChefObject)
                return;

            _currentChefObject = activeChefObject;

            _openHands.Clear();
            _gripHands.Clear();

            if (_currentChefObject == null)
                return;

            CacheHandsFromChef(_currentChefObject.transform);
            ApplyOpenOnly();
        }

        private void CacheHandsFromChef(Transform chefRoot)
        {
            Transform[] children = chefRoot.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in children)
            {
                string lowerName = child.name.ToLower();

                if (lowerName.Contains("hand_open"))
                {
                    _openHands.Add(child.gameObject);
                }
                else if (lowerName.Contains("hand_grip"))
                {
                    _gripHands.Add(child.gameObject);
                }
            }
        }

        private void ApplyOpenOnly()
        {
            for (int i = 0; i < _openHands.Count; i++)
            {
                SetActiveSafe(_openHands[i], true);
            }

            for (int i = 0; i < _gripHands.Count; i++)
            {
                SetActiveSafe(_gripHands[i], false);
            }
        }

        private void ApplyGripOnly()
        {
            for (int i = 0; i < _openHands.Count; i++)
            {
                SetActiveSafe(_openHands[i], false);
            }

            for (int i = 0; i < _gripHands.Count; i++)
            {
                SetActiveSafe(_gripHands[i], true);
            }
        }

        private void SetActiveSafe(GameObject target, bool active)
        {
            if (target == null)
                return;

            if (target.activeSelf != active)
            {
                target.SetActive(active);
            }
        }
    }
}
