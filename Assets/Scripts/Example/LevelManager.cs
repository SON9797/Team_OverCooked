using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overcooked
{
    public class LevelManager : MonoBehaviour, ILevelService
    {
        [Header("¼¼ÆÃ")]
        [SerializeField] private Transform _playerSpawnPoint;

        public Transform GetPlayerSpawnPoint()
        {
            return _playerSpawnPoint;
        }
    }
}
