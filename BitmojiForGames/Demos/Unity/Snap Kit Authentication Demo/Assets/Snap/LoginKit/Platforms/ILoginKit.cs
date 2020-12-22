using System.Collections.Generic;

namespace Snap
{
    internal interface ILoginKit
    {
        void Login();

        bool IsLoggedIn();

        void Verify(string phoneNumber, string region);

        void UnlinkAllSessions();

        string GetAccessToken();

        bool HasAccessToScope(string scope);

        void FetchUserDataWithQuery(string query, Dictionary<string, object> variables);
    }
}