using Macabre2D.Framework;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Macabre2D.Project.Gameplay {

    public enum StartMenuOptions {
        Play,
        Exit
    }

    public sealed class StartMenuComponent : BaseComponent, IUpdateableComponent {
        private TextRenderComponent _exitTextRenderer;
        private GameStateComponent _gameStateComponent;
        private InputManager _inputManager;
        private TextRenderComponent _playTextRenderer;
        private StartMenuOptions _selectedOption = StartMenuOptions.Play;
        private SpriteRenderComponent _selectionRenderer;

        public void Update(FrameTime frameTime) {
            if (this._inputManager.IsJumpPressed) {
                if (this._selectedOption == StartMenuOptions.Play) {
                    this._gameStateComponent.State = GameState.Playing;
                }
                else {
                    MacabreGame.Instance.Exit();
                }
            }
            else if (this._inputManager.IsUpOrDownPressed) {
                this.SwapSelectedOption();
            }
        }

        protected override void Initialize() {
            this._playTextRenderer = this.FindComponentInChildren("PlayText") as TextRenderComponent;
            this._exitTextRenderer = this.FindComponentInChildren("ExitText") as TextRenderComponent;
            this._selectionRenderer = this.GetChild<SpriteRenderComponent>();
            this._gameStateComponent = this.Scene.GetAllComponentsOfType<GameStateComponent>().First();
            this._inputManager = this.Scene.GetModule<InputManager>();
        }

        private void SwapSelectedOption() {
            if (this._selectedOption == StartMenuOptions.Exit) {
                this._selectedOption = StartMenuOptions.Play;
                this._selectionRenderer.LocalPosition = new Vector2(this._selectionRenderer.LocalPosition.X, this._playTextRenderer.LocalPosition.Y);
            }
            else {
                this._selectedOption = StartMenuOptions.Exit;
                this._selectionRenderer.LocalPosition = new Vector2(this._selectionRenderer.LocalPosition.X, this._exitTextRenderer.LocalPosition.Y);
            }
        }
    }
}