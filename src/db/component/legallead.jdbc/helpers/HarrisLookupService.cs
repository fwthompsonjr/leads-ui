using Newtonsoft.Json;

namespace legallead.jdbc.helpers
{
    internal static class HarrisLookupService
    {
        public static string Translate(string keyName, string current)
        {
            const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            if (string.IsNullOrWhiteSpace(current)) return string.Empty;
            current = current.Trim();
            var item = Data.Find(x =>
            {
                return x.Key.Equals(keyName, comparison) && x.Code.Equals(current, comparison);
            });
            return item?.Value ?? current;
        }

        public static List<FieldLookupDto> Data
        {
            get
            {
                const char sq = (char)39;
                const char dq = '"';
                if (dataset.Count > 0) return dataset;
                var js = json.Replace(sq, dq);
                var tmp = JsonConvert.DeserializeObject<List<FieldLookupDto>>(js) ?? new();
                dataset.AddRange(tmp);
                return dataset;
            }
        }

        private static readonly List<FieldLookupDto> dataset = [];
        private static readonly string NL = Environment.NewLine;
        private static readonly string json = "[" + NL +
        "{ 'key': 'CDI', 'code': '2', 'value': 'misdemeanor' }," + NL +
        "{ 'key': 'CDI', 'code': '3', 'value': 'felony' }," + NL +
        "{ 'key': 'INS', 'code': 'ACP', 'value': 'ALIAS CAPIAS PROFINE' }," + NL +
        "{ 'key': 'INS', 'code': 'ACW', 'value': 'ALIAS CAPIAS WARRANT' }," + NL +
        "{ 'key': 'INS', 'code': 'AJW', 'value': 'ALIAS ARREST WARRANT' }," + NL +
        "{ 'key': 'INS', 'code': 'APP', 'value': 'ON APPEAL, COURT OF CRIMINAL APPEALS' }," + NL +
        "{ 'key': 'INS', 'code': 'BAP', 'value': 'BOND FORFEITURE APPEAL' }," + NL +
        "{ 'key': 'INS', 'code': 'BF7', 'value': 'SB7' }," + NL +
        "{ 'key': 'INS', 'code': 'BFA', 'value': 'BOND FORFEITURE CAPIAS' }," + NL +
        "{ 'key': 'INS', 'code': 'BFC', 'value': 'CASH BOND' }," + NL +
        "{ 'key': 'INS', 'code': 'BFD', 'value': 'PERSONAL BOND' }," + NL +
        "{ 'key': 'INS', 'code': 'BFE', 'value': 'EARLY PRESENTMENT' }," + NL +
        "{ 'key': 'INS', 'code': 'BFG', 'value': 'GENERAL ORDER BOND - PTRL' }," + NL +
        "{ 'key': 'INS', 'code': 'BFP', 'value': 'BOND FORFEITURE PERSONAL BOND' }," + NL +
        "{ 'key': 'INS', 'code': 'BFS', 'value': 'SURETY OR PROPERTY BOND' }," + NL +
        "{ 'key': 'INS', 'code': 'BFT', 'value': 'BOND FORFEITURE JUDGMENT' }," + NL +
        "{ 'key': 'INS', 'code': 'BFU', 'value': 'UNSECURED BAIL BOND' }," + NL +
        "{ 'key': 'INS', 'code': 'BFW', 'value': 'BOND FORFEITURE WARRANT' }," + NL +
        "{ 'key': 'INS', 'code': 'BOR', 'value': 'BILL OF REVIEW GRANTED' }," + NL +
        "{ 'key': 'INS', 'code': 'BRD', 'value': 'BILL OF REVIEW DENIED' }," + NL +
        "{ 'key': 'INS', 'code': 'BRF', 'value': 'BILL OF REVIEW FILED' }," + NL +
        "{ 'key': 'INS', 'code': 'BWT', 'value': 'BENCH WARRANT' }," + NL +
        "{ 'key': 'INS', 'code': 'CAP', 'value': 'CAPIAS CHILD SUPPORT' }," + NL +
        "{ 'key': 'INS', 'code': 'CAR', 'value': 'CITE AND RELEASE - MISDEMEANOR' }," + NL +
        "{ 'key': 'INS', 'code': 'CIT', 'value': 'CITATION ISSUED' }," + NL +
        "{ 'key': 'INS', 'code': 'CMI', 'value': 'COMMITTMENT & CAPIAS PROFINE' }," + NL +
        "{ 'key': 'INS', 'code': 'COM', 'value': 'COMPLAINT' }," + NL +
        "{ 'key': 'INS', 'code': 'CPF', 'value': 'CAPIAS PROFINE WARRANT' }," + NL +
        "{ 'key': 'INS', 'code': 'CTP', 'value': 'CONTEMPT OF COURT' }," + NL +
        "{ 'key': 'INS', 'code': 'CTR', 'value': 'CITATION RETURNED OR ANSWER FILED' }," + NL +
        "{ 'key': 'INS', 'code': 'CVI', 'value': 'CHANGE OF VENUE INDICTMENT IN' }," + NL +
        "{ 'key': 'INS', 'code': 'DET', 'value': 'NOTICE OF DETAINER' }," + NL +
        "{ 'key': 'INS', 'code': 'DWC', 'value': 'DISMISSED WITH COST' }," + NL +
        "{ 'key': 'INS', 'code': 'EAG', 'value': 'EXPUNCTION ACQUITTAL GRANTED' }," + NL +
        "{ 'key': 'INS', 'code': 'EAR', 'value': 'EXPUNCTION ACQUITTAL RECORD' }," + NL +
        "{ 'key': 'INS', 'code': 'EPG', 'value': 'EXPUNGED RECORD' }," + NL +
        "{ 'key': 'INS', 'code': 'EXA', 'value': 'EXPARTE ON APPEAL' }," + NL +
        "{ 'key': 'INS', 'code': 'EXE', 'value': 'EXECUTION ISSUED' }," + NL +
        "{ 'key': 'INS', 'code': 'EXP', 'value': 'EX PARTE PROCEEDINGS' }," + NL +
        "{ 'key': 'INS', 'code': 'EXR', 'value': 'EXECUTION RETURNED' }," + NL +
        "{ 'key': 'INS', 'code': 'FID', 'value': 'FELONY INDICTMENT' }," + NL +
        "{ 'key': 'INS', 'code': 'FIN', 'value': 'FELONY INFORMATION' }," + NL +
        "{ 'key': 'INS', 'code': 'FUG', 'value': 'FUGITIVE' }," + NL +
        "{ 'key': 'INS', 'code': 'JCW', 'value': 'JUSTICE COURT WARRANT' }," + NL +
        "{ 'key': 'INS', 'code': 'MAJ', 'value': 'MOTION TO ADJUDICATE GUILT' }," + NL +
        "{ 'key': 'INS', 'code': 'MAP', 'value': 'MISDEMEANOR APPEAL' }," + NL +
        "{ 'key': 'INS', 'code': 'MDN', 'value': 'MOTION NEW TRIAL DENIED/OVERRULED' }," + NL +
        "{ 'key': 'INS', 'code': 'MID', 'value': 'MISDEMEANOR INDICTMENT' }," + NL +
        "{ 'key': 'INS', 'code': 'MIN', 'value': 'MISDEMEANOR INFORMATION' }," + NL +
        "{ 'key': 'INS', 'code': 'MND', 'value': 'MOTION NEW TRIAL DENIED' }," + NL +
        "{ 'key': 'INS', 'code': 'MNF', 'value': 'MOTION NEW TRIAL FILED' }," + NL +
        "{ 'key': 'INS', 'code': 'MNT', 'value': 'MOTION NEW TRIAL GRANTED' }," + NL +
        "{ 'key': 'INS', 'code': 'MRC', 'value': 'MOTION TO REVOKE CONDITIONAL DISCHARGE' }," + NL +
        "{ 'key': 'INS', 'code': 'MRP', 'value': 'MOTION REVOKE PROBATION' }," + NL +
        "{ 'key': 'INS', 'code': 'NOB', 'value': 'NO BILL' }," + NL +
        "{ 'key': 'INS', 'code': 'OCF', 'value': 'OUT OF COUNTY FELONY' }," + NL +
        "{ 'key': 'INS', 'code': 'OCM', 'value': 'OUT OF COUNTY MISDEMEANOR' }," + NL +
        "{ 'key': 'INS', 'code': 'PCA', 'value': 'POSTCW - 11.072 (COMMUNITY SUPERVISION) APPEAL' }," + NL +
        "{ 'key': 'INS', 'code': 'PCC', 'value': 'POSTCW - 11.072 (COMMUNITY SUPERVISION)' }," + NL +
        "{ 'key': 'INS', 'code': 'PCW', 'value': 'POSTCW - 11.07 (NON-DEATH)/11.071 (DEATH)' }," + NL +
        "{ 'key': 'INS', 'code': 'PIN', 'value': 'OOC PRETRIAL INTERVENTION (CSCD)' }," + NL +
        "{ 'key': 'INS', 'code': 'PTI', 'value': 'PTS OOC SUPERVISION' }," + NL +
        "{ 'key': 'INS', 'code': 'PWA', 'value': 'POSTCW - 11.07 (NON-DEATH)/11.071 (DEATH) APPEAL' }," + NL +
        "{ 'key': 'INS', 'code': 'RD', 'value': 'REVERSED AND DISMISSED' }," + NL +
        "{ 'key': 'INS', 'code': 'RDL', 'value': 'RESTRICTED DRIVERS LICENSE' }," + NL +
        "{ 'key': 'INS', 'code': 'RID', 'value': 'REINDICTMENT' }," + NL +
        "{ 'key': 'INS', 'code': 'RR', 'value': 'REVERSED AND REMANDED' }," + NL +
        "{ 'key': 'INS', 'code': 'RWC', 'value': 'REINSTATED WITH COST' }," + NL +
        "{ 'key': 'INS', 'code': 'SF', 'value': 'SCIRE FACIAS (BOND FORFEITURE) - INITIAL' }," + NL +
        "{ 'key': 'INS', 'code': 'SVP', 'value': 'SEXUAL VIOLENT PREDATOR' }," + NL +
        "{ 'key': 'INS', 'code': 'TJC', 'value': 'TRANSFERRED TO JUVENILE COURT' }," + NL +
        "{ 'key': 'INS', 'code': 'TRF', 'value': 'TRF TRAFFIC WARRANT (PATROL DIV ONLY)' }," + NL +
        "{ 'key': 'INS', 'code': 'WAR', 'value': 'WARRANT' }," + NL +
        "{ 'key': 'INS', 'code': 'WCW', 'value': 'WORTHLESS CHECK WARRANT' }," + NL +
        "{ 'key': 'INS', 'code': 'WOG', 'value': 'WRIT OF GARNISHMENT' }," + NL +
        "{ 'key': 'INS', 'code': 'WRA', 'value': 'PRECW - 11.08 OR 11.09 (LIBERTY) APPEAL' }," + NL +
        "{ 'key': 'INS', 'code': 'WRT', 'value': 'PRECW - 11.08 OR 11.09 (LIBERTY)' }," + NL +
        "{ 'key': 'CAD', 'code': 'AFFM', 'value': 'AFFIRMATIVE FINDING' }," + NL +
        "{ 'key': 'CAD', 'code': 'APPL', 'value': 'APPEAL' }," + NL +
        "{ 'key': 'CAD', 'code': 'AUTO', 'value': 'AUTOMATIC DISPOSITION (SPECIAL)' }," + NL +
        "{ 'key': 'CAD', 'code': 'BFPD', 'value': 'BOND FORFEITURE PAID' }," + NL +
        "{ 'key': 'CAD', 'code': 'CABD', 'value': 'CASH BOND' }," + NL +
        "{ 'key': 'CAD', 'code': 'CABS', 'value': 'CABS - ABSCONDED' }," + NL +
        "{ 'key': 'CAD', 'code': 'CDTH', 'value': 'CDTH - DEATH OF PROBATIONER' }," + NL +
        "{ 'key': 'CAD', 'code': 'CETR', 'value': 'CETR - EARLY TERMINATION' }," + NL +
        "{ 'key': 'CAD', 'code': 'CEXP', 'value': 'CEXP - PROBATION EXPIRED' }," + NL +
        "{ 'key': 'CAD', 'code': 'CLAW', 'value': 'CLAW - LAW VIOLATION' }," + NL +
        "{ 'key': 'CAD', 'code': 'CMOV', 'value': 'CMOV - MOVED FROM HARRIS COUNTY' }," + NL +
        "{ 'key': 'CAD', 'code': 'COMM', 'value': 'DEF. COMMITTED TO JAIL FOR FINE & COSTS' }," + NL +
        "{ 'key': 'CAD', 'code': 'COND', 'value': 'CONDITIONAL DISCHARGE' }," + NL +
        "{ 'key': 'CAD', 'code': 'COTH', 'value': 'COTH - OTHER REASON' }," + NL +
        "{ 'key': 'CAD', 'code': 'CREJ', 'value': 'CREJ - SUPERVISION REJECTED' }," + NL +
        "{ 'key': 'CAD', 'code': 'CREQ', 'value': 'CREQ - REQUEST OF ORIGINAL JURISDICTION' }," + NL +
        "{ 'key': 'CAD', 'code': 'CRVK', 'value': 'CRVK - PROBATION REVOKED' }," + NL +
        "{ 'key': 'CAD', 'code': 'CTEC', 'value': 'CTEC - TECHNICAL VIOLATION' }," + NL +
        "{ 'key': 'CAD', 'code': 'DADJ', 'value': 'DEFERRED ADJUDICATION OF GUILT' }," + NL +
        "{ 'key': 'CAD', 'code': 'DEFD', 'value': 'DEFERRED DISPOSITION' }," + NL +
        "{ 'key': 'CAD', 'code': 'DERR', 'value': 'DISPOSITION TO FOLLOW' }," + NL +
        "{ 'key': 'CAD', 'code': 'DISM', 'value': 'DISMISSED' }," + NL +
        "{ 'key': 'CAD', 'code': 'DISP', 'value': 'DISPOSED' }," + NL +
        "{ 'key': 'CAD', 'code': 'DWOP', 'value': 'CIVIL DISMISSAL FOR WANT OF PROSECUTION' }," + NL +
        "{ 'key': 'CAD', 'code': 'EXPG', 'value': 'EXPUNGED RECORD' }," + NL +
        "{ 'key': 'CAD', 'code': 'FINE', 'value': 'FINED' }," + NL +
        "{ 'key': 'CAD', 'code': 'GILT', 'value': 'FOUND GUILTY' }," + NL +
        "{ 'key': 'CAD', 'code': 'GOBR', 'value': 'GENERAL ORDER BOND' }," + NL +
        "{ 'key': 'CAD', 'code': 'NEGF', 'value': 'NEGATIVE FINDING' }," + NL +
        "{ 'key': 'CAD', 'code': 'NEWT', 'value': 'NEW TRIAL' }," + NL +
        "{ 'key': 'CAD', 'code': 'NOB', 'value': 'NO BILLED' }," + NL +
        "{ 'key': 'CAD', 'code': 'NOTG', 'value': 'FOUND NOT GUILTY' }," + NL +
        "{ 'key': 'CAD', 'code': 'NPCF', 'value': 'NO PROBABLE CAUSE FOUND' }," + NL +
        "{ 'key': 'CAD', 'code': 'PART', 'value': 'PARTIAL PAYMENT' }," + NL +
        "{ 'key': 'CAD', 'code': 'PDFC', 'value': 'PAID FINE AND COSTS - JP COURTS' }," + NL +
        "{ 'key': 'CAD', 'code': 'PROB', 'value': 'PROBATION' }," + NL +
        "{ 'key': 'CAD', 'code': 'PTIN', 'value': 'PRETRIAL INTERVENTION CSCD' }," + NL +
        "{ 'key': 'CAD', 'code': 'REIN', 'value': 'BOND REINSTATED' }," + NL +
        "{ 'key': 'CAD', 'code': 'REVS', 'value': 'CASE REVERSED' }," + NL +
        "{ 'key': 'CAD', 'code': 'SENT', 'value': 'SENTENCED' }," + NL +
        "{ 'key': 'CAD', 'code': 'SUBD', 'value': 'SURETY BOND' }," + NL +
        "{ 'key': 'CAD', 'code': 'TEMP', 'value': 'TEMPORARY' }," + NL +
        "{ 'key': 'CAD', 'code': 'USTP', 'value': 'UNSATISFACTORY TERMINATION OF PROBATION' }," + NL +
        "{ 'key': 'CST', 'code': 'A', 'value': 'HAS OPEN SETTINGS' }," + NL +
        "{ 'key': 'CST', 'code': 'B', 'value': 'INACTIVE DUE TO BOND FORFEITURE' }," + NL +
        "{ 'key': 'CST', 'code': 'C', 'value': 'FINAL DISPOSITION, NO ACTIVITY EXPECTED' }," + NL +
        "{ 'key': 'CST', 'code': 'D', 'value': 'DISMISSED' }," + NL +
        "{ 'key': 'CST', 'code': 'E', 'value': 'NGRI COMMITTED' }," + NL +
        "{ 'key': 'CST', 'code': 'F', 'value': 'INACTIVE-PENDING FELONY DISPOSITION EXPECTED' }," + NL +
        "{ 'key': 'CST', 'code': 'G', 'value': 'PENDING GRAND JURY' }," + NL +
        "{ 'key': 'CST', 'code': 'I', 'value': 'HAS NO OPEN SETTINGS' }," + NL +
        "{ 'key': 'CST', 'code': 'J', 'value': 'INACTIVE - DEF. SENT TO TDCJ ON OTHER CASE' }," + NL +
        "{ 'key': 'CST', 'code': 'K', 'value': 'PTS OOC SUPERVISION' }," + NL +
        "{ 'key': 'CST', 'code': 'L', 'value': 'TRIAL DOCKET' }," + NL +
        "{ 'key': 'CST', 'code': 'M', 'value': 'PENDING REFILE' }," + NL +
        "{ 'key': 'CST', 'code': 'N', 'value': 'INACTIVE - BOND FORFEITURE ON OTHER CASE' }," + NL +
        "{ 'key': 'CST', 'code': 'O', 'value': 'OTHER CASE ON APPEAL' }," + NL +
        "{ 'key': 'CST', 'code': 'P', 'value': 'PENDING APPEAL' }," + NL +
        "{ 'key': 'CST', 'code': 'Q', 'value': 'PENDING COMPLETION COMMUNITY SERVICE' }," + NL +
        "{ 'key': 'CST', 'code': 'R', 'value': 'PENDING COMPLETION OF PROBATION' }," + NL +
        "{ 'key': 'CST', 'code': 'S', 'value': 'PENDING RESTORATION OF SANITY' }," + NL +
        "{ 'key': 'CST', 'code': 'T', 'value': 'TEMPORARY' }," + NL +
        "{ 'key': 'CST', 'code': 'V', 'value': 'EXECUTION RETURNED' }," + NL +
        "{ 'key': 'CST', 'code': 'W', 'value': 'WAITING EXECUTION' }," + NL +
        "{ 'key': 'CST', 'code': 'X', 'value': 'PENDING DUE COURSE' }," + NL +
        "{ 'key': 'CST', 'code': 'Z', 'value': 'INACTIVE-BOND FORFEITURE' }," + NL +
        "{ 'key': 'DST', 'code': 'A', 'value': 'DEFERRED ADJUDICATION OF GUILT' }," + NL +
        "{ 'key': 'DST', 'code': 'B', 'value': 'BOND MADE' }," + NL +
        "{ 'key': 'DST', 'code': 'C', 'value': 'CITY JAIL (FOR HOUSTON JAIL ONLY)' }," + NL +
        "{ 'key': 'DST', 'code': 'D', 'value': 'DISPOSED' }," + NL +
        "{ 'key': 'DST', 'code': 'E', 'value': 'EXECUTION SERVED' }," + NL +
        "{ 'key': 'DST', 'code': 'F', 'value': 'CITATION SERVED' }," + NL +
        "{ 'key': 'DST', 'code': 'G', 'value': 'BOND REINSTATED' }," + NL +
        "{ 'key': 'DST', 'code': 'H', 'value': 'OLD CASE NEEDS RESOLUTION' }," + NL +
        "{ 'key': 'DST', 'code': 'I', 'value': 'INSUFFICIENT BOND' }," + NL +
        "{ 'key': 'DST', 'code': 'J', 'value': 'DEFENDANT WAS PLACED IN A HARRIS COUNTY JAIL' }," + NL +
        "{ 'key': 'DST', 'code': 'K', 'value': 'JUSTICE COURT CASE, PRE-WARRANT' }," + NL +
        "{ 'key': 'DST', 'code': 'L', 'value': 'JUSTICE COURT CASE, COMMUNITY SERVICE' }," + NL +
        "{ 'key': 'DST', 'code': 'M', 'value': 'JUSTICE COURT CASE, POST-WARRANT' }," + NL +
        "{ 'key': 'DST', 'code': 'N', 'value': 'WARRANT OR CITATION ISSUED' }," + NL +
        "{ 'key': 'DST', 'code': 'O', 'value': 'SUMMONS OUTSTANDING' }," + NL +
        "{ 'key': 'DST', 'code': 'P', 'value': 'PROBATION' }," + NL +
        "{ 'key': 'DST', 'code': 'Q', 'value': 'EXPUNCTION REQUESTED' }," + NL +
        "{ 'key': 'DST', 'code': 'R', 'value': 'WARRANT RETURNED IN WARRANT SUBSYSTEM, UNEXECUTED' }," + NL +
        "{ 'key': 'DST', 'code': 'S', 'value': 'SUMMONS RETURNED' }," + NL +
        "{ 'key': 'L_D', 'code': 'F', 'value': 'FELONY' }," + NL +
        "{ 'key': 'L_D', 'code': 'F1', 'value': 'FELONY FIRST DEGREE' }," + NL +
        "{ 'key': 'L_D', 'code': 'F2', 'value': 'FELONY SECOND DEGREE' }," + NL +
        "{ 'key': 'L_D', 'code': 'F3', 'value': 'FELONY THIRD DEGREE' }," + NL +
        "{ 'key': 'L_D', 'code': 'FC', 'value': 'FELONY CAPITAL' }," + NL +
        "{ 'key': 'L_D', 'code': 'FS', 'value': 'FELONY STATE JAIL' }," + NL +
        "{ 'key': 'L_D', 'code': 'M', 'value': 'MISDEMEANOR' }," + NL +
        "{ 'key': 'L_D', 'code': 'MA', 'value': 'MISDEMEANOR CLASS A' }," + NL +
        "{ 'key': 'L_D', 'code': 'MB', 'value': 'MISDEMEANOR CLASS B' }," + NL +
        "{ 'key': 'L_D', 'code': 'MC', 'value': 'MISDEMEANOR CLASS C' }," + NL +
        "{ 'key': 'CNC', 'code': 'AA', 'value': 'MISDEMEANOR UNSETTABLE DATE' }," + NL +
        "{ 'key': 'CNC', 'code': 'AC', 'value': 'ASSIGNED COURT' }," + NL +
        "{ 'key': 'CNC', 'code': 'AD', 'value': 'ATTORNEY CONSULTATION' }," + NL +
        "{ 'key': 'CNC', 'code': 'AJ1', 'value': 'ASSOCIATE JUDGE DOCKET 1' }," + NL +
        "{ 'key': 'CNC', 'code': 'AJ2', 'value': 'ASSOCIATE JUDGE DOCKET 2' }," + NL +
        "{ 'key': 'CNC', 'code': 'AJ3', 'value': 'ASSOCIATE JUDGE DOCKET 3' }," + NL +
        "{ 'key': 'CNC', 'code': 'AJ4', 'value': 'ASSOCIATE JUDGE DOCKET 4' }," + NL +
        "{ 'key': 'CNC', 'code': 'AJ5', 'value': 'ASSOCIATE JUDGE DOCKET 5' }," + NL +
        "{ 'key': 'CNC', 'code': 'AJ6', 'value': 'ASSOCIATE JUDGE DOCKET 6' }," + NL +
        "{ 'key': 'CNC', 'code': 'AJR', 'value': 'ASSOCIATE JUDGE REFERRAL DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'AP', 'value': 'PENDING APPEAL' }," + NL +
        "{ 'key': 'CNC', 'code': 'AR', 'value': 'ARRAIGNMENT DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'CR', 'value': 'COMPETENCY RESTORATION' }," + NL +
        "{ 'key': 'CNC', 'code': 'CS', 'value': 'COMPETENCY STAFFING' }," + NL +
        "{ 'key': 'CNC', 'code': 'ER2', 'value': 'EMERGENCY RESPONSE DOCKET 2' }," + NL +
        "{ 'key': 'CNC', 'code': 'ERD', 'value': 'EMERGENCY RESPONSE DOCKET 1' }," + NL +
        "{ 'key': 'CNC', 'code': 'ET', 'value': 'EXAMINING TRIAL DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'FJ', 'value': 'VIDEO PLEA DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'GJ', 'value': 'GRAND JURY AGENDA' }," + NL +
        "{ 'key': 'CNC', 'code': 'IA', 'value': 'IMPACT COURT DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'IB', 'value': 'IMPACT COURT DOCKET II' }," + NL +
        "{ 'key': 'CNC', 'code': 'IC', 'value': 'EMERGENCY RELIEF DOCKET 3' }," + NL +
        "{ 'key': 'CNC', 'code': 'ID', 'value': 'EMERGENCY RELIEF DOCKET 4' }," + NL +
        "{ 'key': 'CNC', 'code': 'IE', 'value': 'EMERGENCY RELIEF DOCKET 5' }," + NL +
        "{ 'key': 'CNC', 'code': 'IF', 'value': 'EMERGENCY RELIEF DOCKET 6' }," + NL +
        "{ 'key': 'CNC', 'code': 'IR', 'value': 'REINTEGRATION DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'JA', 'value': 'BAIL REVIEW DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'JB', 'value': 'JAIL LOCATION 2' }," + NL +
        "{ 'key': 'CNC', 'code': 'JC', 'value': 'IMPACT COURT DOCKET III' }," + NL +
        "{ 'key': 'CNC', 'code': 'JD', 'value': 'JAIL DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'JE', 'value': 'NRG JURY DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'JT', 'value': 'JURY TRIAL DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'MD', 'value': 'MASTER DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'MH', 'value': 'FELONY MENTAL HEALTH COURT' }," + NL +
        "{ 'key': 'CNC', 'code': 'MO', 'value': 'MOTIONS DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'NA', 'value': 'NO ARREST DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'NI', 'value': 'NO ISSUE DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'OA', 'value': 'OTHER CASES PENDING APPEAL' }," + NL +
        "{ 'key': 'CNC', 'code': 'PC', 'value': 'PROBABLE CAUSE HEARING' }," + NL +
        "{ 'key': 'CNC', 'code': 'PD', 'value': 'PLEA DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'PM', 'value': 'PROBATION MOTIONS DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'PS', 'value': 'PENDING RESTORATION OF SANITY' }," + NL +
        "{ 'key': 'CNC', 'code': 'PW', 'value': 'POST CONVICTION WRIT' }," + NL +
        "{ 'key': 'CNC', 'code': 'RD', 'value': 'RESEARCH DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'RI', 'value': 'RE-INDICTMENT TEMPORARY DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'S1', 'value': 'STAR 1 JAIL DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'S2', 'value': 'STAR 2 JAIL DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'S3', 'value': 'STAR 3 JAIL DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'S4', 'value': 'STAR 4 JAIL DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'SD', 'value': 'SENTENCING DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'SF', 'value': 'SEIZURE AND FORFEITURE' }," + NL +
        "{ 'key': 'CNC', 'code': 'SP', 'value': 'SIPS (SUBJECT AND PROCESS SYSTEM) DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'TBC', 'value': 'CASES TO BE CALENDARED' }," + NL +
        "{ 'key': 'CNC', 'code': 'TD', 'value': 'TRIAL DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'TR', 'value': 'TRANSFER DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'V1', 'value': 'VTC JAIL DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'VC', 'value': 'VETERANS COURT' }," + NL +
        "{ 'key': 'CNC', 'code': 'WD', 'value': 'WAIVER DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'WH', 'value': 'WRIT HEARING DOCKET' }," + NL +
        "{ 'key': 'CNC', 'code': 'XX', 'value': 'CTI-MH' }," + NL +
        "{ 'key': 'CNC', 'code': 'YY', 'value': 'REFERRAL COURT DOCKET' }," + NL +
        "{ 'key': 'REA', 'code': 'ADGM', 'value': 'ADJUDICATION MOTION GRANTED' }," + NL +
        "{ 'key': 'REA', 'code': 'ADMP', 'value': 'PROGRAM ADMISSION PENDING' }," + NL +
        "{ 'key': 'REA', 'code': 'AMO1', 'value': 'FIRST AMENDED MOTION' }," + NL +
        "{ 'key': 'REA', 'code': 'AMO2', 'value': 'SECOND AMENDED MOTION' }," + NL +
        "{ 'key': 'REA', 'code': 'AMO3', 'value': 'THIRD AMENDED MOTION' }," + NL +
        "{ 'key': 'REA', 'code': 'AMO4', 'value': 'FOURTH AMENDED MOTION' }," + NL +
        "{ 'key': 'REA', 'code': 'AMO5', 'value': 'FIFTH AMENDED MOTION' }," + NL +
        "{ 'key': 'REA', 'code': 'ANSW', 'value': 'ANSWER SEIZURE AND FORFIETURE PETITION' }," + NL +
        "{ 'key': 'REA', 'code': 'APHR', 'value': 'APPEAL HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'APPL', 'value': 'MOTION OF APPEAL' }," + NL +
        "{ 'key': 'REA', 'code': 'APRB', 'value': 'AMEND PROBATION' }," + NL +
        "{ 'key': 'REA', 'code': 'ARRG', 'value': 'ARRAIGNMENT' }," + NL +
        "{ 'key': 'REA', 'code': 'ASMT', 'value': 'ASSESSMENT' }," + NL +
        "{ 'key': 'REA', 'code': 'BLHG', 'value': 'BAIL REVIEW HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'CAR', 'value': 'CITE AND RELEASE APPEARANCE' }," + NL +
        "{ 'key': 'REA', 'code': 'CLJE', 'value': 'CANCEL SETTING' }," + NL +
        "{ 'key': 'REA', 'code': 'CMAP', 'value': 'COMMITMENT APPLICATION' }," + NL +
        "{ 'key': 'REA', 'code': 'CMCR', 'value': 'CASE MANAGEMENT CONFERENCE' }," + NL +
        "{ 'key': 'REA', 'code': 'COMP', 'value': 'COMPLIANCE DOCKET' }," + NL +
        "{ 'key': 'REA', 'code': 'CPSI', 'value': 'CLINICAL PRESCREENING INTERVIEW' }," + NL +
        "{ 'key': 'REA', 'code': 'CTRL', 'value': 'COURT TRIAL' }," + NL +
        "{ 'key': 'REA', 'code': 'DADD', 'value': 'DIVERT DISP' }," + NL +
        "{ 'key': 'REA', 'code': 'DADH', 'value': 'DIVERT HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'DADS', 'value': 'DIVERT STATUS' }," + NL +
        "{ 'key': 'REA', 'code': 'DCCS', 'value': 'DCM CONSULTATION SETTING' }," + NL +
        "{ 'key': 'REA', 'code': 'DCEE', 'value': 'DCM EVIDENCE EXCHANGE' }," + NL +
        "{ 'key': 'REA', 'code': 'DCPC', 'value': 'DCM COMPREHENSIVE PRETRIAL CONFERENCE' }," + NL +
        "{ 'key': 'REA', 'code': 'DFPD', 'value': 'DEFERRED PAYMENT' }," + NL +
        "{ 'key': 'REA', 'code': 'DISC', 'value': 'DISCOVERY COMPLIANCE' }," + NL +
        "{ 'key': 'REA', 'code': 'DISM', 'value': 'DISMISSAL' }," + NL +
        "{ 'key': 'REA', 'code': 'DISP', 'value': 'DISPOSITION' }," + NL +
        "{ 'key': 'REA', 'code': 'DPID', 'value': 'DWI PTI DISPOSITION' }," + NL +
        "{ 'key': 'REA', 'code': 'DPIH', 'value': 'DWI PTI HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'DPIV', 'value': 'DWI PTI VIOLATION' }," + NL +
        "{ 'key': 'REA', 'code': 'EXHR', 'value': 'EXPUNG-REQ-HEAR' }," + NL +
        "{ 'key': 'REA', 'code': 'EXPH', 'value': 'EX PARTE HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'EXTR', 'value': 'EXAMING TRIAL' }," + NL +
        "{ 'key': 'REA', 'code': 'FANN', 'value': 'FUTURE ANNOUNCEMENTS' }," + NL +
        "{ 'key': 'REA', 'code': 'FELP', 'value': 'FELONY PENDING' }," + NL +
        "{ 'key': 'REA', 'code': 'FUG', 'value': 'FUGITIVE' }," + NL +
        "{ 'key': 'REA', 'code': 'GJ', 'value': 'GRAND JURY' }," + NL +
        "{ 'key': 'REA', 'code': 'HEAR', 'value': 'JAIL HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'JTCR', 'value': 'JURY TRIAL CONFERENCE' }," + NL +
        "{ 'key': 'REA', 'code': 'JTRL', 'value': 'JURY TRIAL' }," + NL +
        "{ 'key': 'REA', 'code': 'MADJ', 'value': 'MOTION TO AJUDICATE' }," + NL +
        "{ 'key': 'REA', 'code': 'MAJH', 'value': 'MOTION TO ADJUDICATE HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'MCCA', 'value': 'MAILED POST-CONVICTION WRIT TO AUSTIN' }," + NL +
        "{ 'key': 'REA', 'code': 'MCH', 'value': 'MENTAL HEALTH COMPETENCY HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'MCHJ', 'value': 'MENTAL COMPETENCY HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'MCHR', 'value': 'MENTAL COMPETENCY HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'MCO', 'value': 'MOTION FOR CONTINUANCE' }," + NL +
        "{ 'key': 'REA', 'code': 'MCOH', 'value': 'MOTION FOR CONTINUANCE' }," + NL +
        "{ 'key': 'REA', 'code': 'MCRH', 'value': 'MENTAL COMPETENCY RESTORATION HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'MDH', 'value': 'MOTION FOR DISCOVERY' }," + NL +
        "{ 'key': 'REA', 'code': 'MDHR', 'value': 'MOTION FOR DISCOVERY HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'MDHV', 'value': 'MOTTION/DISCOVERY VIDEOTAPE' }," + NL +
        "{ 'key': 'REA', 'code': 'MDTH', 'value': 'MANDATE HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'MDTR', 'value': 'MANDATE RETURNED' }," + NL +
        "{ 'key': 'REA', 'code': 'MNT', 'value': 'MOTION FOR NEW TRIAL HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'MNTH', 'value': 'MOTION FOR NEW TRIAL HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'MOTN', 'value': 'MOTION HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'MRP', 'value': 'MOTION TO REVOKE PROBATION' }," + NL +
        "{ 'key': 'REA', 'code': 'MRPH', 'value': 'MOTION TO REVOKE PROBATION HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'MSE', 'value': 'MOTION TO SUPPRESS EVIDENCE' }," + NL +
        "{ 'key': 'REA', 'code': 'MSEH', 'value': 'MOTION TO SUPPRESS EVIDENCE HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'NGIH', 'value': 'NGRI COMMITMENT HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'NGIS', 'value': 'NGRI COMMITMENT STATUS CONFERENCE' }," + NL +
        "{ 'key': 'REA', 'code': 'NTRL', 'value': 'NON-TRIAL SETTING' }," + NL +
        "{ 'key': 'REA', 'code': 'OHC', 'value': 'OPEN HOURS COURT' }," + NL +
        "{ 'key': 'REA', 'code': 'ONDC', 'value': 'ORDER FOR NON DISCLOSURE' }," + NL +
        "{ 'key': 'REA', 'code': 'OOC', 'value': 'OUT OF COUNTY' }," + NL +
        "{ 'key': 'REA', 'code': 'OTHA', 'value': 'APPEAL ON OTHER CASE' }," + NL +
        "{ 'key': 'REA', 'code': 'OTHR', 'value': 'OTHER' }," + NL +
        "{ 'key': 'REA', 'code': 'OTHT', 'value': 'TDC ON OTHER CASE' }," + NL +
        "{ 'key': 'REA', 'code': 'PACA', 'value': 'PRELIMINARY ASSIGNED COURT APPEARANCE' }," + NL +
        "{ 'key': 'REA', 'code': 'PCD', 'value': 'PROBABLE CAUSE DOCKET' }," + NL +
        "{ 'key': 'REA', 'code': 'PCS', 'value': 'PROBABLE CAUSE' }," + NL +
        "{ 'key': 'REA', 'code': 'PFBL', 'value': 'PREVIOUS FELONY BAIL' }," + NL +
        "{ 'key': 'REA', 'code': 'PFCS', 'value': 'PAYMENT FINE, COST, SUPERVISORY FEE' }," + NL +
        "{ 'key': 'REA', 'code': 'PGJ', 'value': 'PENDING GRAND JURY' }," + NL +
        "{ 'key': 'REA', 'code': 'PIA', 'value': 'PRELIMINARY INITIAL APPEARANCE' }," + NL +
        "{ 'key': 'REA', 'code': 'PLEA', 'value': 'PLEA' }," + NL +
        "{ 'key': 'REA', 'code': 'PLWO', 'value': 'PLEA WITHOUT RECOMMENDATION' }," + NL +
        "{ 'key': 'REA', 'code': 'PLWR', 'value': 'PLEA WITH RECOMMENDATION' }," + NL +
        "{ 'key': 'REA', 'code': 'PNDC', 'value': 'PETITION FOR NON-DISCLOSURE' }," + NL +
        "{ 'key': 'REA', 'code': 'PNGJ', 'value': 'PENDING GRAND JURY' }," + NL +
        "{ 'key': 'REA', 'code': 'PNSE', 'value': 'PENDING SETTLEMENT SEIZURE AND FORFIETURE' }," + NL +
        "{ 'key': 'REA', 'code': 'PRDK', 'value': 'PRIOR DOCKET' }," + NL +
        "{ 'key': 'REA', 'code': 'PSI', 'value': 'PRE-SENTENCE INVESTIGATION' }," + NL +
        "{ 'key': 'REA', 'code': 'PSIH', 'value': 'PRE-SENTENCE INVESTIGATION HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'PTCR', 'value': 'PRE-TRIAL CONFERENCE' }," + NL +
        "{ 'key': 'REA', 'code': 'PTDV', 'value': 'PRE TRIAL DIVERSION VIOLATION' }," + NL +
        "{ 'key': 'REA', 'code': 'PTH', 'value': 'PRE-TRIAL HEARINGS' }," + NL +
        "{ 'key': 'REA', 'code': 'PTID', 'value': 'PRE-TRIAL INTERVENTION DISPOSITION' }," + NL +
        "{ 'key': 'REA', 'code': 'PTIH', 'value': 'PRETRIAL INTERVENTION HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'PTMO', 'value': 'PRE TRIAL MOTIONS' }," + NL +
        "{ 'key': 'REA', 'code': 'PUHR', 'value': 'PUNISHMENT HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'RDLH', 'value': 'RESTRICTED DRIVERS LICENSE HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'RPC', 'value': 'REVIEW PROBATION CONDITIONS' }," + NL +
        "{ 'key': 'REA', 'code': 'RPCC', 'value': 'REVIEW PROBATION CONDITIONS CONFERENCE' }," + NL +
        "{ 'key': 'REA', 'code': 'RPM', 'value': 'PROBATE MOTION' }," + NL +
        "{ 'key': 'REA', 'code': 'RPM1', 'value': '1ST AMENDED PROBATE MOTION' }," + NL +
        "{ 'key': 'REA', 'code': 'RPM2', 'value': '2ND AMENDED PROBATE MOTION' }," + NL +
        "{ 'key': 'REA', 'code': 'RPM3', 'value': '3RD AMENDED PROBATE MOTION' }," + NL +
        "{ 'key': 'REA', 'code': 'RPM4', 'value': '4TH AMENDED PROBATE MOTION' }," + NL +
        "{ 'key': 'REA', 'code': 'RPM5', 'value': '5TH AMENDED PROBATE MOTION' }," + NL +
        "{ 'key': 'REA', 'code': 'RSCH', 'value': 'RESEARCHING' }," + NL +
        "{ 'key': 'REA', 'code': 'RSHR', 'value': 'RESTORATION OF SANITY HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'SCHR', 'value': 'SHOW CAUSE HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'SENT', 'value': 'SENTENCING' }," + NL +
        "{ 'key': 'REA', 'code': 'SFBF', 'value': 'SET FOR BOND FORFEITURE' }," + NL +
        "{ 'key': 'REA', 'code': 'SFJS', 'value': 'SET FOR JUDGMENT AND SENTENCE' }," + NL +
        "{ 'key': 'REA', 'code': 'SPHR', 'value': 'SHOCK PROBATION HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'STCR', 'value': 'STATUS CONFERENCE' }," + NL +
        "{ 'key': 'REA', 'code': 'TRAN', 'value': 'TRANSFER' }," + NL +
        "{ 'key': 'REA', 'code': 'TRIA', 'value': 'TRIAL' }," + NL +
        "{ 'key': 'REA', 'code': 'WRT', 'value': 'WRIT HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'WRTH', 'value': 'WRIT OF HABEAS CORPUS HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'WVEX', 'value': 'WAIVER OF EXTRADITION' }," + NL +
        "{ 'key': 'REA', 'code': 'WVHR', 'value': 'WAIVER OF HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'WVST', 'value': 'WAIVER OF SPEEDY TRIAL' }," + NL +
        "{ 'key': 'REA', 'code': 'XDLH', 'value': 'ALR RESTRICTED DRIVERS LICENSE HEARING' }," + NL +
        "{ 'key': 'REA', 'code': 'YDLH', 'value': 'APPEAL OF ALR SUSPENSION HEARING' }," + NL +
        "{ 'key': 'DEF_RAC', 'code': 'A', 'value': 'ASIAN OR PACIFIC ISLANDER' }," + NL +
        "{ 'key': 'DEF_RAC', 'code': 'B', 'value': 'BLACK ' }," + NL +
        "{ 'key': 'DEF_RAC', 'code': 'I', 'value': 'NATIVE AMERICAN ' }," + NL +
        "{ 'key': 'DEF_RAC', 'code': 'M', 'value': 'MULTIRACIAL ' }," + NL +
        "{ 'key': 'DEF_RAC', 'code': 'U', 'value': 'UNKNOWN ' }," + NL +
        "{ 'key': 'DEF_RAC', 'code': 'W', 'value': 'WHITE ' }," + NL +
        "{ 'key': 'DEF_CITIZEN', 'code': 'N', 'value': 'NO' }," + NL +
        "{ 'key': 'DEF_CITIZEN', 'code': 'Y', 'value': 'YES' }," + NL +
        "{ 'key': 'DEF_CITIZEN', 'code': 'U', 'value': 'UNKNOWN' }," + NL +
        "{ 'key': 'BAMEXP', 'code': 'D', 'value': 'BOND DENIED' }," + NL +
        "{ 'key': 'BAMEXP', 'code': 'I', 'value': 'DUPLICATE INDICTMENT' }," + NL +
        "{ 'key': 'BAMEXP', 'code': 'R', 'value': 'REFER TO MAGISTRATE' }," + NL +
        "{ 'key': 'BAMEXP', 'code': 'S', 'value': 'SUMMONS ISSUED' }," + NL +
        "{ 'key': 'BAMEXP', 'code': 'U', 'value': 'UNSECURED GOB ELIGIBLE' }," + NL +
        "{ 'key': 'GJ_CDP', 'code': 'FID', 'value': 'FELONY INDICTMENT' }," + NL +
        "{ 'key': 'GJ_CDP', 'code': 'FIN', 'value': 'FELONY INFORMATION' }," + NL +
        "{ 'key': 'GJ_CDP', 'code': 'MID', 'value': 'MISDEMEANOR INDICTMENT' }," + NL +
        "{ 'key': 'GJ_CDP', 'code': 'NOB', 'value': 'NO BILL' }," + NL +
        "{ 'key': 'GJ_CDP', 'code': 'RID', 'value': 'REINDICTMENT' }," + NL +
        "{ 'key': 'GJ_CDP', 'code': 'FID', 'value': 'FELONY INDICTMENT' }," + NL +
        "{ 'key': 'GJ_CDP', 'code': 'FIN', 'value': 'FELONY INFORMATION' }," + NL +
        "{ 'key': 'GJ_CDP', 'code': 'MID', 'value': 'MISDEMEANOR INDICTMENT' }," + NL +
        "{ 'key': 'GJ_CDP', 'code': 'NOB', 'value': 'NO BILL' }," + NL +
        "{ 'key': 'GJ_CDP', 'code': 'RID', 'value': 'REINDICTMENT' }" + NL +
        "]";
    }
}
