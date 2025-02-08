using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private List<Card> _cardsInHand = new();

    private void Awake()
    {
        EventManager.AddListener<RemoveCardFromHandEvent>(OnRemoveCardFromHandEvent);
        EventManager.AddListener<AddCardToHandEvent>(OnAddCardToHandEvent);
    }

    private void OnAddCardToHandEvent(AddCardToHandEvent e)
    {
        AddCardToHand(e.Card);
    }
    
    private void OnRemoveCardFromHandEvent(RemoveCardFromHandEvent e)
    {
        RemoveCardFromHand(e.Card);
    }

    private void AddCardToHand(Card card)
    {
        _cardsInHand.Add(card);
    }

    private void RemoveCardFromHand(Card card)
    {
        _cardsInHand.Remove(card);
    }

    public int GetCardCount()
    {
        return _cardsInHand.Count;
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<AddCardToHandEvent>(OnAddCardToHandEvent);
        EventManager.RemoveListener<RemoveCardFromHandEvent>(OnRemoveCardFromHandEvent);
    }
}