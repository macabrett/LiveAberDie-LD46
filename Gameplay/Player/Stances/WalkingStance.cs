namespace Macabre2D.Project.Gameplay.Player.Stances {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class WalkingStance : GroundedStance {

        public WalkingStance(PlayerComponent playerComponent) : base(playerComponent) {
        }

        public override PlayerStance Stance {
            get {
                return PlayerStance.Walking;
            }
        }

        public override void Update(FrameTime frameTime, KeyboardState keyboardState) {
            var horizontalVelocity = this.CalculateHorizontalVelocity(frameTime, keyboardState);
            var verticalVelocity = 0f;

            if (!this.CheckIfStillGrounded(out _)) {
                verticalVelocity = -PlayerMovementValues.Gravity * (float)frameTime.SecondsPassed;
                this.PlayerComponent.ChangeStance(PlayerStance.Falling, frameTime);
            }
            else if (keyboardState.IsKeyDown(Keys.Space)) {
                verticalVelocity = PlayerMovementValues.JumpVelocity;
                this.PlayerComponent.ChangeStance(PlayerStance.Jumping, frameTime);
            }

            this.PlayerComponent.Velocity = new Vector2(horizontalVelocity, verticalVelocity);
        }
    }
}