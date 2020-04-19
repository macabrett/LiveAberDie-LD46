namespace Macabre2D.Project.Gameplay.Creature {

    using Macabre2D.Framework;
    using Macabre2D.Project.Gameplay.Food;
    using Microsoft.Xna.Framework;
    using System;
    using System.Linq;
    using System.Runtime.Serialization;

    public sealed class CreatureComponent : BaseComponent, IBoundable, IUpdateableComponent, IResetable {
        private const float MaxFallSpeed = 0.4f;
        private const float MaxRiseSpeed = 2f;
        private const float MinFallSpeed = 0.2f;
        private const float MinRiseSpeed = 1f;

        [DataMember(Name = "Chomp Animation", Order = 2)]
        private SpriteAnimation _chompAnimation;

        private float _currentFallSpeed = MinFallSpeed;
        private float _currentRiseSpeed = MaxRiseSpeed;
        private uint _currentTimeThreshold;

        [DataMember(Name = "Dead Animation", Order = 3)]
        private SpriteAnimation _deadAnimation;

        [DataMember(Name = "Flip Horizontal", Order = 0)]
        private bool _flipHorizontal;

        [DataMember(Name = "Idle Animation", Order = 1)]
        private SpriteAnimation _idleAnimation;

        private SpriteAnimationComponent _spriteAnimationComponent;
        private Vector2 _startingPosition;
        private TimerComponent _timer;
        private double _timeToRise = 0f;

        public event EventHandler HasDied;

        public BoundingArea BoundingArea {
            get {
                var worldTransform = this.WorldTransform;
                return new BoundingArea(worldTransform.Position + new Vector2(0f, 1f), worldTransform.Position + new Vector2(1f, 2f));
            }
        }

        public bool IsDead { get; set; }

        public void EatMe(FoodComponent food) {
            if (!this.IsDead) {
                this._timeToRise += 1f;
                food.LocalPosition = new Vector2(-10f, -10f); // this kills it
                this._spriteAnimationComponent.Play(this._chompAnimation, false);
                this._spriteAnimationComponent.Enqueue(this._idleAnimation, true);
            }
        }

        public void Reset() {
            this.IsDead = false;
            this._currentFallSpeed = MinFallSpeed;
            this._currentRiseSpeed = MaxRiseSpeed;
            this._currentTimeThreshold = 0;
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

                    var newWorldPosition = this.WorldTransform.Position - new Vector2(0f, (float)frameTime.SecondsPassed * this._currentFallSpeed);
                    if (newWorldPosition.Y <= 0.5f) {
                        newWorldPosition = new Vector2(newWorldPosition.X, 0.5f);
                        this._spriteAnimationComponent.Play(this._deadAnimation, true);
                        this.IsDead = true;
                        this.HasDied.SafeInvoke(this);
                    }

                    this.SetWorldPosition(newWorldPosition);
                }
                else {
                    this._timeToRise -= frameTime.SecondsPassed;

                    var newWorldPosition = this.WorldTransform.Position + new Vector2(0f, (float)frameTime.SecondsPassed * this._currentRiseSpeed);
                    if (newWorldPosition.Y > this._startingPosition.Y) {
                        newWorldPosition = this._startingPosition;
                    }

                    this.SetWorldPosition(newWorldPosition);
                }

                var nextThreshold = this._currentTimeThreshold + 50;
                if (this._timer.TotalTimePassed >= nextThreshold) {
                    this._currentTimeThreshold += 50;
                    this._currentFallSpeed = Math.Min(MaxFallSpeed, this._currentFallSpeed + 0.02f);
                    this._currentRiseSpeed = Math.Max(MinRiseSpeed, this._currentRiseSpeed - 0.1f);
                }
            }
        }

        protected override void Initialize() {
            this._startingPosition = this.LocalPosition;
            this._spriteAnimationComponent = this.GetChild<SpriteAnimationComponent>();
            this._timer = this.Scene.GetAllComponentsOfType<TimerComponent>().First();
            this.DrawOrder = 11;
            this.Reset();
        }
    }
}