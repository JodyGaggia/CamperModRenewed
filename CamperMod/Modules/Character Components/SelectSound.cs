using UnityEngine;

namespace CamperMod.Modules
{
    public class SelectSound : MonoBehaviour
    {
        private uint playID;

        private void OnDestroy()
        {
            if(playID != default(uint))
                AkSoundEngine.StopPlayingID(playID);
        }

        private void OnEnable()
        {
            this.playID = AkSoundEngine.PostEvent("PlayMenuSounds", base.gameObject);
        }
    }
}
