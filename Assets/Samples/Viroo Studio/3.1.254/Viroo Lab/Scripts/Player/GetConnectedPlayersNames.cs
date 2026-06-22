#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Viroo.Api;
using Viroo.Networking;

namespace VirooLab
{
    public class GetConnectedPlayersNames : MonoBehaviour
    {
        [SerializeField]
        private GameObject playerInfoPrefab = default!;

        [SerializeField]
        private Transform playerInfoParent = default!;

        private List<IPlayer>? players;

        private readonly List<GameObject> playersInfo = new();

        protected void Awake()
        {
            VirooApi.Instance.OnInitialized += OnVirooInitialized;
        }

        protected void OnDestroy()
        {
            if (!VirooApi.Instance.IsInitialized)
            {
                return;
            }

            VirooApi.Instance.Players().OnPlayerRegistered -= OnPlayerRegistered;
            VirooApi.Instance.Players().OnPlayerUnregistered -= OnPlayerUnregistered;
        }

        private void OnVirooInitialized(object sender, EventArgs e)
        {
            VirooApi.Instance.OnInitialized -= OnVirooInitialized;

            players = VirooApi.Instance.Players().GetAll().ToList();

            UpdatePlayers();

            VirooApi.Instance.Players().OnPlayerRegistered += OnPlayerRegistered;
            VirooApi.Instance.Players().OnPlayerUnregistered += OnPlayerUnregistered;
        }

        private void OnPlayerRegistered(object sender, PlayerRegisteredEventArgs e)
        {
            if (!players!.Contains(e.Player))
            {
                AddPlayer(e.Player);
            }

            UpdatePlayers();
        }

        private void OnPlayerUnregistered(object sender, PlayerRegisteredEventArgs e)
        {
            if (e.Player.PlayerData!.IsLocalPlayer)
            {
                DeleteUIItems();
                return;
            }

            players!.Remove(e.Player);

            UpdatePlayers();
        }

        private void AddPlayer(IPlayer player)
        {
            if (!players!.Contains(player))
            {
                players.Add(player);
            }
        }

        private void UpdatePlayers()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            DeleteUIItems();

            foreach (IPlayer player in players!)
            {
                if (!player.IsInitialized)
                {
                    return;
                }

                GameObject playerInfo = Instantiate(playerInfoPrefab, playerInfoParent);
                playerInfo.GetComponentInChildren<TextMeshProUGUI>().text = player.PlayerAvatar!.GetAvatarName();
                playersInfo.Add(playerInfo);
            }
        }

        private void DeleteUIItems()
        {
            for (int i = 0; i < playersInfo.Count; i++)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(playersInfo[i]);
                }
                else
                {
                    Destroy(playersInfo[i]);
                }
            }
        }
    }
}
