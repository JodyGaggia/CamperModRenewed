namespace CamperMod.SkillStates
{
    public class SelfCare : BaseHealState
    {
        public override void OnEnter()
        {
            this.isFragile = false;
            this.hptCoefficient = 0.0009f;

            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}