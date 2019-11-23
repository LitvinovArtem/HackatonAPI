using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebDekAPI.Auth
{
    public class RequestResult
    {
        public RequestState state { get; set; }
        public string msg { get; set; }
        public Object data { get; set; }
        public string accessToken { get; set; }
        public double requertAt { get; set; }
        public double expiresIn { get; set; }
    }

    public enum RequestState
    {
        Failed = -1,
        NotAuth = 0,
        Success = 1,
        NotAccess=-2
    }

    public static class RequestStateExtensions
    {
        public static string ToText(this RequestState me)
        {
            switch (me)
            {
                case RequestState.Failed:
                    return "Произошла ошибка";
                case RequestState.NotAccess:
                    return "Нет прав доступа на изменение ведомости";
                case RequestState.Success:
                    return "Операция прошла успешно";
                case RequestState.NotAuth:
                    return "Необходимо выполнить вход в систему";
                default:
                    return "";
            }
        }
    }

}
