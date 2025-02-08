using Mercop.Core;
using UnityEngine;

namespace DefaultNamespace
{
    public class Game : Singleton<Game>
    {
        [Header("Mandatory")]
        [SerializeField] private CardsDatabase cardDatabase;
        [SerializeField] private GameSettings gameSettings;

        public CardsDatabase CardDatabase => cardDatabase;
        public GameSettings GameSettings => gameSettings;

        public void Quit()
        {
#if UNITY_EDITOR
            Debug.LogError("AppQuit!");
#endif
            Application.Quit();

        }
    }
}