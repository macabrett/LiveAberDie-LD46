namespace Macabre2D.Project.Gameplay.Player.Stances {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class IdleStance : BaseStance {

        public IdleStance(PlayerComponent playerComponent) : base(playerComponent) {
        }

        public override PlayerStance Stance {
            get {
                return PlayerStance.Idle;
            }
        }

        public override void EnterStance(FrameTime frameTime) {
            this.PlayerComponent.Velocity = Vector2.Zero;
        }

        public override void Update(FrameTime frameTime, KeyboardState keyboardState) {
            var horizontalVelocity = this.CalculateHorizontalVelocity(frameTime, keyboardState);

            if (horizontalVelocity != 0f) {
                this.PlayerComponent.ChangeStance(PlayerStance.Walking, frameTime);
            }
        }
    }
}