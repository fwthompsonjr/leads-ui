using System.Diagnostics.CodeAnalysis;

namespace legallead.installer.Commands
{
    public partial class CommandHandler
    {
        [Command("list", "Display release details for legallead applications")]
        [ExcludeFromCodeCoverage(Justification = "Method interacts with 3rd party resources")]
        public async Task List()
        {
            Console.WriteLine("Listing available versions for legallead applications.");
            var models = await _reader.GetReleases();
            if (models == null || models.Count == 0)
            {
                Console.WriteLine(" - No items found.");
                return;
            }
            foreach (var model in models)
            {
                Console.WriteLine(" - {0}: {1:D}", model.Name, model.PublishDate);
                if (model.Assets.Count == 0) continue;
                var details = model.Assets.Select(x =>
                {
                    return $"     -- {x.AssetId}: {x.Name} {x.Version}";
                });
                Console.WriteLine(string.Join(Environment.NewLine, details));
            }
        }

    }
}
