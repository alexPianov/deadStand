using CodeStage.AntiCheat.ObscuredTypes;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Playstel
{
    public class ItemFirearm : MonoBehaviourPun
    {
        [Header("Refs")]
        public Transform muzzlePos;
        public Transform sparkPos;
        public Transform shellPos;
        public Transform aimPos;
        public Transform holderInstance;

        public Unit Unit;
        public ItemInfo Info;
        public ItemAmmo Ammo;
        public ItemStat Stat;
        public ItemFirearmReloader Reloader;
        public ItemFirearmLaserSight LaserSight;
        public ItemFirearmEffects Effects;

        private Transform _unitTransform;

        #region Initialize

        public void SetComponents(Unit unit, ItemInfo weaponInfo)
        {
            Info = weaponInfo;
            Unit = unit;
            _unitTransform = Unit.transform;
            
            Stat = GetComponent<ItemStat>();
            Stat.SetInfo(weaponInfo);
        }

        public void SetLaserSight()
        {
            LaserSight = GetComponentInChildren<ItemFirearmLaserSight>();
            
            if (Unit.IsNPC)
            {
                LaserSight.VisibleLine(false);
            }
        }

        public void SetAmmo(ItemAmmo ammo)
        {
            Ammo = ammo;
        }

        public void SetReloader(ItemFirearmReloader reloader)
        {
            Reloader = reloader;
            Reloader.SetFirearm(this);
        }
        
        public void SetEffects(ItemFirearmEffects effects, ItemSFX sfx)
        {
            Effects = effects;
            Effects.SetSFX(sfx);
            Effects.SetFirearm(this);
            Effects.CreateShotEffects();
        }

        #endregion

        #region Shot
        
        public void Shot(Vector3[] shotOrientations)
        {
            if (photonView.IsMine && !Unit.IsNPC)
            {
                if (ReloadRequierd(Ammo.Payload.GetBullets())) return;
            }

            foreach (var shotVector in shotOrientations)
            {
                BulletSpread(shotVector);
                CreateBullet();
            }

            Effects.ShotEffect();
            
            SubtractBullet();

            if (Stat.eachShotReload) Reloader.ReloadEachShot();
        }

        private void BulletSpread(Vector3 muzzleOrientation)
        {
            muzzlePos.LookAt(muzzleOrientation);
        }

        private void SubtractBullet()
        {
            var bullets = Ammo.Payload.GetBullets() - 1;

            Ammo.Payload.UpdateBullets(bullets);

            ReloadRequierd(bullets);
        }

        private bool ReloadRequierd(int bullets)
        {
            if (bullets < 1)
            {
                Unit.Callback.FirearmReload();
                return true;
            }

            return false;
        }

        #endregion

        #region Bullet

        private void CreateBullet()
        {
            var bullet = Ammo.GetProjectile();
 
            if (!bullet) return;

            var _bullet = bullet.transform;
            _bullet.rotation = muzzlePos.rotation;
            _bullet.position = muzzlePos.position;

            if (bullet.TryGetComponent(out ItemProjectile projectile))
            {
                projectile.StartBullet(this);
            }
        }

        #endregion

        #region Spread

        private float spread;
        private ObscuredInt maxSpread = 10;
        private ObscuredInt minSpread = 2;
        Vector3 finalOrientation;
        private int spreadDivisor = 100;

        public Vector3 GetRandomOrientation(Vector3 aimTarget)
        {
            var aimingDistance = Unit.Aim.GetAimingDistance(aimTarget);
            var distance = Mathf.RoundToInt(aimingDistance);

            spread = maxSpread + distance - Stat.accuracy;

            if (spread < minSpread) spread = minSpread;

            float SpreadX = Random.Range(spread, -spread) *
                            Vector3.Distance(_unitTransform.position, aimTarget);

            SpreadX /= spreadDivisor;

            float SpreadY = Random.Range(spread, -spread) *
                            Vector3.Distance(_unitTransform.position, aimTarget);

            SpreadY /= spreadDivisor;

            float SpreadZ = Random.Range(spread, -spread) *
                            Vector3.Distance(_unitTransform.position, aimTarget);

            SpreadZ /= spreadDivisor;

            return aimPos.position + new Vector3
                (SpreadX, SpreadY, SpreadZ);
        }

        #endregion
    }
}
