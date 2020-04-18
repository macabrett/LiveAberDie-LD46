namespace Macabre2D.Project.Gameplay.Player.Stances {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System;

    public sealed class FallingStance : BaseStance {

        public FallingStance(PlayerComponent playerComponent) : base(playerComponent) {
        }

        public override PlayerStance Stance {
            get {
                return PlayerStance.Falling;
            }
        }

        public override void Update(FrameTime frameTime, KeyboardState keyboardState) {
            var verticalVelocity = this.PlayerComponent.Velocity.Y;
            if (verticalVelocity < 0f && this.CheckIfHitGround(frameTime, verticalVelocity, out var hit)) {
                this.PlayerComponent.SetWorldPosition(new Vector2(this.PlayerComponent.WorldTransform.Position.X, hit.ContactPoint.Y + PlayerMovementValues.PlayerHalfWidth));
                this.PlayerComponent.ChangeStance(PlayerStance.Walking, frameTime);
                verticalVelocity = 0f;
            }
            else {
                if (verticalVelocity > 0f && this.CheckIfHitCeiling(frameTime, verticalVelocity)) {
                    verticalVelocity = 0f;
                }

                verticalVelocity -= PlayerMovementValues.Gravity * (float)frameTime.SecondsPassed;
            }

            var horitonzalVelocity = this.CalculateHorizontalVelocity(frameTime, keyboardState);
            this.PlayerComponent.Velocity = new Vector2(horitonzalVelocity, verticalVelocity);
        }

        private bool CheckIfHitGround(FrameTime frameTime, float verticalVelocity, out RaycastHit hit) {
            var direction = new Vector2(0f, -1f);
            var distance = PlayerMovementValues.PlayerHalfWidth + (float)Math.Abs(verticalVelocity * frameTime.SecondsPassed);

            var result = this.PlayerComponent.TryRaycast(
                direction,
                distance,
                this.PlayerComponent.GroundLayer,
                out hit,
                new Vector2(-AnchorValue, 0f),
                new Vector2(AnchorValue, 0f));

            return result && hit != null;
        }
    }
}