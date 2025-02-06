using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<CardModel> cardDatabase;
    public List<CardModel> deck = new();
    public GameObject cardPrefab;
    public int startingHandSize = 5;
    public Hand hand;

    private void Start()
    {
        InitializeDeck();
        ShuffleDeck();
        DrawStartingHand();
    }

    private void InitializeDeck()
    {
        for (int i = 0; i < 20; i++)
        {
            deck.Add(cardDatabase[0]);
        }

        for (int i = 0; i < 10; i++)
        {
            deck.Add(cardDatabase[1]);
        }
    }

    private void ShuffleDeck()
    {
        System.Random rng = new System.Random();
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (deck[k], deck[n]) = (deck[n], deck[k]); //swap
        }
    }

    private void DrawStartingHand()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            DrawCard();
        }
    }

    private void DrawCard()
    {
        if (deck.Count > 0)
        {
            CardModel cardData = deck[0]; // Pobierz CardData z talii.
            deck.RemoveAt(0);

            GameObject cardObject = Instantiate(cardPrefab, hand.transform);
            Card card = cardObject.GetComponent<Card>();
            card.Setup(cardData);
            hand.AddCardToHand(card);
        }
        else
        {
            Debug.Log("Deck to draw from is empty!");
        }
    }

    private void EndTurn()
    {
        //TODO apply cards effects
        
        // Dociągnij brakujące karty do ręki
        int cardsToDraw = startingHandSize - hand.GetCardCount();
        for (int i = 0; i < cardsToDraw; i++)
        {
            DrawCard();
        }
    }
}