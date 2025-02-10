using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof (CardsPool))]
public partial class DeckManager
{
    private IObjectPool<Card> _cardsPool;
    private void Awake()
    {
        _cardsPool = GetComponent<CardsPool>().Pool;
    }

    private async UniTask TryMoveCardsFromPlayAreaToDiscardPile()
    {
        if (playArea.CardsInPlayArea.Count > 0)
        {
            discardPile.AddCardToPile(playArea.CardsInPlayArea);
            playArea.RemoveAllCards();
            await _animations.DiscardCardsSequence(discardPile.DiscardedCards, dragAnchor.transform, null, null);
            
        }
    }

    private void ClearDiscardedPile()
    {
        discardPile.DiscardedCards.ForEach(c => _cardsPool.Release(c));
        discardPile.DeactivateCardsOnPile();
    }
    
    private async UniTask EndTurnSequence()
    {
        EnableInput(false);

        await TryMoveCardsFromPlayAreaToDiscardPile();
        ClearDiscardedPile();
        
        int cardsToDraw = _gameSettings.playerHandSize - hand.GetCardCount();
        List<Card> newCards = new();
        for (int i = 0; i < cardsToDraw; i++)
        {
            Card c = TryDrawCardToHand();
            if (c != null)
            {
                newCards.Add(c);
                hand.AddCardToHand(c);
            }
        }

        if (newCards.Count > 0)
        {
            var oldHandCards = hand.Cards.Except(newCards).ToList();
            oldHandCards.ForEach(cc => cc.DetachGraphicsRootFromCard(dragAnchor.transform, true));
            List<UniTask> tasks = new();
            tasks.Add(_animations.DrawCardsSequence(newCards, cardSpawnRoot.transform, null, null));
            await UniTask.DelayFrame(1);
            oldHandCards.ForEach(cc =>
                tasks.Add(_animations.MoveCardToTargetFromCurrentPosition(cc, cc.transform.position)));
            await UniTask.WhenAll(tasks);
            oldHandCards.ForEach(cc => cc.ReattachGraphicsRootToCard());
        }

        if (_deck.Count == 0)
        {
            Debug.Log("No more cards to draw!");
            SetDrawDeckVisible(false);
        }

        EnableInput(true);
    }
    
    private async UniTask OnSwapCardPlacesSequence(Card draggedCard, int draggedCardIdx, Card stationaryCard,
        int stationaryCardIdx)
    {
        EnableInput(false);

        var positionStationaryOrigin = stationaryCard.transform.position;
        var positionDraggedOrigin = draggedCard.transform.position;
        draggedCard.DetachGraphicsRootFromCard(dragAnchor.transform, true);
        stationaryCard.DetachGraphicsRootFromCard(dragAnchor.transform, true);
        List<UniTask> moveTasks = new()
        {
            _animations.MoveCardToTargetFromCurrentPosition(draggedCard, positionStationaryOrigin),
            _animations.MoveCardToTargetFromCurrentPosition(stationaryCard, positionDraggedOrigin)
        };
        await UniTask.WhenAll(moveTasks);

        hand.ReorderCard(draggedCard, draggedCardIdx, stationaryCardIdx);
        hand.ReorderCard(stationaryCard, stationaryCardIdx, draggedCardIdx);
        //restore card gfx localposition after moving parent objects, becouse they have delta position now
        await UniTask.DelayFrame(1);
        draggedCard.ReattachGraphicsRootToCard();
        stationaryCard.ReattachGraphicsRootToCard();

        EnableInput(true);
    }
    
    private async UniTask PlayCardSequence(Card card)
    {
        EnableInput(false);
        card.OnPlayCard -= OnPlayCard;
        hand.RemoveCardFromHand(card);

        playArea.CardsInPlayArea.ForEach(c => c.DetachGraphicsRootFromCard(dragAnchor.transform, true));
        hand.Cards.ForEach(c=> c.DetachGraphicsRootFromCard(dragAnchor.transform, true));
        playArea.AddCardToPlayArea(card);
        
        await UniTask.DelayFrame(1);
        List<UniTask> asyncTasks = new();
        hand.Cards.ForEach(c =>
            asyncTasks.Add(_animations.MoveCardToTargetFromCurrentPosition(c, c.transform.position)));
        playArea.CardsInPlayArea.ForEach(c =>
            asyncTasks.Add(_animations.MoveCardToTargetFromCurrentPosition(c, c.transform.position)));
        asyncTasks.Add(_animations.MoveCardToTargetFromCurrentPosition(card, card.transform.position));
        await UniTask.WhenAll(asyncTasks);
        playArea.CardsInPlayArea.ForEach(c => c.ReattachGraphicsRootToCard());
        hand.Cards.ForEach(c=> c.ReattachGraphicsRootToCard());
        
        EnableInput(true);
        _animations.PlayEffectsOnCardPlayed(particle, card.transform.position);
    }
}