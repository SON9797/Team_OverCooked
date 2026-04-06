using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overcooked
{
    // 아이템던지기 - 던져진 아이템의 충돌을 PlayerItemController로 전달
    public class ThrownItemCollisionRelay : MonoBehaviour
    {
        private PlayerItemController _owner;

        public void Initialize(PlayerItemController owner)
        {
            _owner = owner;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_owner == null)
            {
                return;
            }

            _owner.NotifyThrownObjectCollision(gameObject, collision);
        }
    }
}
