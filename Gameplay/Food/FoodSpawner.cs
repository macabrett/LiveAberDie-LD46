namespace Macabre2D.Project.Gameplay.Food {

    using Macabre2D.Framework;
    using Macabre2D.Project.Gameplay.Extensions;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    public sealed class FoodSpawner : BaseComponent, IUpdateableComponent {
        private const float FoodMinFallSpeed = 3f;
        private const float SecondsBetweenFruitDrops = 1.2f;
        private const int SpawnWidth = 12;
        private readonly Queue<FoodComponent> _foodQueue = new Queue<FoodComponent>();
        private readonly Random _random = new Random();

        [DataMember(Name = "Bread Animation", Order = 1)]
        private SpriteAnimation _breadAnimation;

        private Camera _camera;

        [DataMember(Name = "Donut Animation", Order = 2)]
        private SpriteAnimation _donutAnimation;

        [DataMember(Name = "Melon Animation", Order = 3)]
        private SpriteAnimation _melonAnimation;

        [DataMember(Name = "Plum Animation", Order = 4)]
        private SpriteAnimation _plumAnimation;

        private double _timePassed;

        public void Update(FrameTime frameTime) {
            this._timePassed += frameTime.SecondsPassed;

            var lastDrop = -10;
            while (this._timePassed > SecondsBetweenFruitDrops) {
                var currentDrop = this._random.Next(0, SpawnWidth * 2);

                while (currentDrop == lastDrop) {
                    currentDrop = this._random.Next(0, SpawnWidth * 2);
                }

                var food = this._foodQueue.Dequeue();
                var xPosition = (currentDrop - SpawnWidth) * 0.5f;
                var yPosition = this._camera.ViewHeight + 1f;
                food.SetWorldPosition(new Vector2(xPosition, yPosition));
                food.HasBeenHit = false;
                food.Velocity = new Vector2(0f, -this.GetFallSpeed());
                food.IsEnabled = true;
                this.Scene.AddComponent(food);

                this._timePassed -= SecondsBetweenFruitDrops;
            }
        }

        protected override void Initialize() {
            this._camera = this.Scene.GetAllComponentsOfType<Camera>().First();

            var foodList = new List<FoodComponent>();
            for (var i = 1; i <= 8; i++) {
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

                food.DrawOrder = 1;
                food.Destroyed += this.Food_Destroyed;
                foodList.Add(food);
            }

            foreach (var food in foodList.Shuffle()) {
                this._foodQueue.Enqueue(food);
            }
        }

        private void Food_Destroyed(object sender, EventArgs e) {
            if (sender is FoodComponent component) {
                component.IsEnabled = false;
                this._foodQueue.Enqueue(component);
                this.Scene.RemoveComponent(component);
            }
        }

        private float GetFallSpeed() {
            return (2f * (float)this._random.NextDouble()) + FoodMinFallSpeed;
        }
    }
}