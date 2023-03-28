using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using static Playstel.UnitAiTargetGlobal;
using static Playstel.UnitFraction;
using static Playstel.UnitPower;

namespace Playstel
{
    public class RoomPlayers : MonoBehaviour
    {
        private Dictionary<Fraction, List<Unit>> _players = new();
        private List<Unit> bots = new();
        public int MaxPlayersInTeam = 6;
        public int PlayersCount;

        [Inject] private LocationInstaller _locationInstaller;
        private void Awake()
        {
            _locationInstaller.BindFromInstance(this);
        }

        public void SetPlayer(Fraction fraction, Unit unit)
        {
            PlayersCount++;
            
            unit.gameObject.layer = LayerMask.NameToLayer(fraction.ToString());
            
            _players.TryGetValue(fraction, out var playerList);
            if (playerList == null) playerList = new(); 
            
            playerList.Add(unit);
            _players.Remove(fraction);
            _players.Add(fraction, playerList);

            if (unit.IsNPC) bots.Add(unit); 
        }

        public List<Unit> GetBots()
        {
            return bots;
        }

        public Unit GetGroupLeader(Fraction fraction)
        {
            _players.TryGetValue(fraction, out var bots);

            bots.OrderBy(unit => unit.Power.GetPower());

            return bots[0];
        }
        
        public int GetOrientationUnitsCount(Fraction fraction, AiOrientation orientation)
        {
            _players.TryGetValue(fraction, out var bots);

            var npc = bots.FindAll(unit => unit.IsNPC);
            var npcGroup = npc.FindAll(unit => unit.TargetGlobal.orientation == orientation);

            return npcGroup.Count;
        }
        
        public Unit GetFreeUnit(Fraction fraction, Unit unit)
        {
            _players.TryGetValue(fraction, out var bots);
            
            var freeUnit = bots.Find(u => !u.Power.GetOppositeUnit());
            
            if (freeUnit)
            {
                freeUnit.Power.SetOppositeUnit(unit);
                return freeUnit;
            }
            
            return unit.Power.GetOppositeUnit();
        }
        
        public AiOrientation GetBalancedOrientation(Fraction fraction)
        {
            var right = GetOrientationUnitsCount(fraction, AiOrientation.Right);
            var left = GetOrientationUnitsCount(fraction, AiOrientation.Left);

            if (right > left) return AiOrientation.Left;
            
            return AiOrientation.Right;
        }
        
        public Unit GetOppositeUnit(Fraction currentFraction, Unit unit)
        {
            if (currentFraction == Fraction.Red)
            {
                return GetFreeUnit(Fraction.Blue, unit);
            }
            
            if (currentFraction == Fraction.Blue)
            {
                return GetFreeUnit(Fraction.Red, unit);
            }

            return null;
        }
    }
}