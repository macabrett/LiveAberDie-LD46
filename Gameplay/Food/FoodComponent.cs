namespace Macabre2D.Project.Gameplay.Food {

    using Macabre2D.Framework;
    using Macabre2D.Project.Gameplay.Creature;
    using Macabre2D.Project.Gameplay.Player;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class FoodComponent : BaseComponent, IBoundable, IUpdateableComponent {
        public const float FoodMaxFallSpeed = 5f;
        public const float FoodMinFallSpeed = 2f;
        private readonly List<CreatureComponent> _creatures = new List<CreatureComponent>();
        private readonly Random _random = new Random();
        private SpriteAnimation _animation;
        private PlayerComponent _player;
        private SpriteAnimationComponent _spriteAnimator;
        private Vector2 _velocity;

        public event EventHandler Destroyed;

        public SpriteAnimation Animation {
            get {
                return this._animation;
            }

            set {
                this._animation = value;

                if (this.IsInitialized) {
                    if (this._animation == null) {
                        this._spriteAnimator.Stop(true);
                    }
                    else {
                        this._spriteAnimator.Play(this._animation, true);
                    }
                }
            }
        }

        public BoundingArea BoundingArea {
            get {
                return this._spriteAnimator.BoundingArea;
            }
        }

        public bool HasBeenHit { get; set; }

        public Vector2 Velocity {
            get {
                return this._velocity;
            }

            set {
                this._velocity = new Vector2(
                    MathHelper.Clamp(value.X, -PlayerMovementValues.MaximumHorizontalVelocity, PlayerMovementValues.MaximumHorizontalVelocity),
                    MathHelper.Clamp(value.Y, -0.5f * PlayerMovementValues.TerminalVelocity, PlayerMovementValues.TerminalVelocity));

                if (this._spriteAnimator != null) {
                    if (this._velocity.X < 0f) {
                        this._spriteAnimator.RenderSettings.FlipHorizontal = true;
                    }
                    else if (this._velocity.X > 0f) {
                        this._spriteAnimator.RenderSettings.FlipHorizontal = false;
                    }
                    else {
                        this._spriteAnimator.RenderSettings.FlipHorizontal = this._random.Next(2) == 1;
                    }
                }
            }
        }

        public void Update(FrameTime frameTime) {
            if (this.IsOutOfBounds()) {
                this.Destroyed.SafeInvoke(this);
            }
            else if (this.HasBeenHit) {
                this.Velocity = new Vector2(this.Velocity.X, this.Velocity.Y - (0.5f * PlayerMovementValues.Gravity) * (float)frameTime.SecondsPassed);
            }

            foreach (var creature in this._creatures) {
                if (this.BoundingArea.Overlaps(creature.BoundingArea)) {
                    creature.EatMe(this);
                    break;
                }
            }

            if (this.BoundingArea.Overlaps(this._player.BoundingArea)) {
                var playerSpeed = this._player.Velocity.Length();
                var mySpeed = this.Velocity.Length();
                var vectorBetween = (this.WorldTransform.Position - this._player.WorldTransform.Position).GetNormalized();
                var direction = this._player.Velocity != Vector2.Zero ?
                    0.5f * (vectorBetween + this._player.Velocity.GetNormalized()) :
                    vectorBetween;
                this.Velocity = direction * (playerSpeed + mySpeed);
                this.HasBeenHit = true;
            }

            this.SetWorldPosition(this.WorldTransform.Position + (this.Velocity * (float)frameTime.SecondsPassed));
        }

        protected override void Initialize() {
            this.IsEnabledChanged += this.FoodComponent_IsEnabledChanged;
            this._player = this.Scene.GetAllComponentsOfType<PlayerComponent>().First();
            this._spriteAnimator = this.GetChild<SpriteAnimationComponent>();
            this._creatures.AddRange(this.Scene.GetAllComponentsOfType<CreatureComponent>());

            if (this._spriteAnimator == null) {
                this._spriteAnimator = this.AddChild<SpriteAnimationComponent>();
            }

            this._spriteAnimator.SnapToPixels = true;
            this._spriteAnimator.FrameRate = 16;

            if (this._animation != null) {
                this._spriteAnimator.Play(this._animation, true);
            }
        }

        private void FoodComponent_IsEnabledChanged(object sender, EventArgs e) {
            if (this.IsInitialized && this._spriteAnimator != null) {
                if (this.IsEnabled && this._animation != null) {
                    this._spriteAnimator.Play(this._animation, true);
                }
                else {
                    this._spriteAnimator.Stop(true);
                }
            }
        }

        private bool IsOutOfBounds() {
            var worldTransform = this.WorldTransform;
            return worldTransform.Position.Y < -1f || Math.Abs(worldTransform.Position.X) > 10f;
        }
    }
}