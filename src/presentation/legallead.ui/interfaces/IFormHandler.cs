namespace legallead.ui.interfaces
{
    internal interface IFormHandler
    {
        void End();
        void SetMessage(string htm);
        void Start();
        void SubmissionCompleted();
    }
}