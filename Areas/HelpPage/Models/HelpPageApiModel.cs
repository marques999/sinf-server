using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Web.Http.Description;

namespace FirstREST.Areas.HelpPage.Models
{
    public class HelpPageApiModel
    {
        public HelpPageApiModel()
        {
            SampleRequests = new Dictionary<MediaTypeHeaderValue, object>();
            SampleResponses = new Dictionary<MediaTypeHeaderValue, object>();
            ErrorMessages = new Collection<string>();
        }

        public ApiDescription ApiDescription
        {
            get;
            set;
        }

        public IDictionary<MediaTypeHeaderValue, object> SampleRequests
        {
            get;
            private set;
        }

        public IDictionary<MediaTypeHeaderValue, object> SampleResponses
        {
            get;
            private set;
        }

        public Collection<string> ErrorMessages
        {
            get;
            private set;
        }
    }
}