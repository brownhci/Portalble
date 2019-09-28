using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour {
    private bool invincible = false;
    public float Health = 1000f;

    /* update commented session */
    //public RedFlashEffect RedEffect;

    private long[] vibratePattern = new long[] { 0, 200, 50, 200, 50, 300 };

    void OnTriggerEnter(Collider other) {
        // Since only Boxer's hit can hit this, so deal damage here
        /* update commented session */
        //BoxerAI bai = BoxerGameController.Instance.GetBoxer();

        /* update commented session */
        //if (!invincible && bai.isAttackAvailable()) {
        //    // deal damage
        //    invincible = true;
        //    Health -= 100f;
        //    Vibration.Vibrate(vibratePattern, -1);
        //    if (RedEffect != null) {
        //        RedEffect.Flash(0.5f);
        //    }
        //    Invoke("BeAbleToBeHit", 0.7f);
        //}
    }

    void BeAbleToBeHit() {
        invincible = false;
    }
}
