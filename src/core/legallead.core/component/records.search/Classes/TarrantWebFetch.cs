using legallead.records.search.Dto;
using legallead.records.search.Models;

namespace legallead.records.search.Classes
{
    public partial class TarrantWebInteractive
    {
        private interface ITarrantWebFetch
        {
            string Name { get; }
            TarrantWebInteractive Web { get; }

            void Fetch(DateTime startingDate, out WebFetchResult webFetch, out List<PersonAddress> people, int? caseOverrideId = null);
        }

        private class NonCriminalFetch : ITarrantWebFetch
        {
            public NonCriminalFetch(TarrantWebInteractive tarrantWeb)
            {
                Web = tarrantWeb;
            }

            protected static class FetchType
            {
                public const int NonCriminal = 0;
                public const int Criminal = 2;
            }

            protected const StringComparison Ccic = StringComparison.CurrentCultureIgnoreCase;
            public virtual string Name => "NonCriminal";

            public virtual TarrantWebInteractive Web { get; }

            public virtual void Fetch(DateTime startingDate, out WebFetchResult webFetch, out List<PersonAddress> people, int? caseOverrideId = null)
            {
                List<NavigationStep> steps = new();
                string navigationFile = Web.GetParameterValue<string>(CommonKeyIndexes.NavigationControlFile); // "navigation.control.file");
                List<string> sources = navigationFile.Split(',').ToList();
                caseOverrideId ??= TarrantComboBxValue.CourtMap.First(x => x.Name.Equals("Justice of Peace", Ccic)).Id;
                sources.ForEach(s => steps.AddRange(GetAppSteps(s).Steps));
                SetupParameters(steps, caseOverrideId, out people, out XmlContentHolder results, out List<HLinkDataRow> cases);
                webFetch = Web.SearchWeb(results, steps, startingDate, startingDate, ref cases, out people);
            }

            protected void SetupParameters(List<NavigationStep> steps,
                int? caseTypeOverrideId,
                out List<PersonAddress> people,
                out XmlContentHolder results,
                out List<HLinkDataRow> cases)
            {
                results = new SettingsManager().GetOutput(Web);
                cases = new List<HLinkDataRow>();
                people = new List<PersonAddress>();

                int caseTypeId = caseTypeOverrideId ?? Web.GetParameterValue<int>(CommonKeyIndexes.CaseTypeSelectedIndex); // "caseTypeSelectedIndex");
                // set special item values
                NavigationStep caseTypeSelect = steps.First(x => x.ActionName.Equals(CommonKeyIndexes.SetSelectValue, StringComparison.CurrentCultureIgnoreCase));
                caseTypeSelect.ExpectedValue = caseTypeId.ToString(CultureInfo.CurrentCulture);
            }
        }

        private class NonCrimalFetchProbateCourt : NonCriminalFetch
        {
            public NonCrimalFetchProbateCourt(TarrantWebInteractive tarrantWeb) : base(tarrantWeb)
            {
            }

            public override string Name => "NonCriminalProbateCount";

            public override void Fetch(DateTime startingDate, out WebFetchResult webFetch, out List<PersonAddress> people, int? caseOverrideId = null)
            {
                int overrideId = TarrantComboBxValue.CourtMap.First(x => x.Name.Equals("Probate", Ccic)).Id;
                base.Fetch(startingDate, out webFetch, out people, overrideId);
            }
        }

        private class NonCrimalFetchCclCourt : NonCriminalFetch
        {
            public NonCrimalFetchCclCourt(TarrantWebInteractive tarrantWeb) : base(tarrantWeb)
            {
            }

            public override string Name => "NonCriminalCclCount";

            public override void Fetch(DateTime startingDate, out WebFetchResult webFetch, out List<PersonAddress> people, int? caseOverrideId = null)
            {
                int overrideId = TarrantComboBxValue.CourtMap.First(x => x.Name.Equals("Court Court at Law", Ccic)).Id;
                base.Fetch(startingDate, out webFetch, out people, overrideId);
            }
        }

        private class CriminalFetch : NonCriminalFetch
        {
            public CriminalFetch(TarrantWebInteractive tarrantWeb)
                : base(tarrantWeb)
            {
            }

            public override string Name => "Criminal";

            public override void Fetch(DateTime startingDate, out WebFetchResult webFetch, out List<PersonAddress> people, int? caseOverrideId = null)
            {
                List<NavigationStep> steps = new();
                string navigationFile = Web.GetParameterValue<string>("navigation.control.alternate.file");
                List<string> sources = navigationFile.Split(',').ToList();
                sources.ForEach(s => steps.AddRange(GetAppSteps(s).Steps));
                SetupParameters(steps, null, out people, out XmlContentHolder results, out List<HLinkDataRow> cases);
                webFetch = Web.SearchWeb(FetchType.Criminal, results, steps, startingDate, startingDate, ref cases, out people);
            }
        }

        private class FetchProvider
        {
            public TarrantWebInteractive Web { get; }

            public FetchProvider(TarrantWebInteractive tarrantWeb)
            {
                Web = tarrantWeb;
            }

            public List<ITarrantWebFetch> GetFetches(int searchMode = 2)
            {
                const string criminal = "criminal";
                const StringComparison ccic = StringComparison.CurrentCultureIgnoreCase;
                List<ITarrantWebFetch> fetchers = new()
                {
                    new NonCriminalFetch(Web),
                    new NonCrimalFetchProbateCourt(Web),
                    new NonCrimalFetchCclCourt(Web),
                    new CriminalFetch(Web)
                };
                switch (searchMode)
                {
                    case 0:
                        fetchers = fetchers.FindAll(x =>
                        {
                            string lowered = x.Name.ToLower(CultureInfo.CurrentCulture);
                            return lowered.StartsWith(criminal, ccic);
                        });
                        break;

                    case 2:
                        fetchers = fetchers.FindAll(x =>
                        {
                            string lowered = x.Name.ToLower(CultureInfo.CurrentCulture);
                            return !lowered.StartsWith(criminal, ccic);
                        });
                        break;

                    default:
                        break;
                }

                return fetchers;
            }
        }
    }
}