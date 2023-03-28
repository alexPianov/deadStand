using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    public static class KeyStore
    {
        #region Regions

        //https://doc.photonengine.com/en-us/pun/current/connection-and-authentication/regions
        
        public static string GetRegion(int regionNumber)
        {
            switch (regionNumber)
            {
                case 1: return "asia"; 
                case 2: return "au";
                case 3: return "cae";
                case 4: return "eu";
                case 5: return "in";
                case 6: return "jp"; 
                case 7: return "ru"; 
                case 8: return "rue"; 
                case 9: return "za"; 
                case 10: return "sa"; 
                case 11: return "kr";
                case 12: return "tr";
                case 13: return "us";
                case 14: return "usw";
                case 15: return "cn";
            }

            return null;
        }
        
        public static int GetRegion(string regionNumber)
        {
            switch (regionNumber)
            {
                case "asia": return 1; 
                case "au": return 2;
                case "cae": return 3; 
                case "eu": return 4; 
                case "in": return 5;
                case "jp": return 6;
                case "ru": return 7;
                case "rue": return 8; 
                case "za": return 9; 
                case "sa": return 10; 
                case "kr": return 11; 
                case "tr": return 12;
                case "us": return 13;
                case "usw": return 14;
                case "cn": return 15;
            }

            return 0;
        }

        #endregion

        #region External Links

        public static string SITE_REF = "http://playstel.com";
        public static string TERMS_REF = "http://playstel.com/ds-terms";
        public static string PRIVATE_POLICY_REF = "http://playstel.com/ds-private";
        public static string SUPPORT_REF = "http://playstel.com/ds-support";
        public static string VERSIONS_REF = "http://playstel.com/ds-versions";
        public static string RATE_REF = "http://playstel.com/ds-rate";

        #endregion

        #region Addressable

        public static string SCENE_KEY = "SCENE_KEY";
        
        public static string VFX_GRAB = "V_Itm_Grab";
        public static string VFX_DUST = "V_Col_Dust";
        public static string VFX_BLOOD = "V_Col_Blood";
        
        public static string SFX_HIT_FLESH = "S_Hit_Flesh";
        public static string SFX_HIT_SOLID = "S_Hit_Solid";
        
        public static string SFX_IMPACT_FIRE = "S_Impact_Fire";
        public static string SFX_IMPACT_SHOCK = "S_Impact_Shock";

        #endregion

        #region Item Database

        public static ItemInfo.Catalog GetCatalog(string itemCaltalog)
        {
            switch (itemCaltalog)
            {
                case "Weapons": return ItemInfo.Catalog.Weapons;
                case "Support": return ItemInfo.Catalog.Support;
                case "Character": return ItemInfo.Catalog.Character;
                case "Backpack": return ItemInfo.Catalog.Backpack;
                case "Shop": return ItemInfo.Catalog.Shop;
                case "Setup": return ItemInfo.Catalog.Setup;
            }

            return ItemInfo.Catalog.Null;
        }
        
        public static ItemInfo.Class GetClass(string itemClass)
        {
            switch (itemClass)
            {
                case "Ammo": return ItemInfo.Class.Ammo;
                case "Firearm": return ItemInfo.Class.Firearm;
                case "Fuel": return ItemInfo.Class.Fuel;
                case "Grenade": return ItemInfo.Class.Grenade;
                case "Drug": return ItemInfo.Class.Drug;
                case "Impact": return ItemInfo.Class.Impact;
                case "Loot": return ItemInfo.Class.Loot;
                case "Melee": return ItemInfo.Class.Melee;
                case "Room": return ItemInfo.Class.Room;
                case "Rig": return ItemInfo.Class.Rig;
                case "Stuff": return ItemInfo.Class.Stuff;
                case "Unit": return ItemInfo.Class.Unit;
                case "Scheme": return ItemInfo.Class.Scheme;
                case "Pass": return ItemInfo.Class.Pass;
                case "Car": return ItemInfo.Class.Car;
                case "Currency": return ItemInfo.Class.Currency;
                case "Upgrade": return ItemInfo.Class.Upgrade;
                case "Crate": return ItemInfo.Class.Crate;
                case "Season": return ItemInfo.Class.Season;
                case "Pack": return ItemInfo.Class.Pack;
                case "Referral": return ItemInfo.Class.Referral;
            }

            return ItemInfo.Class.Null;
        }
        
        public static ItemInfo.Subclass GetSubclass(string itemSubclass)
        {
            switch (itemSubclass)
            {
                case "Autogun": return ItemInfo.Subclass.Autogun;
                case "Rucksack": return ItemInfo.Subclass.Rucksack;
                case "Beard": return ItemInfo.Subclass.Beard;
                case "Blast": return ItemInfo.Subclass.Blast;
                case "Blunt": return ItemInfo.Subclass.Blunt;
                case "Collectible": return ItemInfo.Subclass.Collectible;
                case "Common": return ItemInfo.Subclass.Common;
                case "Cutting": return ItemInfo.Subclass.Cutting;
                case "Health": return ItemInfo.Subclass.Health;
                case "Electro": return ItemInfo.Subclass.Electro;
                case "Fire": return ItemInfo.Subclass.Fire;
                case "Food": return ItemInfo.Subclass.Food;
                case "Glasses": return ItemInfo.Subclass.Glasses;
                case "Headwear": return ItemInfo.Subclass.Headwear;
                case "Heavy": return ItemInfo.Subclass.Heavy;
                case "Piercing": return ItemInfo.Subclass.Piercing;
                case "Stim": return ItemInfo.Subclass.Stim;
                case "Pistol": return ItemInfo.Subclass.Pistol;
                case "Apocalypse": return ItemInfo.Subclass.Apocalypse;
                case "Player": return ItemInfo.Subclass.Player;
                case "Rifle": return ItemInfo.Subclass.Rifle;
                case "Sawing": return ItemInfo.Subclass.Sawing;
                case "Shotgun": return ItemInfo.Subclass.Shotgun;
                case "Stabbing": return ItemInfo.Subclass.Stabbing;
                case "Toxic": return ItemInfo.Subclass.Toxic;
                case "Trinkets": return ItemInfo.Subclass.Trinkets;
                case "Zombie": return ItemInfo.Subclass.Zombies;
                case "BossZombies": return ItemInfo.Subclass.BossZombies;
                case "Gang": return ItemInfo.Subclass.Gang;
                case "BattleRoyale": return ItemInfo.Subclass.BattleRoyale;
                case "CarFuel": return ItemInfo.Subclass.CarFuel;
                case "CarUpgrade": return ItemInfo.Subclass.CarUpgrade;
                case "LightBullets": return ItemInfo.Subclass.LightBullets;
                case "PiercingBullets": return ItemInfo.Subclass.PiercingBullets;
                case "RifleBullets": return ItemInfo.Subclass.RifleBullets;
                case "ShotgunBullets": return ItemInfo.Subclass.ShotgunBullets;
                case "UnitUpgrade": return ItemInfo.Subclass.UnitUpgrade;
                case "WeaponUpgrade": return ItemInfo.Subclass.WeaponUpgrade;
                case "NPC": return ItemInfo.Subclass.NPC;
                case "SMG": return ItemInfo.Subclass.SMG;
                case "Gold": return ItemInfo.Subclass.Gold;
                case "Upgrades": return ItemInfo.Subclass.Upgrades;
                case "Anarchy": return ItemInfo.Subclass.Anarchy;
            }

            return ItemInfo.Subclass.Null;
        }

        #endregion
    }
}
