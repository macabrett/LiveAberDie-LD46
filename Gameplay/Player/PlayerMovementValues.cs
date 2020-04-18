namespace Macabre2D.Project.Gameplay.Player {

    public static class PlayerMovementValues {
        public const float DecelerationRate = 25f;
        public const float Gravity = 39f;
        public const float JumpVelocity = 8f;
        public const float MaximumHorizontalVelocity = 7f;
        public const float MinimumVelocity = 2f;
        public const float PlayerHalfWidth = 0.5f * PlayerWidth;
        public const float PlayerWidth = 1f;
        public const float TerminalVelocity = 15f;
        private const float Acceleration = 6f;
        private const float AirAccelerationMultiplier = 0.9f;

        public static float GetHorizontalAcceleration(PlayerState state) {
            var acceleration = Acceleration;

            if (state.Stance != PlayerStance.Walking) {
                acceleration *= AirAccelerationMultiplier;
            }

            return acceleration;
        }
    }
}