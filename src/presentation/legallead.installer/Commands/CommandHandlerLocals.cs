using System.Text;

namespace legallead.installer.Commands
{
    public partial class CommandHandler
    {
        [Command("locals", "Display all local installed applications")]
        public void Locals()
        {
            var data = GetLocals();
            Console.WriteLine(data);
        }

        public string GetLocals()
        {
            var builder = new StringBuilder("Listing installed versions for legallead applications.");
            builder.AppendLine();
            var models = _applocator.GetInstalledApplications();
            if (models.Count == 0)
            {
                builder.AppendLine(" - No items found.");
                return builder.ToString();
            }
            foreach (var model in models)
            {
                builder.AppendLine($" - {model.Name}");
                if (model.Versions.Count == 0) continue;
                var details = model.Versions.Select(x =>
                {
                    return $"     -- {x.Version} {x.PublishDate:D}";
                });
                builder.AppendLine(string.Join(Environment.NewLine, details));
            }
            return builder.ToString();
        }

    }
}
