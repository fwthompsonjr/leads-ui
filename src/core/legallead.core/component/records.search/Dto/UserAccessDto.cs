using legallead.records.search.Classes;
using System.Text;
using JConn = Newtonsoft.Json.JsonConvert;

namespace legallead.records.search.Dto
{
    public class UserAccessDtoCollection
    {
        public List<UserAccessDto> AccessDtos { get; set; } = new();
    }

    public class UserAccessDto
    {
        public string CreateDate { get; set; } = string.Empty;
        public string UserGuid { get; set; } = string.Empty;
        public string UserKey { get; set; } = string.Empty;

        public DateTime? CreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(CreateDate))
                {
                    return null;
                }

                if (!DateTime.TryParse(CreateDate, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out DateTime createdDate))
                {
                    return null;
                }

                return createdDate;
            }
        }

        public string UserData { get; set; } = string.Empty;

        public static List<UserAccessDto>? GetListDto(string fileSuffix)
        {
            const string dataFormat = @"{0}\xml\{1}.json";
            string appDirectory = ContextManagment.AppDirectory;
            string dataFile = string.Format(
                CultureInfo.CurrentCulture,
                dataFormat,
                appDirectory,
                fileSuffix);
            var fallback = GetFallbackContent(fileSuffix);
            var data = File.Exists(dataFile) ? File.ReadAllText(dataFile) : fallback;
            if (data.Length == 0 || !File.Exists(dataFile))
            {
                throw new FileNotFoundException(CommonKeyIndexes.NavigationFileNotFound);
            }
            List<UserAccessDto>? colUsers = JConn.DeserializeObject<List<UserAccessDto>>(data);
            if (colUsers == null)
            {
                return null;
            }

            return colUsers;
        }

        public static UserAccessDto? GetDto(string fileSuffix)
        {
            const string dataFormat = @"{0}\xml\{1}.json";
            string appDirectory = ContextManagment.AppDirectory;
            string dataFile = string.Format(
                CultureInfo.CurrentCulture,
                dataFormat,
                appDirectory,
                fileSuffix);
            var fallback = GetFallbackContent(fileSuffix);
            var data = File.Exists(dataFile) ? File.ReadAllText(dataFile) : fallback;
            if (data.Length == 0 || !File.Exists(dataFile))
            {
                throw new FileNotFoundException(CommonKeyIndexes.NavigationFileNotFound);
            }
            List<UserAccessDto>? colUsers = JConn.DeserializeObject<List<UserAccessDto>>(data);
            if (colUsers == null)
            {
                return null;
            }

            if (!colUsers.Any())
            {
                return new UserAccessDto();
            }

            return colUsers[^1];
        }

        public static UserAccessDto CreateCredential(string cleared, string userKey, string targetFile)
        {
            string decoded = CryptoEngine.Encrypt(cleared, userKey, out var data64);
            UserAccessDto dto = new()
            {
                UserKey = userKey,
                UserGuid = decoded,
                UserData = data64,
                CreateDate = DateTime.Now.ToLongDateString()
            };
            var list = GetListDto(targetFile);
            list?.Add(dto);

            const string dataFormat = @"{0}\xml\{1}.json";
            string appDirectory = ContextManagment.AppDirectory;
            string dataFile = string.Format(
                CultureInfo.CurrentCulture,
                dataFormat,
                appDirectory,
                targetFile);
            if (File.Exists(dataFile))
            {
                File.Delete(dataFile);
            }

            using (StreamWriter sw = new(dataFile))
            {
                sw.Write(JConn.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));
            }
            return dto;
        }

        public static List<string>? GetCredential(UserAccessDto dto)
        {
            if (dto == null ||
                string.IsNullOrEmpty(dto.UserGuid) || 
                string.IsNullOrEmpty(dto.UserKey) || 
                string.IsNullOrEmpty(dto.UserData))
            {
                return null;
            }

            string decoded = CryptoEngine.Decrypt(dto.UserGuid, dto.UserKey, dto.UserData);
            return decoded.Split('|').ToList();
        }


        private static string GetFallbackContent(string fileName)
        {
            var sbb = new StringBuilder();
            const char tilde = '~';
            const char qte = '"';
            if (string.IsNullOrEmpty(fileName)) return string.Empty;
            if (fileName.Equals("collinCountyCaseType"))
            {
                sbb.AppendLine("{");
                sbb.AppendLine("  ~dropDowns~: [");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~id~: 0,");
                sbb.AppendLine("      ~name~: ~criminal courts~,");
                sbb.AppendLine("      ~query~: ~#sbxControlID2~,");
                sbb.AppendLine("      ~options~: [");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 0,");
                sbb.AppendLine("          ~name~: ~criminal case records~,");
                sbb.AppendLine("          ~query~: ~#divOption1 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 1,");
                sbb.AppendLine("          ~name~: ~probate case records~,");
                sbb.AppendLine("          ~query~: ~#divOption2 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 2,");
                sbb.AppendLine("          ~name~: ~magistrate case records~,");
                sbb.AppendLine("          ~query~: ~#divOption3 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 3,");
                sbb.AppendLine("          ~name~: ~civil and family case records~,");
                sbb.AppendLine("          ~query~: ~#divOption4 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 4,");
                sbb.AppendLine("          ~name~: ~justice of the peace case records~,");
                sbb.AppendLine("          ~query~: ~#divOption5 > a~");
                sbb.AppendLine("        }");
                sbb.AppendLine("      ]");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~id~: 1,");
                sbb.AppendLine("      ~name~: ~probate courts~,");
                sbb.AppendLine("      ~query~: ~#sbxControlID2~,");
                sbb.AppendLine("      ~options~: [");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 0,");
                sbb.AppendLine("          ~name~: ~probate case records~,");
                sbb.AppendLine("          ~query~: ~#divOption2 > a~");
                sbb.AppendLine("        }");
                sbb.AppendLine("      ]");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~id~: 2,");
                sbb.AppendLine("      ~name~: ~magistrate courts~,");
                sbb.AppendLine("      ~query~: ~#sbxControlID2~,");
                sbb.AppendLine("      ~options~: [");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 0,");
                sbb.AppendLine("          ~name~: ~magistrate case records~,");
                sbb.AppendLine("          ~query~: ~#divOption3 > a~");
                sbb.AppendLine("        }");
                sbb.AppendLine("      ]");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~id~: 3,");
                sbb.AppendLine("      ~name~: ~civil and family courts~,");
                sbb.AppendLine("      ~query~: ~#sbxControlID2~,");
                sbb.AppendLine("      ~options~: [");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 0,");
                sbb.AppendLine("          ~name~: ~civil and family case records~,");
                sbb.AppendLine("          ~query~: ~#divOption4 > a~");
                sbb.AppendLine("        }");
                sbb.AppendLine("      ]");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~id~: 4,");
                sbb.AppendLine("      ~name~: ~justice courts~,");
                sbb.AppendLine("      ~query~: ~#sbxControlID2~,");
                sbb.AppendLine("      ~options~: [");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 0,");
                sbb.AppendLine("          ~name~: ~justice of the peace case records~,");
                sbb.AppendLine("          ~query~: ~#divOption5 > a~");
                sbb.AppendLine("        }");
                sbb.AppendLine("      ]");
                sbb.AppendLine("    }");
                sbb.AppendLine("  ]");
                sbb.AppendLine("}");
            }
            if (fileName.Equals("collinCountyUserMap"))
            {
                sbb.AppendLine("[");
                sbb.AppendLine("  {");
                sbb.AppendLine("    ~createDate~: ~04/01/2019 08:00~,");
                sbb.AppendLine("    ~userGuid~: ~7n4HjZvPIQ9HOIYGG2YFUgvuXxqhPHP5~,");
                sbb.AppendLine("    ~userKey~: ~data.clear.check~");
                sbb.AppendLine("  },");
                sbb.AppendLine("  {");
                sbb.AppendLine("    ~createDate~: ~11/17/2023 14:10~,");
                sbb.AppendLine("    ~userGuid~: ~c5PyieZ40EnYyHXOZulCNfjJyqXuAHVE1PLflC67mdQ=~,");
                sbb.AppendLine("    ~userData~: ~5WB6tSC95PmKZ1NLlMt5RA==~,");
                sbb.AppendLine("    ~userKey~: ~data.static.chek~");
                sbb.AppendLine("  }");
                sbb.AppendLine("]");
            }
            sbb.Replace(tilde, qte);
            return sbb.ToString();
        }
    }
}