namespace Macabre2D.Project.Gameplay.Player {

    using Microsoft.Xna.Framework;

    public struct PlayerState {
        public readonly Vector2 Position;

        public readonly PlayerStance Stance;

        public readonly Vector2 Velocity;

        public PlayerState(PlayerStance stance, Vector2 position, Vector2 velocity) {
            this.Stance = stance;
            this.Position = position;
            this.Velocity = velocity;
        }

        public static bool operator !=(PlayerState left, PlayerState right) {
            return !(left == right);
        }

        public static bool operator ==(PlayerState left, PlayerState right) {
            return left.Equals(right);
        }

        public override bool Equals(object obj) {
            var result = false;
            if (obj is PlayerState playerState) {
                result = playerState.Stance == this.Stance && playerState.Position == this.Position && playerState.Velocity == this.Velocity;
            }

            return result;
        }

        public override int GetHashCode() {
            var hash = 13;
            hash = (hash * 7) + this.Position.GetHashCode();
            hash = (hash * 7) + this.Stance.GetHashCode();
            hash = (hash * 7) + this.Velocity.GetHashCode();
            return hash;
        }
    }
}