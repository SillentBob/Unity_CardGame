using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using Gameplay;
using UnityEngine;
using Random = System.Random;

public class DeckManager : MonoBehaviour
{
    [Header("Mandatory")] [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject cardSpawnRoot;
    [SerializeField] private GameObject dragAnchor;
    [SerializeField] private Hand hand;
    [SerializeField] private PlayArea playArea;
    [SerializeField] private DiscardPile discardPile;
    [SerializeField] private EndTurnButton endTurnButton;
    [Header("Optional")] [SerializeField] private ParticleSystem particle;

    private CardsDatabase _cardDatabase;
    private GameSettings _gameSettings;

    private readonly DeckManagerAnimator _animations = new();
    private readonly List<CardModel> _deck = new();

    private void Start()
    {
        _cardDatabase = Game.Instance.CardDatabase;
        _gameSettings = Game.Instance.GameSettings;
        endTurnButton.Button.onClick.AddListener(EndTurnSequence);
        InitializeDeck();
        ShuffleDeck();
        DrawStartingHand();
    }

    private void InitializeDeck()
    {
        var variousCardsCount = _cardDatabase.cardModels.Count;
        for (int i = 0; i < _gameSettings.initialDeckSize; i++)
        {
            _deck.Add(_cardDatabase.cardModels[i % variousCardsCount]);
        }
    }

    private void ShuffleDeck()
    {
        Random rng = new Random();
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
        List<Card> newCards = new();
        for (int i = 0; i < _gameSettings.playerHandSize; i++)
        {
            var drawnCard = TryDrawCardToHand();
            if (drawnCard != null)
            {
                newCards.Add(drawnCard);
            }
        }

        if (newCards.Count == 0)
        {
            Debug.LogError("No cards to start drawing! Specify correct amount in settings");
            return;
        }
        _animations.DrawCardsSequence(newCards, cardSpawnRoot.transform, () => EnableInput(false),
            () => EnableInput(true)).Forget();
    }

    private void OnPlayCard(Card c)
    {
        c.OnPlayCard -= OnPlayCard;
        hand.RemoveCardFromHand(c);
        playArea.AddCardToPlayArea(c);
        _animations.PlayEffectsOnCardPlayed(particle);
    }

    private void OnSwapCardPlaces(Card draggedCard, int draggedCardIdx, Card stationaryCard, int stationaryCardIdx)
    {
        hand.ReorderCard(draggedCard, draggedCardIdx, stationaryCardIdx);
        hand.ReorderCard(stationaryCard, stationaryCardIdx, draggedCardIdx);
    }

    private Card TryDrawCardToHand()
    {
        Card card = null;
        if (_deck.Count > 0)
        {
            CardModel cardData = _deck[0];
            _deck.RemoveAt(0);
            GameObject cardObject = Instantiate(cardPrefab, hand.transform);
            card = cardObject.GetComponent<Card>();
            card.Setup(cardData, dragAnchor);
            card.OnPlayCard += OnPlayCard;
            card.OnSwapCardPlaces += OnSwapCardPlaces;
            hand.AddCardToHand(card);
        }
        return card;
    }

    private void EndTurnSequence()
    {
        EndTurn().Forget();
    }
    
    private async UniTask EndTurn()
    {
        EnableInput(false);

        await TryMoveCardsFromPlayAreaToDiscardPile();

        int cardsToDraw = _gameSettings.playerHandSize - hand.GetCardCount();
        Debug.Log($"New cards to draw amt: {cardsToDraw}");
        List<Card> newCards = new();
        for (int i = 0; i < cardsToDraw; i++)
        {
            Card c = TryDrawCardToHand();
            if (c != null)
            {
                newCards.Add(c);
            }
        }

        if (newCards.Count > 0)
        {
            Debug.Log($"New cards drawn: {newCards.Count}");
            await _animations.DrawCardsSequence(newCards, cardSpawnRoot.transform, null, null);
        }
        if(_deck.Count == 0)
        {
            Debug.Log("No more cards to draw!");
            SetDrawDeckVisible(false);
        }

        EnableInput(true);
    }

    private async UniTask TryMoveCardsFromPlayAreaToDiscardPile()
    {
        if (playArea.CardsInPlayArea.Count > 0)
        {
            //detach graphics from discarded cards
            playArea.CardsInPlayArea.ForEach(c => c.GraphicsRoot.transform.SetParent(dragAnchor.transform, true));
            //move Cards roots from play to discard area
            discardPile.AddCardToPile(playArea.CardsInPlayArea);
            playArea.RemoveAllCards();
            //play move detached Card graphics to discard pile
            await _animations.DiscardCardsSequence(discardPile.DiscardedCards, dragAnchor.transform,null,null);
            discardPile.DeactivateCardsOnPile();
        }
    }

    private void EnableInput(bool value)
    {
        hand.Cards.ForEach(c => c.SetDraggable(value));
        endTurnButton.Button.interactable = value;
    }

    private void SetDrawDeckVisible(bool visible)
    {
        cardSpawnRoot.gameObject.SetActive(visible);
    }

    private void OnDestroy()
    {
        endTurnButton.Button.onClick.RemoveListener(EndTurnSequence);
    }
}