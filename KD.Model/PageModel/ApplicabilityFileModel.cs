using KD.Data;
using KD.Model.Common;

namespace KD.Model.PageModel
{
    public class ApplicabilityFileModel : ModelBase
    {
        private bool _progressIsIndeterminate;

        internal ApplicabilityFile ApplicabilityF { get; set; }

        public string Version { get { return ApplicabilityF.Version; } }
        public string NumberNotice { get { return ApplicabilityF.Notice; } }
        public int? IdFile { get { return ApplicabilityF.IdFile; } }
        public bool ProgressIsIndeterminate { get { return _progressIsIndeterminate; } set { _progressIsIndeterminate = value; OnPropertyChanged("ProgressIsIndeterminate"); } }

        public ApplicabilityFileModel(ApplicabilityFile applicabilityFile)
        {
            this.ApplicabilityF = applicabilityFile;
        }
    }
}
