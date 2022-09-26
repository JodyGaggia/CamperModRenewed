using UnityEngine;
using RoR2;

namespace CamperMod.Modules
{
    [RequireComponent(typeof(InputBankTest))]
    [RequireComponent(typeof(TeamComponent))]
    [RequireComponent(typeof(CharacterBody))]
    public class DSTracker : MonoBehaviour
    {
        public float maxTrackingDistance = 20f;
        public float maxTrackingAngle = 20f;
        public float trackerUpdateFrequency = 10f;

        private HurtBox trackingTarget;
        private CharacterBody characterBody;
        private TeamComponent teamComponent;
        private InputBankTest inputBank;
        private float trackerUpdateStopwatch;
        private Indicator indicator;
        private readonly BullseyeSearch search = new BullseyeSearch();

        private void Awake()
        {
            this.indicator = new Indicator(base.gameObject, LegacyResourcesAPI.Load<GameObject>("Prefabs/HuntressTrackingIndicator"));
        }

        private void Start()
        {
            this.characterBody = base.GetComponent<CharacterBody>();
            this.inputBank = base.GetComponent<InputBankTest>();
            this.teamComponent = base.GetComponent<TeamComponent>();

            // We don't need the tracker if we don't have Decisive Strike
            if (!characterBody.skillLocator.special.isCombatSkill) Destroy(base.gameObject.GetComponent<DSTracker>());
        }

        public HurtBox GetTrackingTarget()
        {
            return this.trackingTarget;
        }

        private void OnEnable()
        {
            this.indicator.active = true;
        }

        private void OnDisable()
        {
            this.indicator.active = false;
        }

        private void FixedUpdate()
        {
            this.trackerUpdateStopwatch += Time.fixedDeltaTime;
            if (this.trackerUpdateStopwatch >= 1f / this.trackerUpdateFrequency)
            {
                this.trackerUpdateStopwatch -= 1f / this.trackerUpdateFrequency;
                HurtBox hurtBox = this.trackingTarget;
                Ray aimRay = new Ray(this.inputBank.aimOrigin, this.inputBank.aimDirection);
                this.SearchForTarget(aimRay);
                this.indicator.targetTransform = (this.trackingTarget ? this.trackingTarget.transform : null);
            }

            if(characterBody.skillLocator.special.stock == 0) this.indicator.active = false;
            else this.indicator.active = true;
            
        }

        private void SearchForTarget(Ray aimRay)
        {
            this.search.teamMaskFilter = TeamMask.GetUnprotectedTeams(this.teamComponent.teamIndex);
            this.search.filterByLoS = true;
            this.search.searchOrigin = aimRay.origin;
            this.search.searchDirection = aimRay.direction;
            this.search.sortMode = BullseyeSearch.SortMode.Angle;
            this.search.maxDistanceFilter = this.maxTrackingDistance;
            this.search.maxAngleFilter = this.maxTrackingAngle;
            this.search.RefreshCandidates();
            this.search.FilterOutGameObject(base.gameObject);
            this.trackingTarget = default(HurtBox);

            // Filter out flying enemies
            foreach(HurtBox enemyHurtbox in this.search.GetResults())
            {
                if (enemyHurtbox.healthComponent.gameObject.GetComponent<CharacterMotor>())
                {
                    BodyIndex enemyBodyIndex = enemyHurtbox.healthComponent.gameObject.GetComponent<CharacterBody>().bodyIndex;

                    // fuck you blind pests and vultures
                    if(enemyBodyIndex != BodyCatalog.FindBodyIndex("FlyingVerminBody") && enemyBodyIndex != BodyCatalog.FindBodyIndex("VultureBody"))
                        this.trackingTarget = enemyHurtbox;
                    break;
                }
            }
        }
    }
}
