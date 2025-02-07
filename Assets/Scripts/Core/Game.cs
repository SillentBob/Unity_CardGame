using Const;
using Mercop.Core;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class Game : Singleton<Game>
    {
        [SerializeField] private CardsDatabase cardDatabase;
        [SerializeField] private GameSettings gameSettings;

        public CardsDatabase CardDatabase => cardDatabase;
        public GameSettings GameSettings => gameSettings;
        
        private Button _exitButton;

        private void Start()
        {
            _exitButton = GameObject.FindGameObjectWithTag(GameObjectTags.EXIT_GAMEPLAY_BUTTON).GetComponent<Button>();
            _exitButton.onClick.AddListener(Application.Quit);
        }

        private void OnDestroy()
        {
            _exitButton.onClick.RemoveListener(Application.Quit);
        }
    }
}