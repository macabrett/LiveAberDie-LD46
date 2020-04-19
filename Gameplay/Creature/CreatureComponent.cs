namespace Macabre2D.Project.Gameplay.Creature {

    using Macabre2D.Framework;
    using Macabre2D.Project.Gameplay.Food;
    using Microsoft.Xna.Framework;
    using System.Runtime.Serialization;

    public sealed class CreatureComponent : BaseComponent, IBoundable, IUpdateableComponent, IResetable {
        private const float MoveSpeed = 0.1f;

        [DataMember(Name = "Chomp Animation", Order = 2)]
        private SpriteAnimation _chompAnimation;

        [DataMember(Name = "Dead Animation", Order = 3)]
        private SpriteAnimation _deadAnimation;

        [DataMember(Name = "Flip Horizontal", Order = 0)]
        private bool _flipHorizontal;

        [DataMember(Name = "Idle Animation", Order = 1)]
        private SpriteAnimation _idleAnimation;

        private SpriteAnimationComponent _spriteAnimationComponent;
        private Vector2 _startingPosition;
        private double _timeToRise = 0f;

        public BoundingArea BoundingArea {
            get {
                var worldTransform = this.WorldTransform;
                return new BoundingArea(worldTransform.Position + new Vector2(0f, 1f), worldTransform.Position + new Vector2(1f, 2f));
            }
        }

        public bool IsDead { get; set; }

        public void EatMe(FoodComponent food) {
            if (!this.IsDead) {
                this._timeToRise += 3f;
                food.LocalPosition = new Vector2(-10f, -10f); // this kills it
                this._spriteAnimationComponent.Play(this._chompAnimation, false);
                this._spriteAnimationComponent.Enqueue(this._idleAnimation, true);
            }
        }

        public void Reset() {
            this.IsDead = false;
            this.SetWorldPosition(this._startingPosition);

            if (this._spriteAnimationComponent != null && this._idleAnimation != null) {
                this._spriteAnimationComponent.Play(this._idleAnimation, true);
                this._spriteAnimationComponent.RenderSettings.FlipHorizontal = this._flipHorizontal;
            }
        }

        public void Update(FrameTime frameTime) {
            if (!this.IsDead) {
                if (this._timeToRise <= 0f) {
                    this._timeToRise = 0f;
                    this.SetWorldPosition(this.WorldTransform.Position - new Vector2(0f, (float)frameTime.SecondsPassed * MoveSpeed));
                    // If reaches the bottom of the screen die
                }
                else {
                    this._timeToRise -= frameTime.SecondsPassed;
                    this.SetWorldPosition(this.WorldTransform.Position + new Vector2(0f, (float)frameTime.SecondsPassed * MoveSpeed));
                }
            }
        }

        protected override void Initialize() {
            this._startingPosition = this.LocalPosition;
            this._spriteAnimationComponent = this.GetChild<SpriteAnimationComponent>();
            this.DrawOrder = 11;
            this.Reset();
        }
    }
}