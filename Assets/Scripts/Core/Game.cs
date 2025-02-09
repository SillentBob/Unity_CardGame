using System;
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
        public RenderMode RenderMode => _renderMode;
        private RenderMode _renderMode;
        private void Start()
        {
            var topCanvas = FindObjectOfType<Canvas>();
            if (topCanvas != null)
            {
                _renderMode =  topCanvas.renderMode;
            }
        }

        public void Quit()
        {
#if UNITY_EDITOR
            Debug.LogError("AppQuit!");
#endif
            Application.Quit();

        }
    }
}