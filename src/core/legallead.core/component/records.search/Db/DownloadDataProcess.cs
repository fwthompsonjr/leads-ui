﻿namespace legallead.records.search.Db
{
    public class DownloadDataProcess : BaseDataProcess
    {
        protected override HccProcess Execute(IProgress<HccProcess> progress, HccProcess process)
        {
            List<BaseAction> actions = GetEnumerableOfType<BaseAction>("header", process).ToList();
            foreach (BaseAction? item in actions)
            {
                if (actions.IndexOf(item) > 0)
                {
                    item.WebDriver = actions[0].WebDriver;
                }
                item.Execute(progress);
            }
            return process;
        }
    }
}