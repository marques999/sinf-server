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
                queryKeyString = string.Join("_", HttpUtility.ParseQueryString(urlParts[1]).AllKeys);
            }

            var friendlyPath = new StringBuilder().AppendFormat("{0}-{1}", description.HttpMethod.Method, localPath.Replace("/", "-").Replace("{", string.Empty).Replace("}", string.Empty));

            if (queryKeyString != null)
            {
                friendlyPath.AppendFormat("_{0}", queryKeyString);
            }

            return friendlyPath.ToString();
        }
    }
}