
namespace VolatilityWPFApp
{
    /// <summary>
    /// It stores information related to the notificatins they arrive via the callbak.
    /// </summary>
    internal class NotificationInfo
    {
        public int Updates = 0;
        public int Deletions = 0;
        public int Additions = 0;
        public int Errors = 0;
        public int RecordsDisplayed = 0;

        public void Reset()
        {
            Updates = 0;
            Deletions = 0;
            Additions = 0;
            Errors = 0;
            RecordsDisplayed = 0;
        }

        /// <summary>
        /// The string to display in the status label.
        /// </summary>
        /// <returns></returns>
        public string GetDisplayString()
        {
            var ret = string.Format("ROWS: {4:D5}, UPDATES: {0:D3}, NEW: {1:D3}, DELETIONS: {2:D3}, ERRORS: {3:D3}", Updates, Additions,Deletions,Errors,RecordsDisplayed );
            return ret;
        }
    }
}
