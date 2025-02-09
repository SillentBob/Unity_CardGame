using UnityEngine;

namespace Animators
{
    public class CardFlipAnimator : MonoBehaviour
    {
        [SerializeField] private Animator cardAnimator;
        
        private static readonly int TRIGGER_FLIP_TOREVERSE = Animator.StringToHash("flipToReverse");
        private static readonly int TRIGGER_FLIP_FROMREVERSE = Animator.StringToHash("flipFromReverse");
        
        public void PlayFlipToReverse()
        {
            cardAnimator?.SetTrigger(TRIGGER_FLIP_TOREVERSE);
        }
        public void PlayFlipFromReverseToForeground()
        {
            cardAnimator?.SetTrigger(TRIGGER_FLIP_FROMREVERSE);
        }
        
    }
}