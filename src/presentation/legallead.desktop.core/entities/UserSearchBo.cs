using System.ComponentModel.DataAnnotations;

namespace legallead.desktop.entities
{
    internal class UserSearchBo
    {
        private IEnumerable<CountyParameterModel> _parameters = 
            Enumerable.Empty<CountyParameterModel>();

        public string UserName { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;

        [Required]
        [StringLength(2)]
        public string State { get; set; } = string.Empty;
        [Required]
        public string County { get; set; } = string.Empty;
        [Required]
        public DateTime? StartDate { get; set; }
        [Required]
        public DateTime? EndDate { get; set; }
        public IEnumerable<CountyParameterModel> Parameters { 
            get {  return _parameters; } 
            set 
            { 
                _parameters = value;
                SearchStarted = DateTime.UtcNow;
                ParameterChanged?.Invoke();
            }
        }

        public Action? ParameterChanged { get; internal set; }
        internal DateTime? SearchStarted { get; set; }
    }
}
