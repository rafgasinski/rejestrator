namespace rejestrator.Models
{
    public class LoginModel
    {
        #region Singleton
        private static LoginModel _instance = null;
        public static LoginModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LoginModel();
                }

                return _instance;
            }
        }
        #endregion
    }
}
