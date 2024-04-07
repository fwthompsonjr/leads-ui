namespace legallead.installer.Commands
{
    public partial class CommandHandler
    {
        [Command("locals", "Display all local installed applications")]
        public void Locals()
        {
            Console.WriteLine("Listing installed versions for legallead applications.");
            var models = _applocator.GetInstalledApplications();
            if (models.Count == 0)
            {
                Console.WriteLine(" - No items found.");
                return;
            }
            foreach (var model in models)
            {
                Console.WriteLine(" - {0}", model.Name);
                if (model.Versions.Count == 0) continue;
                var details = model.Versions.Select(x =>
                {
                    return $"     -- {x.Version} {x.PublishDate:D}";
                });
                Console.WriteLine(string.Join(Environment.NewLine, details));
            }
        }

    }
}
