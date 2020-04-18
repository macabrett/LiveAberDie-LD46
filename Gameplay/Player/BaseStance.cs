namespace Macabre2D.Project.Gameplay.Player {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System;

    public abstract class BaseStance {
        protected const float AnchorValue = PlayerMovementValues.PlayerHalfWidth - Constants.PixelSize;
        protected static readonly Vector2 _horizontalNormal = new Vector2(1f, 0f);
        protected static readonly Vector2 _verticalNormal = new Vector2(0f, 1f);

        protected BaseStance(PlayerComponent playerComponent) {
            this.PlayerComponent = playerComponent;
        }

        public abstract PlayerStance Stance { get; }

        protected PlayerComponent PlayerComponent { get; }

        public virtual void EnterStance(FrameTime frameTime) {
            return;
        }

        public virtual void ExitStance(FrameTime frameTime) {
            return;
        }

        public abstract void Update(FrameTime frameTime, KeyboardState keyboardState);

        // TODO: need to make this generic for gamepads/keyboards/user settings
        protected float CalculateHorizontalVelocity(FrameTime frameTime, KeyboardState keyboardState) {
            var horizontalVelocity = this.PlayerComponent.Velocity.X;
            var movingDirection = HorizontalDirection.Neutral;

            if (keyboardState.IsKeyDown(Keys.D)) {
                var newVelocity = horizontalVelocity + PlayerMovementValues.GetHorizontalAcceleration(this.PlayerComponent.State) * (float)frameTime.SecondsPassed;
                if (this.PlayerComponent.Velocity.X >= 0f) {
                    horizontalVelocity = Math.Max(newVelocity, PlayerMovementValues.MaximumHorizontalVelocity * 0.5f);
                }
                else {
                    horizontalVelocity = newVelocity;
                }

                movingDirection = HorizontalDirection.Right;
            }
            else if (horizontalVelocity > 0f) {
                horizontalVelocity -= PlayerMovementValues.DecelerationRate * (float)frameTime.SecondsPassed;

                if (horizontalVelocity < PlayerMovementValues.MinimumVelocity) {
                    horizontalVelocity = 0f;
                }
            }

            if (keyboardState.IsKeyDown(Keys.A)) {
                var newVelocity = horizontalVelocity - PlayerMovementValues.GetHorizontalAcceleration(this.PlayerComponent.State) * (float)frameTime.SecondsPassed;
                if (this.PlayerComponent.Velocity.X <= 0f) {
                    horizontalVelocity = Math.Min(newVelocity, -PlayerMovementValues.MaximumHorizontalVelocity * 0.5f);
                }
                else {
                    horizontalVelocity = newVelocity;
                }

                movingDirection = movingDirection == HorizontalDirection.Neutral ? HorizontalDirection.Left : HorizontalDirection.Neutral;
            }
            else if (horizontalVelocity < 0f) {
                horizontalVelocity += PlayerMovementValues.DecelerationRate * (float)frameTime.SecondsPassed;

                if (horizontalVelocity > -PlayerMovementValues.MinimumVelocity) {
                    horizontalVelocity = 0f;
                }
            }

            if (horizontalVelocity != 0f) {
                if (this.CheckIfHitWall(frameTime, horizontalVelocity, true)) {
                    horizontalVelocity = 0f;
                }

                this.CheckIfHitWall(frameTime, -horizontalVelocity, false);
            }
            else {
                this.CheckIfHitWall(frameTime, 0f, false);
            }

            this.PlayerComponent.MovingDirection = movingDirection;

            return horizontalVelocity;
        }

        protected bool CheckIfHitCeiling(FrameTime frameTime, float verticalVelocity) {
            var worldTransform = this.PlayerComponent.WorldTransform;
            var direction = new Vector2(0f, 1f);
            var distance = PlayerMovementValues.PlayerHalfWidth + (float)Math.Abs(verticalVelocity * frameTime.SecondsPassed);

            var result = this.PlayerComponent.TryRaycast(
                direction,
                distance,
                this.PlayerComponent.CeilingLayer,
                out var hit,
                new Vector2(-AnchorValue, 0f),
                new Vector2(AnchorValue, 0f));

            if (result && hit != null) {
                this.PlayerComponent.SetWorldPosition(new Vector2(worldTransform.Position.X, hit.ContactPoint.Y - PlayerMovementValues.PlayerHalfWidth));
            }

            return result;
        }

        protected bool CheckIfHitWall(FrameTime frameTime, float horizontalVelocity, bool applyVelocityToRaycast) {
            return horizontalVelocity != 0f ?
                this.RaycastWall(frameTime, horizontalVelocity, applyVelocityToRaycast) :
                this.RaycastWall(frameTime, -1f, false) || this.RaycastWall(frameTime, 1f, false);
        }

        private bool RaycastWall(FrameTime frameTime, float horizontalVelocity, bool applyVelocityToRaycast) {
            var worldTransform = this.PlayerComponent.WorldTransform;
            var isDirectionPositive = horizontalVelocity >= 0f;
            var direction = new Vector2(isDirectionPositive ? 1f : -1f, 0f);
            var distance = applyVelocityToRaycast ? PlayerMovementValues.PlayerHalfWidth + (float)Math.Abs(horizontalVelocity * frameTime.SecondsPassed) : PlayerMovementValues.PlayerHalfWidth;

            var result = this.PlayerComponent.TryRaycast(
                direction,
                distance,
                this.PlayerComponent.WallLayer,
                out var hit,
                new Vector2(0f, -AnchorValue),
                new Vector2(0f, AnchorValue));

            if (result && hit != null) {
                this.PlayerComponent.SetWorldPosition(new Vector2(hit.ContactPoint.X + (isDirectionPositive ? -PlayerMovementValues.PlayerHalfWidth : PlayerMovementValues.PlayerHalfWidth), worldTransform.Position.Y));
            }

            return result;
        }
    }
}