﻿// DownloadCaseProcess

namespace legallead.records.search.Db
{
    public class DownloadCaseProcess : BaseDataProcess
    {
        protected override HccProcess Execute(IProgress<HccProcess> progress, HccProcess process)
        {
            var actions = GetEnumerableOfType<BaseAction>("detail", process).ToList();
            foreach (var item in actions)
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