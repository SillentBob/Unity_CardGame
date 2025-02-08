using UnityEngine;

namespace Gameplay
{
    public class CardGraphicsRoot : MonoBehaviour
    {
        [SerializeField] private Card parent;

        public Card Parent => parent;
    }
}