using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace legallead.installer.Commands
{
    public partial class CommandHandler
    {
        [Command("list", "Display release details for legallead applications")]
        [ExcludeFromCodeCoverage(Justification = "Method interacts with 3rd party resources")]
        public async Task List()
        {
            var data = await GetAvailables();
            Console.WriteLine(data);
        }

        [ExcludeFromCodeCoverage(Justification = "Method interacts with 3rd party resources")]
        public async Task<string> GetAvailables()
        {
            var builder = new StringBuilder("Listing available versions for legallead applications.");
            builder.AppendLine();
            var models = await _reader.GetReleases();
            if (models == null || models.Count == 0)
            {
                builder.AppendLine(" - No items found.");
                return builder.ToString();
            }
            foreach (var model in models)
            {
                builder.AppendLine($" - {model.Name}: {model.PublishDate:D}");
                if (model.Assets.Count == 0) continue;
                var details = model.Assets.Select(x =>
                {
                    return $"     -- {x.AssetId}: {x.Name} {x.Version}";
                });
                builder.AppendLine(string.Join(Environment.NewLine, details));
            }
            return builder.ToString();
        }

    }
}
