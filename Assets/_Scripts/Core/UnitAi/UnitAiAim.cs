namespace Playstel
{
    public class UnitAiAim : UnitAim
    {
        protected override void Update()
        {
            UpdateAnimatorAim();
            AimingRigWeight(aiming);
            LaserSightMode(aiming);

            if (aiming)
            {
                
            }
            
            if (attack)
            {
                if (_unit.HandleItems && _unit.HandleItems.currentItemController)
                {
                    _unit.HandleItems.currentItemController.Attack();
                }
            }
        }
    }
}