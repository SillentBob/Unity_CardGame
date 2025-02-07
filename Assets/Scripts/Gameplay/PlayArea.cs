using System;
using System.Collections.Generic;
using Const;
using UnityEngine;

public class PlayArea : MonoBehaviour
{
    private readonly List<Card> _cardsInPlayArea = new();
    private DiscardPile _discardPile;

    private void Start()
    {
        _discardPile = GameObject.FindGameObjectWithTag(GameObjectTags.DISCARD_PILE).GetComponent<DiscardPile>();
    }

    public void PlayCard(Card card)
    {
        Debug.Log($"Playarea PlayCard {card.name}");
        _cardsInPlayArea.Add(card);
        card.transform.SetParent(this.transform);
    }

    public void RemoveCard(Card card)
    {
        _discardPile.AddCardToDiscardPile(card); //TODO change
    }
}