using System;
using System.Linq;
using System.Collections.Generic;

using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera
{
    public class Authentication
    {
        private static Dictionary<String, Session> sessions = new Dictionary<String, Session>();

        public static string CreateSession(string userName, string representativeId)
        {
            if (sessions.Count(session => session.Value.Username == userName) > 0)
            {
                return null;
            }

            var userToken = generateToken();

            sessions.Add(userToken, new Session(userName, representativeId));

            return userToken;
        }

        public static bool TerminateSession(string userToken)
        {
            return VerifyToken(userToken) ? sessions.Remove(userToken) : false;
        }

        private static string generateToken()
        {
            return Convert.ToBase64String(BitConverter.GetBytes(DateTime.UtcNow.ToBinary()).Concat(Guid.NewGuid().ToByteArray()).ToArray());
        }

        public static bool VerifyToken(string userToken)
        {
            return true;
            //return DateTime.FromBinary(BitConverter.ToInt64(Convert.FromBase64String(userToken), 0)) >= DateTime.UtcNow.AddMinutes(-30);
        }

        internal static string GetRepresentative(string sessionId)
        {
            return "1";
        }
    }
}