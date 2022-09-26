namespace CamperMod.SkillStates
{
    public class Medkit : BaseHealState
    {
        private float timeSinceLastHit;
        private float duration = Modules.StaticValues.medkitDuration;

        public override void OnEnter()
        {
            this.hpsCoefficient = Modules.StaticValues.medkitHPSCoefficient;

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                this.timeSinceLastHit = this.healthComponent.timeSinceLastHit;

                if (this.timeSinceLastHit <= this.fixedAge || this.fixedAge >= this.duration)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }
    }
}