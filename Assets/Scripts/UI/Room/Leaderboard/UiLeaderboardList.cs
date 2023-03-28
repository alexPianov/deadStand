using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Playstel;
using UnityEngine;
using Zenject;

public class UiLeaderboardList : UiFactory
{
    public List<UiLeaderboardSlot> _playerSlots = new();

    [Inject] private RoomPlayers _roomPlayers;

    public void Start()
    {
        UpdateStatistics();
    }

    private float time;
    private int updateInterval = 2;
    private void Update()
    {
        time += Time.deltaTime;
        
        if (time > updateInterval)
        {
            time = 0;
            UpdateStatistics();
        }
    }

    public async void UpdateStatistics()
    {
        ClearFactoryContainer();
        _playerSlots.Clear();

        var players = PhotonNetwork.PlayerList.ToList();
        
        Debug.Log("players: " + players.Count);

        if (players.Count == 0)
        {
            Debug.Log("Players count in room is null");
            return;
        }

        foreach(var player in players)
        {
            GameObject playerSlotInstance = null;

            if (player.IsLocal)
            {
                playerSlotInstance = await CreateSlot(SlotName.UserSlotRoomMine);
            }
            else
            {
                playerSlotInstance = await CreateSlot(SlotName.UserSlotRoom);
            }

            if(playerSlotInstance.TryGetComponent(out UiLeaderboardSlot playerSlot))
            {
                playerSlot.SetPlayerInfo(player);
                _playerSlots.Add(playerSlot);
            }
        }

        var bots = _roomPlayers.GetBots();

        foreach (var bot in bots)
        {
            var playerSlotInstance = await CreateSlot(SlotName.UserSlotRoom);

            if (playerSlotInstance.TryGetComponent(out UiLeaderboardSlot playerSlot))
            {
                playerSlot.SetPlayerInfo(bot);
                _playerSlots.Add(playerSlot);
            }
        }

        SortByOrder();
    }

    private void SortByOrder()
    {
        var orderedList = _playerSlots.OrderByDescending(slot => slot.GetAverageScore()).ToList();

        for (int i = 0; i < orderedList.Count; i++)
        {
            var slot = orderedList[i];
            
            if (slot)
            {
                Debug.Log(i + 1 + ". " + slot.playerName.text + " | " + slot.GetAverageScore());
                slot.SetPlayerPlace(i + 1);
                slot.name = i + 1 + ". " + slot.playerName.text;
                slot.transform.SetSiblingIndex(i);
            } 
        }
    }
}
