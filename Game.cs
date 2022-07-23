using Godot;
using System;

namespace LunarLander
{
    public class Game : Node
    {
        private const float TOTAL_MOVING_TIME = 0.8f;

        private readonly GameLogic Logic = new GameLogic();

        private ParallaxBackground StarsParallax;

        private Sprite Background;

        private AnimationPlayer RocketAnimationPlayer;

        private AnimationPlayer HudAnimationPlayer;

        private ProgressBar AltitudeProgressBar;

        private Label AltitudeLabel;

        private Label PendingFuelLabel;

        private bool isUserHoldingDown;

        private float pendingFuelToBurn;

        private float movingTime;

        private GameState State = GameState.Pending;

        private enum GameState
        {
            Pending,
            Moving,
            Over
        }

        public override void _Ready()
        {
            SetPhysicsProcess(false);

            StarsParallax = GetNode<ParallaxBackground>("StarsParallax");
            Background = GetNode<Sprite>("BackgroundLayer/Background");
            RocketAnimationPlayer = GetNode<AnimationPlayer>("Rocket/AnimationPlayer");
            HudAnimationPlayer = GetNode<AnimationPlayer>("HUD/AnimationPlayer");
            AltitudeProgressBar = GetNode<ProgressBar>("HUD/Altitude/ProgressBar");
            AltitudeLabel = GetNode<Label>("HUD/Altitude/Label");
            PendingFuelLabel = GetNode<Label>("HUD/PendingFuel");

            AltitudeProgressBar.MaxValue = 1000;
            StarsParallax.ScrollOffset = new Vector2(0, Logic.Altitude);
        }

        public override void _Process(float delta)
        {
            UpdateHud();

            // TODO: only anim?
            if (Logic.Altitude <= 0)
                EndGame();

            if (isUserHoldingDown && pendingFuelToBurn < Logic.FuelRemaining)
            {
                // Don't start increasing pending fuel immediately, so
                // it's easier for the user to burn zero fuel.
                pendingFuelToBurn += pendingFuelToBurn > 0.03 ? 2 : 0.001f;
                PendingFuelLabel.Text = "-" + Math.Floor(pendingFuelToBurn);
                RocketAnimationPlayer.Play("holding");
                HudAnimationPlayer.Play("holding");
            }
            else if (pendingFuelToBurn > 0)
            {
                Logic.BurnFuel((int)pendingFuelToBurn);
                pendingFuelToBurn = 0;
                RocketAnimationPlayer.Play("boost");
                HudAnimationPlayer.Play("boost");

                movingTime = 0;
                SetPhysicsProcess(true);
                SetProcess(false);
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            UpdateHud();

            if (movingTime > TOTAL_MOVING_TIME)
            {
                StarsParallax.ScrollOffset = new Vector2(0, Logic.Altitude);
                SetPhysicsProcess(false);
                SetProcess(true);
                return;
            }

            movingTime += delta;
            StarsParallax.ScrollOffset -= new Vector2(0, Logic.Velocity * delta / TOTAL_MOVING_TIME);
        }

        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent.IsActionPressed("restart"))
                GetTree().ReloadCurrentScene();

            if (inputEvent is InputEventScreenTouch touchEvent)
                isUserHoldingDown = touchEvent.Pressed;
        }

        private void UpdateHud()
        {
            AltitudeProgressBar.Value = Logic.Altitude;
            AltitudeLabel.Text = $"{(int)Logic.Altitude}m";

            // TODO: should be removed or hidden
            GetNode<Label>("Info").Text = $"Velocity:{Logic.Velocity}\nFuelRemaining:{Logic.FuelRemaining}\nPendingFuelToBurn:{pendingFuelToBurn}\nStarsParallax.ScrollOffset:{StarsParallax.ScrollOffset}";
        }

        private void EndGame()
        {
        }
    }
}