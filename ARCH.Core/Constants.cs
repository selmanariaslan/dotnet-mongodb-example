namespace ARCH.CoreLibrary
{
    public static class Constants
    {
        public static class ErrorMessageTypes
        {
            public const string Info = "Info";
            public const string Warning = "Warning";
            public const string Error = "Error";

            public const string BusinessError = "BusinessError";
            public const string ExceptionLogId = "ExceptionLogId";
            public const string UserMessage = "UserMessage";
        }

        public static class DefaultUserMessagesTR
        {
            public const string Success = "İşleminiz başarıyla gerçekleşti.";
            public const string Error = "İşleminiz gerçekleştirilmeye çalışılırken bir hata alındı.";
            public const string Warning = "İşleminiz gerçşekleştirilemedi, Lütfen birazdan tekrar deneyin.";
        }

        public static class DbStatus
        {
            public const int Deleted = -1;
            public const int Passive = 0;
            public const int Active = 1;
        }
    }
}