using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DeckManager : MonoBehaviour
{
    [SerializeField] [FormerlySerializedAs("cardDb")] 
    private CardsDatabase cardDatabase;
    
    public GameSettings gameSettings;
    public GameObject cardPrefab;
    public Hand hand;

    private readonly List<CardModel> _deck = new();

    private void Start()
    {
        InitializeDeck();
        ShuffleDeck();
        // DrawStartingHand();
    }

    private void InitializeDeck()
    {
        var variousCardsCount = cardDatabase.cardModels.Count;
        for (int i = 0; i < gameSettings.initialDeckSize; i++)
        {
            _deck.Add(cardDatabase.cardModels[i%variousCardsCount]);
        }
    }

    private void ShuffleDeck()
    {
        System.Random rng = new System.Random();
        int n = _deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (_deck[k], _deck[n]) = (_deck[n], _deck[k]); //swap
        }
    }

    private void DrawStartingHand()
    {
        for (int i = 0; i < gameSettings.playerHandSize; i++)
        {
            DrawCard();
        }
    }

    private void DrawCard()
    {
        if (_deck.Count > 0)
        {
            CardModel cardData = _deck[0];
            _deck.RemoveAt(0);

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
        int cardsToDraw = gameSettings.playerHandSize - hand.GetCardCount();
        for (int i = 0; i < cardsToDraw; i++)
        {
            DrawCard();
        }
    }
}