using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    public class DeckManagerAnimator
    {
        public async UniTask DrawCardsSequence(List<Card> cards, Transform cardSpawnRoot, Action onSequenceStart, Action onSequenceEnd)
        {
            onSequenceStart?.Invoke();
            cards.ForEach(c => c.GraphicsRoot.transform.SetParent(cardSpawnRoot, false));
            
            await UniTask.Delay(TimeSpan.FromSeconds(1)); //wait for ui panels to update
            await AnimateDrawCards(cards);
            cards.ForEach(c =>
            {
                c.GraphicsRoot.transform.SetParent(c.transform, true);
                c.GraphicsRoot.transform.localPosition = Vector3.zero;
            });
            onSequenceEnd?.Invoke();
        }

        public async UniTask AnimateDrawCards(List<Card> cards)
        {
            await UniTask.DelayFrame(1);
            var delayAfterEachCard = 1f;
            var durationEachCard = 1f;
            var totalTimeForDraw = cards.Count * delayAfterEachCard + (durationEachCard - delayAfterEachCard);
            for (var i = cards.Count - 1; i >= 0; i--)
            {
                Card c = cards[i];
                var tween = c.GraphicsRoot.transform.DOMove(c.transform.position, durationEachCard).SetEase(Ease.OutSine)
                    .SetDelay(i * delayAfterEachCard);
                tween.OnStart(() => c.PlayFlipFromReverseToForeground());
            }

            await UniTask.Delay(TimeSpan.FromSeconds(totalTimeForDraw));
            Debug.Log("Draw animations finished");
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
    }
}