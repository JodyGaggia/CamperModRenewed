namespace CamperMod.SkillStates
{
    public class DropFirecracker : BaseFirecrackerState
    {
        public override void OnEnter()
        {
            this.isRemote = false;

            this.baseDuration = 1f;
            this.baseChargeTime = 0.5f;
            this.radius = 20f;
            this.procCoefficient = 1f;
            this.playerDamageCoefficient = 0.2f;

            this.airForce = 15f;
            this.groundForce = 2f;

            base.OnEnter();
        }
    }
}