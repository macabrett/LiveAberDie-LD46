namespace Macabre2D.Project.Gameplay {

    using Macabre2D.Framework;
    using Macabre2D.Project.Gameplay.Creature;
    using Macabre2D.Project.Gameplay.Player;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class GameStateComponent : BaseComponent, IUpdateableComponent {
        private readonly List<CreatureComponent> _creatures = new List<CreatureComponent>();
        private readonly List<IResetable> _resetables = new List<IResetable>();
        private PlayerComponent _player;

        public void Update(FrameTime frameTime) {
            if (this._creatures.All(x => x.IsDead)) {
                // end
            }
            else if (this._player.IsDead) {
            }
        }

        protected override void Initialize() {
            this._creatures.AddRange(this.Scene.GetAllComponentsOfType<CreatureComponent>());
            this._resetables.AddRange(this.Scene.GetAllComponentsOfType<IResetable>());
        }
    }
}