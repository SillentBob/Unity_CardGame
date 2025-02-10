using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Pool;

namespace Gameplay
{
    public class CardsPool : MonoBehaviour
    {
        [SerializeField] private Card cardPrefab;
        [SerializeField] private Transform cardSpawnParent;
        
        private int _maxPoolSize;
        private int _startCapacity;
        private readonly bool _collectionChecks = true;
        private IObjectPool<Card> cardPool;

        private void Awake()
        {
            _startCapacity = Game.Instance.GameSettings.playerHandSize * 2;
            _maxPoolSize = _startCapacity;
        }

        public IObjectPool<Card> Pool
        {
            get
            {
                if (cardPool == null)
                {
                    cardPool = new ObjectPool<Card>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, _collectionChecks, _startCapacity, _maxPoolSize);
                }
                return cardPool;
            }
        }

        private Card CreatePooledItem()
        {
            Card cardObject = Instantiate(cardPrefab, cardSpawnParent);
            return cardObject;
        }

        void OnReturnedToPool(Card c)
        {
            c.gameObject.SetActive(false);
        }

        void OnTakeFromPool(Card c)
        {
            c.gameObject.SetActive(true);
        }

        void OnDestroyPoolObject(Card c)
        {
            Destroy(c.gameObject);
        }
    }
}