using Godot;
using System.Linq;
using System.Text;

namespace LunarLander
{
    public class Results : Control
    {
        private TextEdit TextEdit;

        public override void _Ready()
        {
            TextEdit = GetNode<TextEdit>("Panel/TextEdit");
            GetNode<Button>("PlayAgainButton").Connect("pressed", this, "OnPlayAgainPressed");
        }

        public void SetText(GameLogic logic)
        {
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < logic.History.Length; i++)
            {
                stringBuilder
                    .Append('[')
                    .Append(i)
                    .Append("s] ")
                    .Append(logic.History[i])
                    .AppendLine("L fuel burnt");
            }

            stringBuilder.AppendLine("-----");

            if (logic.Velocity <= 10)
            {
                stringBuilder
                    .Append("Landed safely at ")
                    .Append(logic.Velocity)
                    .AppendLine("m/s");
            }
            else
            {
                stringBuilder
                    .Append("Blasted a ")
                    .Append((int)(logic.Velocity / 10))
                    .AppendLine("m deep crater!");
            }

            stringBuilder
                .Append(logic.FuelRemaining)
                .AppendLine("L extra fuel");

            TextEdit.Text = stringBuilder.ToString();
        }

        public void OnPlayAgainPressed() => GetTree().ReloadCurrentScene();
    }
}