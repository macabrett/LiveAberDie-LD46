namespace Macabre2D.Project.Gameplay.Player.Stances {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;

    public sealed class JumpingStance : BaseStance {
        private const float JumpHoldLength = 0.175f;
        private float _elapsedSeconds;

        public JumpingStance(PlayerComponent playerComponent) : base(playerComponent) {
        }

        public override PlayerStance Stance {
            get {
                return PlayerStance.Jumping;
            }
        }

        public override void EnterStance(FrameTime gameTime) {
            this._elapsedSeconds = 0f;
        }

        public override void Update(FrameTime frameTime, InputManager inputManager) {
            var verticalVelocity = PlayerMovementValues.JumpVelocity;
            this._elapsedSeconds += (float)frameTime.SecondsPassed;
            if (this.CheckIfHitCeiling(frameTime, verticalVelocity)) {
                verticalVelocity = 0f;
                this.PlayerComponent.ChangeStance(PlayerStance.Falling, frameTime);
            }
            else if (!inputManager.IsJumpHeld || this._elapsedSeconds > JumpHoldLength) {
                this.PlayerComponent.ChangeStance(PlayerStance.Falling, frameTime);
            }

            var horitonzalVelocity = this.CalculateHorizontalVelocity(frameTime, inputManager);
            this.PlayerComponent.Velocity = new Vector2(horitonzalVelocity, verticalVelocity);
        }
    }
}