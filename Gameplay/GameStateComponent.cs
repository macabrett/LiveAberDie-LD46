namespace Macabre2D.Project.Gameplay {

    using Macabre2D.Framework;
    using Macabre2D.Project.Gameplay.Creature;
    using Macabre2D.Project.Gameplay.Player;
    using System.Collections.Generic;
    using System.Linq;

    public enum GameState {
        TitleScreen,
        Playing,
        DeathScreen,
        SecretScreen,
    }

    public sealed class GameStateComponent : BaseComponent, IUpdateableComponent {
        private readonly HashSet<CreatureComponent> _creatures = new HashSet<CreatureComponent>();
        private readonly HashSet<IResetable> _resetables = new HashSet<IResetable>();
        private BaseComponent _deathScreen;
        private BaseComponent _gameScreen;
        private InputManager _inputManager;
        private PlayerComponent _player;
        private BaseComponent _secretScreen;
        private GameState _state = GameState.TitleScreen;
        private BaseComponent _titleScreen;

        public GameState State {
            get {
                return this._state;
            }

            set {
                this._state = value;

                if (this._state == GameState.TitleScreen) {
                    this._titleScreen.IsEnabled = true;
                    this._gameScreen.IsEnabled = false;
                    this._deathScreen.IsEnabled = false;
                    this._secretScreen.IsEnabled = false;
                }
                else if (this._state == GameState.DeathScreen) {
                    this._deathScreen.IsEnabled = true;
                    this._titleScreen.IsEnabled = false;
                    this._gameScreen.IsEnabled = false;
                    this._secretScreen.IsEnabled = false;
                }
                else if (this._state == GameState.Playing) {
                    this._gameScreen.IsEnabled = true;
                    this._titleScreen.IsEnabled = false;
                    this._deathScreen.IsEnabled = false;
                    this._secretScreen.IsEnabled = false;
                }
                else if (this._state == GameState.SecretScreen) {
                    this._secretScreen.IsEnabled = true;
                    this._gameScreen.IsEnabled = false;
                    this._titleScreen.IsEnabled = false;
                    this._deathScreen.IsEnabled = false;
                }

                this.Reset();
            }
        }

        public void Update(FrameTime frameTime) {
            if (this.State == GameState.Playing) {
                if (this._player.IsDead || this._creatures.All(x => x.IsDead)) {
                    this.State = GameState.DeathScreen;
                }
            }
            else if (this.State == GameState.DeathScreen || this.State == GameState.SecretScreen) {
                if (this._inputManager.IsJumpPressed) {
                    this.State = GameState.Playing;
                }
            }
            else if (this.State == GameState.TitleScreen) {
                // TODO: have a menu
                if (this._inputManager.IsJumpPressed) {
                    this.State = GameState.Playing;
                }
            }
        }

        protected override void Initialize() {
            this._creatures.AddRange(this.Scene.GetAllComponentsOfType<CreatureComponent>());
            this._resetables.AddRange(this.Scene.GetAllComponentsOfType<IResetable>());
            this._player = this.Scene.GetAllComponentsOfType<PlayerComponent>().First();
            this._inputManager = this.Scene.GetModule<InputManager>();
            this._gameScreen = this.Scene.FindComponent("GameScreen");
            this._deathScreen = this.Scene.FindComponent("DeathScreen");
            this._titleScreen = this.Scene.FindComponent("TitleScreen");
            this._secretScreen = this.Scene.FindComponent("SecretScreen");

            this.State = GameState.TitleScreen;
        }

        private void Reset() {
            foreach (var resetable in this._resetables) {
                resetable.Reset();
            }
        }
    }
}