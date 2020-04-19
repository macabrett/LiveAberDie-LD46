namespace Macabre2D.Project.Gameplay.Food {

    using Macabre2D.Framework;
    using Macabre2D.Project.Gameplay.Extensions;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    public sealed class FoodSpawner : BaseComponent, IResetable, IUpdateableComponent {
        private const float FoodMinFallSpeed = 3f;
        private const int SpawnWidth = 12;
        private readonly List<FoodComponent> _allFoods = new List<FoodComponent>();
        private readonly Queue<FoodComponent> _foodQueue = new Queue<FoodComponent>();
        private readonly Random _random = new Random();

        [DataMember(Name = "Bread Animation", Order = 4)]
        private SpriteAnimation _breadAnimation;

        private Camera _camera;

        [DataMember(Name = "Crunch Noise", Order = 9)]
        private AudioClip _crunchNoise;

        [DataMember(Name = "Donut Animation", Order = 5)]
        private SpriteAnimation _donutAnimation;

        [DataMember(Name = "Drop Time", Order = 2)]
        private float _dropTime = 1.2f;

        [DataMember(Name = "Food Draw Order", Order = 1)]
        private int _foodDrawOrder = 1;

        [DataMember(Name = "Hit Noise", Order = 8)]
        private AudioClip _hitNoise;

        [DataMember(Name = "Melon Animation", Order = 6)]
        private SpriteAnimation _melonAnimation;

        [DataMember(Name = "Plum Animation", Order = 7)]
        private SpriteAnimation _plumAnimation;

        private double _timePassed;

        [DataMember(Name = "Use Collisions", Order = 3)]
        private bool _useCollisions = true;

        public void Reset() {
            this._timePassed = 0f;
            this._foodQueue.Clear();
            foreach (var food in this._allFoods.Shuffle()) {
                this.Food_Destroyed(food, null);
            }
        }

        public void Update(FrameTime frameTime) {
            this._timePassed += frameTime.SecondsPassed;

            var lastDrop = -10;
            while (this._timePassed > this._dropTime) {
                var currentDrop = this._random.Next(0, SpawnWidth * 2);

                while (currentDrop == lastDrop) {
                    currentDrop = this._random.Next(0, SpawnWidth * 2);
                }

                if (this._foodQueue.Any()) {
                    var food = this._foodQueue.Dequeue();
                    var xPosition = (currentDrop - SpawnWidth) * 0.5f;
                    var yPosition = this._camera.ViewHeight + 1f;
                    food.SetWorldPosition(new Vector2(xPosition, yPosition));
                    food.HasBeenHit = false;
                    food.Velocity = new Vector2(0f, -this.GetFallSpeed());
                    food.IsEnabled = true;
                    this.Scene.AddComponent(food);
                }

                this._timePassed -= this._dropTime;
            }
        }

        protected override void Initialize() {
            this._camera = this.Scene.GetAllComponentsOfType<Camera>().First();

            for (var i = 1; i <= 32; i++) {
                var food = new FoodComponent();
                if (i % 4 == 0) {
                    food.Animation = this._breadAnimation;
                }
                else if (i % 3 == 0) {
                    food.Animation = this._donutAnimation;
                }
                else if (i % 2 == 0) {
                    food.Animation = this._melonAnimation;
                }
                else {
                    food.Animation = this._plumAnimation;
                }

                food.DrawOrder = this._foodDrawOrder;
                food.WorryAboutCollisions = this._useCollisions;
                food.HitNoise = this._hitNoise;
                food.CrunchNoise = this._crunchNoise;
                food.Destroyed += this.Food_Destroyed;
                this._allFoods.Add(food);
            }

            foreach (var food in this._allFoods.Shuffle()) {
                this._foodQueue.Enqueue(food);
            }
        }

        private void Food_Destroyed(object sender, EventArgs e) {
            if (sender is FoodComponent component) {
                component.IsEnabled = false;
                component.HasBeenHit = false;
                this._foodQueue.Enqueue(component);
                this.Scene.RemoveComponent(component);
            }
        }

        private float GetFallSpeed() {
            return (2f * (float)this._random.NextDouble()) + FoodMinFallSpeed;
        }
    }
}