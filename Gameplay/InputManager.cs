namespace Macabre2D.Project.Gameplay {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public interface IInputManager {
        float HorizontalAxis { get; }

        bool IsJumpPressed { get; }
    }

    public sealed class InputManager : BaseUpdateableModule, IInputManager {
        private const float DeadZone = 0.25f;

        private GamePadState _currentGamePadState;
        private KeyboardState _currentKeyboardState;
        private GamePadState _previousGamePadState;
        private KeyboardState _previousKeyboardState;

        public float HorizontalAxis { get; private set; }

        public bool IsJumpHeld { get; private set; }
        public bool IsJumpPressed { get; private set; }

        public override void PostUpdate(FrameTime frameTime) {
            return;
        }

        public override void PreUpdate(FrameTime frameTime) {
            this._previousKeyboardState = this._currentKeyboardState;
            this._currentKeyboardState = Keyboard.GetState();

            this._previousGamePadState = this._currentGamePadState;
            this._currentGamePadState = GamePad.GetState(PlayerIndex.One);

            this.IsJumpHeld = this._currentKeyboardState.IsKeyDown(Keys.Space) || (this._currentGamePadState.IsConnected && this._currentGamePadState.IsButtonDown(Buttons.A));
            this.IsJumpPressed = this.GetIsJumpPressed();
            this.HorizontalAxis = this.GetHorizontalAxis();
        }

        private float GetHorizontalAxis() {
            var result = 0f;
            if (this._currentKeyboardState.IsKeyDown(Keys.A) || (this._currentGamePadState.IsConnected && (this._currentGamePadState.DPad.Left == ButtonState.Pressed || this._currentGamePadState.ThumbSticks.Left.X < -DeadZone))) {
                result = -1f;
            }

            if (this._currentKeyboardState.IsKeyDown(Keys.D) || (this._currentGamePadState.IsConnected && (this._currentGamePadState.DPad.Right == ButtonState.Pressed || this._currentGamePadState.ThumbSticks.Left.X > DeadZone))) {
                result = 1f;
            }

            return result;
        }

        private bool GetIsJumpPressed() {
            var result = false;
            if (!this._previousKeyboardState.IsKeyDown(Keys.Space) && !this._previousGamePadState.IsButtonDown(Buttons.A)) {
                result = this.IsJumpHeld;
            }

            return result;
        }
    }
}