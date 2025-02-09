using UnityEngine;

namespace Animators
{
    public class CardFlipAnimator
    {
        private static readonly int TRIGGER_FLIP_TOREVERSE = Animator.StringToHash("flipToReverse");
        private static readonly int TRIGGER_FLIP_FROMREVERSE = Animator.StringToHash("flipFromReverse");
        private Animator anim;
        public CardFlipAnimator(Animator a)
        {
            anim = a;
        }

        public void PlayFlipToReverse()
        {
            anim.SetTrigger(TRIGGER_FLIP_TOREVERSE);
        }
        public void PlayFlipFromReverseToForeground()
        {
            anim.SetTrigger(TRIGGER_FLIP_FROMREVERSE);
        }
        
    }
}