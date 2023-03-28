using System;
using NukeFactory;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class Unit : MonoBehaviourPun
    {
        public bool IsNPC;
        public string unitName;

        [HideInInspector] public UnitItems Items;
        [HideInInspector] public UnitItemSpawner ItemSpawner;
        [HideInInspector] public UnitHandleItems HandleItems;
        [HideInInspector] public UnitSprint Sprint;
        [HideInInspector] public UnitSkin Skin;
        [HideInInspector] public UnitInteraction Interaction;
        [HideInInspector] public UnitHostOperator HostOperator;
        [HideInInspector] public UnitBoot Boot;
        [HideInInspector] public UnitAwait Await;
        [HideInInspector] public UnitCurrency Currency;
        [HideInInspector] public UnitRenderer Renderer;
        [HideInInspector] public UnitExperience Experience;
        [HideInInspector] public UnitTokens Tokens;
        [HideInInspector] public UnitBooster Booster;
        [HideInInspector] public UnitGizmo Gizmo;
        [HideInInspector] public UnitCallback Callback;
        [HideInInspector] public UnitAim Aim;
        [HideInInspector] public UnitGrenadeThrow GrenadeThrow;
        [HideInInspector] public UnitJoystick Joystick;
        [HideInInspector] public UnitMove Move;
        [HideInInspector] public UnitAnimation Animation;
        [HideInInspector] public UnitCamera Camera;
        [HideInInspector] public UnitRoot Root;
        [HideInInspector] public NpcCharacterSkin NpcSkin;
        [HideInInspector] public UnitBuilder Builder;
        [HideInInspector] public UnitFraction Fraction;
        [HideInInspector] public UnitHealth Health;
        [HideInInspector] public UnitImpact Impact;
        [HideInInspector] public UnitVFX VFX;
        [HideInInspector] public UnitSFX SFX;
        [HideInInspector] public UnitRagdoll Ragdoll;
        [HideInInspector] public UnitPlayTime PlayTime;
        [HideInInspector] public UnitDrugs Drugs;
        [HideInInspector] public UnitDeath Death;
        [HideInInspector] public UnitDeathScreen DeathScreen;
        [HideInInspector] public UnitLives Lives;
        [HideInInspector] public UnitAmmo Ammo;
        [HideInInspector] public UnitBuffer Buffer;
        [HideInInspector] public UnitDamage Damage;
        [HideInInspector] public UnitItemsUse ItemsUse;
        [HideInInspector] public UnitAreaBehaviour AreaBehaviour;
        [HideInInspector] public UnitPower Power;
        [HideInInspector] public UnitAiTargetGlobal TargetGlobal;

        [HideInInspector] [Inject] public CacheItemInfo CacheItemInfo;
        [HideInInspector] [Inject] public CacheUserInfo CacheUserInfo;

        private void Awake()
        {
            if (IsNPC)
            {
                TargetGlobal = GetComponent<UnitAiTargetGlobal>();
                Construct();
            }
        }

        [Inject]
        private void Construct()
        {
            Items = GetComponent<UnitItems>();
            ItemSpawner = GetComponent<UnitItemSpawner>();
            HandleItems = GetComponent<UnitHandleItems>();
            Sprint = GetComponent<UnitSprint>();
            Skin = GetComponent<UnitSkin>();
            Interaction = GetComponent<UnitInteraction>();
            HostOperator = GetComponent<UnitHostOperator>();
            Boot = GetComponent<UnitBoot>();
            Await = GetComponent<UnitAwait>();
            Currency = GetComponent<UnitCurrency>();
            Lives = GetComponent<UnitLives>();
            Renderer = GetComponent<UnitRenderer>();
            Experience = GetComponent<UnitExperience>();
            Tokens = GetComponent<UnitTokens>();
            Gizmo = GetComponent<UnitGizmo>();
            Aim = GetComponent<UnitAim>();
            Joystick = GetComponent<UnitJoystick>();
            GrenadeThrow = GetComponent<UnitGrenadeThrow>();
            Move = GetComponent<UnitMove>();
            Animation = GetComponent<UnitAnimation>();
            Camera = GetComponent<UnitCamera>();
            Root = GetComponent<UnitRoot>();
            NpcSkin = GetComponent<NpcCharacterSkin>();
            Builder = GetComponent<UnitBuilder>();
            Fraction = GetComponent<UnitFraction>();
            Health = GetComponent<UnitHealth>();
            Impact = GetComponent<UnitImpact>();
            SFX = GetComponent<UnitSFX>();
            VFX = GetComponent<UnitVFX>();
            Ragdoll = GetComponent<UnitRagdoll>();
            PlayTime = GetComponent<UnitPlayTime>();
            Drugs = GetComponent<UnitDrugs>();
            Callback = GetComponent<UnitCallback>();
            Death = GetComponent<UnitDeath>();
            DeathScreen = GetComponent<UnitDeathScreen>();
            Ammo = GetComponent<UnitAmmo>();
            Buffer = GetComponent<UnitBuffer>();
            Damage = GetComponent<UnitDamage>();
            Boot = GetComponent<UnitBoot>();
            ItemsUse = GetComponent<UnitItemsUse>();
            AreaBehaviour = GetComponent<UnitAreaBehaviour>();
            Booster = GetComponent<UnitBooster>();
            Power = GetComponent<UnitPower>();
        }
        
        public int GetViewId()
        {
            return photonView.ViewID;
        }

        public void SetUnitName(string _unitName)
        {
            unitName = _unitName;
        }
    }
}
