namespace Macabre2D.Project.Gameplay.Player.Stances {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;

    public abstract class GroundedStance : BaseStance {

        protected GroundedStance(PlayerComponent playerComponent) : base(playerComponent) {
        }

        protected bool CheckIfStillGrounded(out RaycastHit hit) {
            var direction = new Vector2(0f, -1f);

            var result = this.PlayerComponent.TryRaycast(
                direction,
                PlayerMovementValues.PlayerHalfWidth,
                this.PlayerComponent.GroundLayer,
                out hit,
                new Vector2(-AnchorValue, -Constants.HalfPixelSize),
                new Vector2(AnchorValue, -Constants.HalfPixelSize)) && hit != null;

            if (result) {
                this.PlayerComponent.SetWorldPosition(new Vector2(this.PlayerComponent.WorldTransform.Position.X, hit.ContactPoint.Y + PlayerMovementValues.PlayerHalfWidth));
            }

            return result;
        }
    }
}