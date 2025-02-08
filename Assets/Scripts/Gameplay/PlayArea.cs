using System;
using System.Collections.Generic;
using Const;
using DefaultNamespace;
using UnityEngine;

public class PlayArea : MonoBehaviour
{
    private readonly List<Card> _cardsInPlayArea = new();

    private void Awake()
    {
        EventManager.AddListener<PlayCardEvent>(OnPlayCardEvent);
        EventManager.AddListener<RemoveCardsFromPlayAreaEvent>(OnRemoveCardsFromPlayAreaEvent);
    }

    private void OnPlayCardEvent(PlayCardEvent playCardEvent)
    {
        Debug.Log($"Playarea PlayCard {playCardEvent.PlayedCard.name}");
        EventManager.Invoke(new RemoveCardFromHandEvent(playCardEvent.PlayedCard));
        _cardsInPlayArea.Add(playCardEvent.PlayedCard);
        playCardEvent.PlayedCard.transform.SetParent(this.transform);
    }
    
    private void OnRemoveCardsFromPlayAreaEvent(RemoveCardsFromPlayAreaEvent evt)
    {
        RemoveAllCards();
    }
    
    public void RemoveAllCards()
    {
        EventManager.Invoke(new DiscardMultipleCardsToPileEvent(_cardsInPlayArea));
        _cardsInPlayArea.Clear();
    }

    private void RemoveCard(Card card)
    {
        EventManager.Invoke(new DiscardCardToPileEvent(card));
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<PlayCardEvent>(OnPlayCardEvent);
    }
}