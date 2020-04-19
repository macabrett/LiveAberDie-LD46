namespace Macabre2D.Project.Gameplay.Player {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    public sealed class PlayerComponent : BaseComponent, IBoundable, IUpdateableComponent {
        private readonly BaseStance[] _stances;
        private PlayerAnimation _currentAnimation = PlayerAnimation.None;
        private BaseStance _currentStance;
        private InputManager _inputManager;
        private HorizontalDirection _movingDirection = HorizontalDirection.Neutral;
        private SimplePhysicsModule _physicsModule;
        private SpriteAnimationComponent _spriteAnimator;
        private Vector2 _velocity;

        public PlayerComponent() : base() {
            this._stances = this.GetAllStances();
            this._currentStance = this._stances[(int)PlayerStance.Walking];
        }

        public BoundingArea BoundingArea {
            get {
                return this._spriteAnimator.BoundingArea;
            }
        }

        [DataMember(Name = "Ceiling Layer", Order = 6)]
        public Layers CeilingLayer { get; private set; } = Layers.Layer03;

        [DataMember(Order = 2)]
        public SpriteAnimation FallAnimation { get; private set; }

        [DataMember(Order = 5)]
        public Layers GroundLayer { get; private set; } = Layers.Layer01;

        [DataMember(Order = 0)]
        public SpriteAnimation IdleAnimation { get; private set; }

        [DataMember(Order = 3)]
        public SpriteAnimation JumpAnimation { get; private set; }

        public HorizontalDirection MovingDirection {
            get {
                return this._movingDirection;
            }

            set {
                if (value != this._movingDirection) {
                    this._movingDirection = value;

                    if (this._spriteAnimator != null) {
                        if (this._movingDirection == HorizontalDirection.Left) {
                            this._spriteAnimator.RenderSettings.FlipHorizontal = true;
                        }
                        else if (this._movingDirection == HorizontalDirection.Right) {
                            this._spriteAnimator.RenderSettings.FlipHorizontal = false;
                        }
                    }
                }
            }
        }

        [DataMember(Order = 1)]
        public SpriteAnimation RunAnimation { get; private set; }

        [DataMember(Order = 4)]
        public SpriteAnimation SlideAnimation { get; private set; }

        public PlayerState State { get; private set; }

        public Vector2 Velocity {
            get {
                return this._velocity;
            }

            set {
                this._velocity = new Vector2(
                    MathHelper.Clamp(value.X, -PlayerMovementValues.MaximumHorizontalVelocity, PlayerMovementValues.MaximumHorizontalVelocity),
                    MathHelper.Clamp(value.Y, -PlayerMovementValues.TerminalVelocity, PlayerMovementValues.TerminalVelocity));
            }
        }

        [DataMember(Name = "Wall Layer", Order = 6)]
        public Layers WallLayer { get; private set; } = Layers.Layer02;

        public void Update(FrameTime frameTime) {
            this._currentStance.Update(frameTime, this._inputManager);
            this.State = new PlayerState(this._currentStance.Stance, this.WorldTransform.Position, this.Velocity);
            this.ChangeAnimation();
            this.LocalPosition += this.Velocity * (float)frameTime.SecondsPassed;
        }

        internal void ChangeStance(PlayerStance newStance, FrameTime frameTime) {
            this._currentStance.ExitStance(frameTime);
            this._currentStance = this._stances[(int)newStance];
            this._currentStance.EnterStance(frameTime);
        }

        internal bool TryRaycast(Vector2 direction, float distance, Layers layers, out RaycastHit hit, params Vector2[] anchors) {
            var result = false;
            hit = null;
            var worldTransform = this.WorldTransform;
            var counter = 0;

            while (!result && counter < anchors.Length) {
                var anchor = anchors[counter];
                result = this._physicsModule.TryRaycast(new Vector2(worldTransform.Position.X + anchor.X, worldTransform.Position.Y + anchor.Y), direction, distance, layers, out hit);
                counter++;
            }

            return result;
        }

        protected override void Initialize() {
            this.State = new PlayerState(this._currentStance.Stance, this.WorldTransform.Position, Vector2.Zero);
            this._physicsModule = this.Scene.GetModule<SimplePhysicsModule>();
            this._inputManager = this.Scene.GetModule<InputManager>();
            this._spriteAnimator = this.GetChild<SpriteAnimationComponent>();

            if (this._physicsModule == null && this.Scene.AddModule(new SimplePhysicsModule())) {
                this._physicsModule = this.Scene.GetModule<SimplePhysicsModule>();
            }
        }

        private void ChangeAnimation() {
            PlayerAnimation animation;
            if (this._currentStance.Stance == PlayerStance.Jumping) {
                animation = PlayerAnimation.Jumping;
            }
            else if (this._currentStance.Stance == PlayerStance.Falling) {
                animation = PlayerAnimation.Falling;
            }
            else if (this.Velocity.X == 0f || this.MovingDirection == HorizontalDirection.Neutral) {
                animation = PlayerAnimation.Idle;
            }
            else if (this.IsSliding()) {
                animation = PlayerAnimation.Sliding;
            }
            else {
                animation = PlayerAnimation.Running;
            }

            if (animation != this._currentAnimation) {
                switch (animation) {
                    case PlayerAnimation.None:
                    case PlayerAnimation.Idle:
                        this._spriteAnimator.Play(this.IdleAnimation, true);
                        break;

                    case PlayerAnimation.Running:
                        this._spriteAnimator.Play(this.RunAnimation, true);
                        break;

                    case PlayerAnimation.Falling:
                        this._spriteAnimator.Play(this.FallAnimation, true);
                        break;

                    case PlayerAnimation.Jumping:
                        this._spriteAnimator.Play(this.JumpAnimation, false);
                        break;

                    case PlayerAnimation.Sliding:
                        this._spriteAnimator.Play(this.SlideAnimation, true);
                        break;
                }

                this._currentAnimation = animation;
            }
        }

        private BaseStance[] GetAllStances() {
            var baseStanceType = typeof(BaseStance);
            var types = Assembly.GetAssembly(baseStanceType).GetTypes().Where(x => x.IsClass && !x.IsAbstract && x.IsSealed && x.IsSubclassOf(baseStanceType));
            var stances = new BaseStance[Enum.GetNames(typeof(PlayerStance)).Length];

            foreach (var type in types) {
                var stance = Activator.CreateInstance(type, this) as BaseStance;
                stances[(int)stance.Stance] = stance;
            }

            return stances;
        }

        private bool IsSliding() {
            return (this.MovingDirection == HorizontalDirection.Left && this.Velocity.X > 0f) ||
                (this.MovingDirection == HorizontalDirection.Right && this.Velocity.X < 0f);
        }
    }
}