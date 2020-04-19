namespace Macabre2D.Project.Gameplay {

    using Macabre2D.Framework;
    using System.Runtime.Serialization;

    // There is a weird alpha transparency thing occurring in my sprite font due to some sort of
    // compression during MonoGame's spritefont compilation, so this is a hacky fix
    public sealed class PixelTextRendererComponent : TextRenderComponent {

        [DataMember]
        private int _numberOfRenderPasses = 3;

        public override void Draw(FrameTime frameTime, BoundingArea viewBoundingArea) {
            for (var i = 0; i < this._numberOfRenderPasses; i++) {
                base.Draw(frameTime, viewBoundingArea);
            }
        }
    }
}