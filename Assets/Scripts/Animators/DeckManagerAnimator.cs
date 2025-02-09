using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    
    public class DeckManagerAnimator
    {
        private readonly float _drawCardMoveDuration = 0.8f;
        private readonly float _drawCardDelayEachDraw = 0.5f;
        private readonly float _discardCardMoveDuration = 1f;
        private readonly float _discardCardDelayEachDraw = 0.2f;
        
        public async UniTask DrawCardsSequence(List<Card> cards, Transform detachAnchor, Action onSequenceStart,
            Action onSequenceEnd)
        {
            onSequenceStart?.Invoke();
            cards.ForEach(c => c.GraphicsRoot.transform.SetParent(detachAnchor, false));
            await UniTask.DelayFrame(1);
            await AnimateMoveCardsToTarget(cards, null, _drawCardMoveDuration, _drawCardDelayEachDraw, (c) => c.PlayFlipFromReverseToForeground());
            cards.ForEach(c =>
            {
                c.GraphicsRoot.transform.SetParent(c.transform, true);
                c.GraphicsRoot.transform.localPosition = Vector3.zero;
            });
            onSequenceEnd?.Invoke();
        }

        public async UniTask DiscardCardsSequence(List<Card> cards, Transform detachAnchor,
            Action onSequenceStart, Action onSequenceEnd)
        {
            onSequenceStart?.Invoke();
            cards.ForEach(c => c.GraphicsRoot.transform.SetParent(detachAnchor, true));
            await UniTask.DelayFrame(1);
            await AnimateMoveCardsToTarget(cards, null, _discardCardMoveDuration, _discardCardDelayEachDraw, (c) => c.PlayFlipToReverse());
            cards.ForEach(c =>
            {
                c.GraphicsRoot.transform.SetParent(c.transform, true);
                c.GraphicsRoot.transform.localPosition = Vector3.zero;
            });
            onSequenceEnd?.Invoke();
        }

        public void PlayEffectsOnCardPlayed(ParticleSystem particle)
        {
            if (particle == null)
            {
                return;
            }

            particle.gameObject.SetActive(true);
            particle.Stop();
            particle.Play();
        }
        
        /// <param name="target"> pass null to treat as card transform target (self) </param> 
        private async UniTask AnimateMoveCardsToTarget(List<Card> cards, Transform target, float moveDuriation,
            float startDelay, Action<Card> onAnimStart)
        {
            await UniTask.DelayFrame(1);
            List<UniTask> animationTasks = new();
            for (var i = 0; i < cards.Count; i++)
            {
                Card c = cards[i];
                var t = AnimateMoveCard(c, target ? target : c.transform, moveDuriation, startDelay*(i+1), onAnimStart);
                animationTasks.Add(t);
            }

            await UniTask.WhenAll(animationTasks);
        }

        private async UniTask AnimateMoveCard(Card c, Transform target, float moveDuriation, float startDelay,
            Action<Card> onAnimStart)
        {
            var tween = c.GraphicsRoot.transform.DOMove(target.position, moveDuriation).SetEase(Ease.OutSine);
            tween.SetAutoKill();
            tween.SetDelay(startDelay);
            tween.OnStart(() => onAnimStart?.Invoke(c));
            await UniTask.Delay(TimeSpan.FromSeconds(moveDuriation + startDelay));
        }
    }
}