using Cysharp.Threading.Tasks;
using Photon.Pun;
using Playstel.UI;
using UnityEngine;

namespace Playstel
{
    public class UnitAiBoot : MonoBehaviourPun
    {
        [Header("Refs")]
        public GuiUnitName GuiUnitName;

        private Unit _unit;
        private void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        public void SetUnitNickName(string nickName)
        {
            if(GuiUnitName) GuiUnitName.HandleNicknameUpdate(nickName);
            gameObject.name = nickName;
            _unit.SetUnitName(nickName);
        }
        
        public async UniTask Install()
        {
            await UniTask.WaitUntil(() => _unit);
            await UniTask.WaitUntil(() => _unit.Builder);
            await UniTask.WaitUntil(() => _unit.Buffer);

            SetCustomMaterial();
            
            await UniTask.Delay(100);
            
            SetItems();
            
            await UniTask.Delay(100);
            
            //if(photonView.IsMine) SetResources();
            
            SetMaxValues();
            
            await UniTask.Delay(100);
            
            if(photonView.IsMine) SetExperience();
            
            SetHealth();
            SetMove();
            //SetAmmo();
            
            await UniTask.Delay(100);

            _unit.TargetGlobal.UpdateOrientation();
        }

        private void SetAmmo()
        {
            var startHolders = PhotonHandler.GetPlayerHash<int>
                (PhotonHandler.Hash.Holders, photonView.Owner);
            
            var startBullets = PhotonHandler.GetPlayerHash<int>
                (PhotonHandler.Hash.Bullets, photonView.Owner);

            _unit.Buffer.ChangeHolders(startHolders, true);
            _unit.Buffer.ChangeBullets(startBullets);
        }

        public void SetCustomMaterial()
        {
            var skinMaterial = PhotonHandler.GetPlayerHash<string>
                (PhotonHandler.Hash.Skin, photonView.Owner);
            
            photonView.RPC(nameof(RPC_SetCustomMaterial), RpcTarget.All, skinMaterial);
        }
        
        [PunRPC]
        private void RPC_SetCustomMaterial(string skinMaterial)
        {
            _unit.Builder.SetCustomMaterial(skinMaterial);
        }

        private void SetItems()
        {
            var startWeapons = PhotonHandler.GetPlayerHash<string[]>
                (PhotonHandler.Hash.Weapons, photonView.Owner);

            var characterItems = PhotonHandler.GetPlayerHash<string[]>
                (PhotonHandler.Hash.Character, photonView.Owner);

            _unit.Buffer.ChangeItems(characterItems, true, ItemInfo.Catalog.Character);
            _unit.Buffer.ChangeItems(startWeapons, true, ItemInfo.Catalog.Weapons);
        }

        private async void SetExperience()
        {
            _unit.Buffer.ChangeExperience();
            _unit.Buffer.ChangeLevel();
        }

        private void SetResources()
        {
            var _lives = PhotonHandler.GetPlayerHash<int>
                (PhotonHandler.Hash.Lives, photonView.Owner);
        
            var _localCurrency = PhotonHandler.GetPlayerHash<int>
                (PhotonHandler.Hash.LocalCurrency, photonView.Owner);
        
            var _mainCurrency = PhotonHandler.GetPlayerHash<int>
                (PhotonHandler.Hash.MainCurrency, photonView.Owner);
            
            _unit.Buffer.ChangeLives(_lives, true);
            _unit.Buffer.ChangeCurrency(_localCurrency, true, ItemInfo.Currency.BC);
            _unit.Buffer.ChangeCurrency(_mainCurrency, true, ItemInfo.Currency.GL);
        }

        private async void SetMove()
        {
            var _runSpeed = PhotonHandler.GetPlayerHash<int>
                (PhotonHandler.Hash.RunSpeed, photonView.Owner);
            
            var _sprintSpeed = PhotonHandler.GetPlayerHash<int>
                (PhotonHandler.Hash.SprintSpeed, photonView.Owner);

            photonView.RPC(nameof(RPC_SetMoveValues), RpcTarget.All, _runSpeed, _sprintSpeed);
        }
        
        [PunRPC]
        private void RPC_SetMoveValues(int RunSpeed, int SprintSpeed)
        {
            _unit.GetComponent<UnitAiMove>().SetMoveValues(RunSpeed, SprintSpeed);
        }

        private void SetHealth()
        {
            var _health = PhotonHandler.GetPlayerHash<int>
                (PhotonHandler.Hash.Health, photonView.Owner);

            _unit.Buffer.ChangeHealth(_health, true, false);
            _unit.Health.ActiveHealthUi(true);
        }

        private void SetMaxValues()
        {
            var _maxBagSlots = PhotonHandler.GetPlayerHash<int>
                (PhotonHandler.Hash.BagSlots, photonView.Owner);
            
            var _maxLives = PhotonHandler.GetPlayerHash<int>
                (PhotonHandler.Hash.MaxLives, photonView.Owner);
            
            var _maxExperience = PhotonHandler.GetPlayerHash<int>
                (PhotonHandler.Hash.MaxExperience, photonView.Owner);
            
            var _maxEndurance = PhotonHandler.GetPlayerHash<int>
                (PhotonHandler.Hash.MaxEndurance, photonView.Owner);
            
            var _maxHealth = PhotonHandler.GetPlayerHash<int>
                (PhotonHandler.Hash.MaxHealth, photonView.Owner);

            photonView.RPC(nameof(RPC_SetMaxValues), RpcTarget.All,
                _maxBagSlots, _maxLives, _maxExperience, _maxEndurance, _maxHealth);
        }

        [PunRPC]
        private void RPC_SetMaxValues(int MaxBagSlots, int MaxLives, int MaxExp, 
            int MaxEndurance, int MaxHealth)
        {
            _unit.Items.SetMaxBagSlots(MaxBagSlots);
            _unit.Lives.SetMaxLives(MaxLives);
            _unit.Experience.SetMaxExperience(MaxExp);
            _unit.Sprint.SetMaxEndurance(MaxEndurance);
            _unit.Sprint.UpdateEndurance(MaxEndurance);
            _unit.Health.SetMaxHealth(MaxHealth);
        }
    }
}