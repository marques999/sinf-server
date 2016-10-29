using System;
using System.Text;
using System.Web;
using System.Web.Http.Description;

namespace FirstREST.Areas.HelpPage
{
    public static class ApiDescriptionExtensions
    {
        public static string GetFriendlyId(this ApiDescription description)
        {
            string path = description.RelativePath;
            string[] urlParts = path.Split('?');
            string localPath = urlParts[0];
            string queryKeyString = null;

            if (urlParts.Length > 1)
            {
                queryKeyString = String.Join("_", HttpUtility.ParseQueryString(urlParts[1]).AllKeys);
            }

            var friendlyPath = new StringBuilder();

            friendlyPath.AppendFormat("{0}-{1}", description.HttpMethod.Method, localPath.Replace("/", "-").Replace("{", String.Empty).Replace("}", String.Empty));

            if (queryKeyString != null)
            {
                friendlyPath.AppendFormat("_{0}", queryKeyString);
            }

            return friendlyPath.ToString();
        }
    }
}