namespace CamperMod.SkillStates
{
    public class RemoteFirecracker : BaseFirecrackerState
    {
        public override void OnEnter()
        {
            this.isRemote = true;

            this.baseCharges = 1f;
            this.radius = 15f;
            this.procCoefficient = 0.4f;
            this.playerDamageCoefficient = 0.15f;

            this.airForce = 7f;
            this.groundForce = 5f;

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}