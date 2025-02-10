using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    public class DeckManagerAnimator
    {
        private const float _slotsCardMoveDuration = 0.5f;
        private const float _drawCardMoveDuration = 0.8f;
        private const float _drawCardDelayEachDraw = 0.5f;
        private const float _discardCardMoveDuration = 1f;
        private const float _discardCardDelayEachDraw = 0.2f;

        private UIParticle particleSystemManager;

        public async UniTask DrawCardsSequence(List<Card> cards, Transform detachAnchor, Action onSequenceStart,
            Action onSequenceEnd)
        {
            onSequenceStart?.Invoke();
            cards.ForEach(c => c.DetachGraphicsRootFromCard(detachAnchor, false));
            await UniTask.DelayFrame(1);
            await AnimateMoveCardsToTarget(cards, null, _drawCardMoveDuration, _drawCardDelayEachDraw,
                (c) => c.PlayFlipFromReverseToForeground());
            cards.ForEach(c => c.ReattachGraphicsRootToCard());
            onSequenceEnd?.Invoke();
        }

        public async UniTask DiscardCardsSequence(List<Card> cards, Transform detachAnchor,
            Action onSequenceStart, Action onSequenceEnd)
        {
            onSequenceStart?.Invoke();
            cards.ForEach(c => c.DetachGraphicsRootFromCard(detachAnchor, true));
            await UniTask.DelayFrame(1);
            await AnimateMoveCardsToTarget(cards, null, _discardCardMoveDuration, _discardCardDelayEachDraw,
                (c) => c.PlayFlipToReverse());
            cards.ForEach(c => c.ReattachGraphicsRootToCard());
            await UniTask.DelayFrame(1);
            onSequenceEnd?.Invoke();
        }

        public async UniTask MoveCardToTargetFromCurrentPosition(Card card, Vector2 targetPosition)
        {
            await AnimateMoveCard(card.GraphicsRoot.transform, targetPosition, _slotsCardMoveDuration, 0f, null);
        }

        public void PlayEffectsOnCardPlayed(ParticleSystem particle, Vector2 atPosition)
        {
            if (particle == null)
            {
                return;
            }

            if (particleSystemManager == null)
            {
                particleSystemManager = particle.GetComponentInParent<UIParticle>();
            }

            particle.Stop();
            if (particleSystemManager != null)
            {
                particleSystemManager.transform.position = atPosition;
            }

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
                var t = AnimateMoveCard(c, target ? target : c.transform, moveDuriation, startDelay * (i + 1),
                    onAnimStart);
                animationTasks.Add(t);
            }

            await UniTask.WhenAll(animationTasks);
        }

        private async UniTask AnimateMoveCard(Card c, Transform target, float moveDuriation, float startDelay,
            Action<Card> onAnimStart)
        {
            await AnimateMoveCard(c, target.position, moveDuriation, startDelay, onAnimStart);
        }

        private async UniTask AnimateMoveCard(Card c, Vector2 target, float moveDuriation, float startDelay,
            Action<Card> onAnimStart)
        {
            var tween = c.GraphicsRoot.transform.DOMove(target, moveDuriation).SetEase(Ease.OutSine);
            tween.SetDelay(startDelay);
            tween.OnStart(() => onAnimStart?.Invoke(c));
            await UniTask.Delay(TimeSpan.FromSeconds(moveDuriation + startDelay));
        }

        private async UniTask AnimateMoveCard(Transform c, Vector2 target, float moveDuriation, float startDelay,
            Action onAnimStart)
        {
            var tween = c.DOMove(target, moveDuriation).SetEase(Ease.OutSine);
            tween.SetDelay(startDelay);
            await UniTask.Delay(TimeSpan.FromSeconds(moveDuriation + startDelay));
        }
    }
}