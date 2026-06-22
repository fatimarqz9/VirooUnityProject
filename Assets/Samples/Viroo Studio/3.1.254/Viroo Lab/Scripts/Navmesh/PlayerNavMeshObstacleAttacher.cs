#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Viroo.Api;
using Viroo.Networking;

namespace VirooLab
{
    public class PlayerNavMeshObstacleAttacher : MonoBehaviour
    {
        private readonly List<NavMeshObstacle> generatedObstacles = new();

        [SerializeField]
        private bool attachObstacleToPlayer = true;

        [SerializeField]
        private bool navMeshShouldCarve = true;

        [SerializeField]
        private float navMeshObstacleRadius = 0.3f;

        protected void Awake()
        {
            VirooApi.Instance.OnInitialized += OnVirooInitialized;
        }

        protected void OnDestroy()
        {
            if (attachObstacleToPlayer)
            {
                if (VirooApi.Instance.IsInitialized)
                {
                    VirooApi.Instance.Players().OnPlayerRegistered -= OnPlayerRegistered;
                    VirooApi.Instance.Players().OnPlayerUnregistered -= OnPlayerUnregistered;
                }

                foreach (NavMeshObstacle obstacle in generatedObstacles)
                {
                    Destroy(obstacle);
                }

                generatedObstacles.Clear();
            }
        }

        private void OnVirooInitialized(object sender, EventArgs e)
        {
            VirooApi.Instance.OnInitialized -= OnVirooInitialized;

            if (attachObstacleToPlayer)
            {
                VirooApi.Instance.Players().OnPlayerRegistered += OnPlayerRegistered;
                VirooApi.Instance.Players().OnPlayerUnregistered += OnPlayerUnregistered;

                foreach (IPlayer player in VirooApi.Instance.Players().GetAll())
                {
                    AttachToPlayer(player);
                }
            }
        }

        private void OnPlayerRegistered(object sender, PlayerRegisteredEventArgs args)
        {
            if (attachObstacleToPlayer)
            {
                AttachToPlayer(args.Player);
            }
        }

        private void OnPlayerUnregistered(object sender, PlayerRegisteredEventArgs args)
        {
            if (args.Player is not Component component)
            {
                return;
            }

            NavMeshObstacle obstacle = component.gameObject.GetComponent<NavMeshObstacle>();
            generatedObstacles.Remove(obstacle);
        }

        private void AttachToPlayer(IPlayer player)
        {
            if (player is not Component component)
            {
                return;
            }

            if (!player.PlayerData!.IsVisible)
            {
                return;
            }

            NavMeshObstacle obstacle = component.gameObject.AddComponent<NavMeshObstacle>();
            obstacle.carveOnlyStationary = false;
            obstacle.carving = navMeshShouldCarve;
            obstacle.radius = navMeshObstacleRadius;
            obstacle.shape = NavMeshObstacleShape.Capsule;
            obstacle.height = 1.7f;
            obstacle.center = 0.5f * obstacle.height * Vector3.up;

            generatedObstacles.Add(obstacle);
        }
    }
}
