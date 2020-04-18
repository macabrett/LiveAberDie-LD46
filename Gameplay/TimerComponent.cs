namespace Macabre2D.Project.Gameplay {

    using Macabre2D.Framework;
    using System.Linq;

    public sealed class TimerComponent : BaseComponent, IUpdateableComponent {
        private const uint MaxTime = 999999;
        private const ushort NumberOfDigits = 6;

        private TextRenderComponent _textRenderer;

        private double _timePassed;

        private uint _totalTimePassed;

        public bool IsPaused { get; set; }

        public uint TotalTimePassed {
            get {
                return this._totalTimePassed;
            }

            set {
                if (value != this._totalTimePassed) {
                    this._totalTimePassed = value;
                    if (this._textRenderer != null) {
                        var newTimeDisplay = this._totalTimePassed.ToString();
                        newTimeDisplay = string.Concat(Enumerable.Repeat("0", NumberOfDigits - newTimeDisplay.Length)) + newTimeDisplay;
                        this._textRenderer.Text = newTimeDisplay;
                    }
                }
            }
        }

        public void Update(FrameTime frameTime) {
            if (!this.IsPaused && this._totalTimePassed < MaxTime && this._textRenderer != null) {
                this._timePassed += frameTime.SecondsPassed;

                if (this._timePassed >= 1d) {
                    var newTotalTime = this._totalTimePassed;
                    while (this._timePassed >= 1d && this._totalTimePassed < MaxTime) {
                        this._timePassed -= 1d;
                        newTotalTime += 1;
                    }

                    this.TotalTimePassed = newTotalTime;
                }
            }
        }

        protected override void Initialize() {
            this._textRenderer = this.GetChild<TextRenderComponent>();
        }
    }
}