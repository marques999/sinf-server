using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Description;

using FirstREST.Areas.HelpPage.Models;

namespace FirstREST.Areas.HelpPage
{
    public static class HelpPageConfigurationExtensions
    {
        private const string ApiModelPrefix = "MS_HelpPageApiModel_";

        public static void SetDocumentationProvider(this HttpConfiguration config, IDocumentationProvider documentationProvider)
        {
            config.Services.Replace(typeof(IDocumentationProvider), documentationProvider);
        }

        public static void SetSampleObjects(this HttpConfiguration config, IDictionary<Type, object> sampleObjects)
        {
            config.GetHelpPageSampleGenerator().SampleObjects = sampleObjects;
        }

        public static void SetSampleRequest(this HttpConfiguration config, object sample, MediaTypeHeaderValue mediaType, string controllerName, string actionName)
        {
            config.GetHelpPageSampleGenerator().ActionSamples.Add(new HelpPageSampleKey(mediaType, SampleDirection.Request, controllerName, actionName, new[] { "*" }), sample);
        }

        public static void SetSampleRequest(this HttpConfiguration config, object sample, MediaTypeHeaderValue mediaType, string controllerName, string actionName, params string[] parameterNames)
        {
            config.GetHelpPageSampleGenerator().ActionSamples.Add(new HelpPageSampleKey(mediaType, SampleDirection.Request, controllerName, actionName, parameterNames), sample);
        }

        public static void SetSampleResponse(this HttpConfiguration config, object sample, MediaTypeHeaderValue mediaType, string controllerName, string actionName)
        {
            config.GetHelpPageSampleGenerator().ActionSamples.Add(new HelpPageSampleKey(mediaType, SampleDirection.Response, controllerName, actionName, new[] { "*" }), sample);
        }

        public static void SetSampleResponse(this HttpConfiguration config, object sample, MediaTypeHeaderValue mediaType, string controllerName, string actionName, params string[] parameterNames)
        {
            config.GetHelpPageSampleGenerator().ActionSamples.Add(new HelpPageSampleKey(mediaType, SampleDirection.Response, controllerName, actionName, parameterNames), sample);
        }

        public static void SetSampleForType(this HttpConfiguration config, object sample, MediaTypeHeaderValue mediaType, Type type)
        {
            config.GetHelpPageSampleGenerator().ActionSamples.Add(new HelpPageSampleKey(mediaType, type), sample);
        }

        public static void SetActualRequestType(this HttpConfiguration config, Type type, string controllerName, string actionName)
        {
            config.GetHelpPageSampleGenerator().ActualHttpMessageTypes.Add(new HelpPageSampleKey(SampleDirection.Request, controllerName, actionName, new[] { "*" }), type);
        }

        public static void SetActualRequestType(this HttpConfiguration config, Type type, string controllerName, string actionName, params string[] parameterNames)
        {
            config.GetHelpPageSampleGenerator().ActualHttpMessageTypes.Add(new HelpPageSampleKey(SampleDirection.Request, controllerName, actionName, parameterNames), type);
        }

        public static void SetActualResponseType(this HttpConfiguration config, Type type, string controllerName, string actionName)
        {
            config.GetHelpPageSampleGenerator().ActualHttpMessageTypes.Add(new HelpPageSampleKey(SampleDirection.Response, controllerName, actionName, new[] { "*" }), type);
        }

        public static void SetActualResponseType(this HttpConfiguration config, Type type, string controllerName, string actionName, params string[] parameterNames)
        {
            config.GetHelpPageSampleGenerator().ActualHttpMessageTypes.Add(new HelpPageSampleKey(SampleDirection.Response, controllerName, actionName, parameterNames), type);
        }

        public static HelpPageSampleGenerator GetHelpPageSampleGenerator(this HttpConfiguration config)
        {
            return (HelpPageSampleGenerator)config.Properties.GetOrAdd(
                typeof(HelpPageSampleGenerator),
                k => new HelpPageSampleGenerator());
        }

        public static void SetHelpPageSampleGenerator(this HttpConfiguration config, HelpPageSampleGenerator sampleGenerator)
        {
            config.Properties.AddOrUpdate(
                typeof(HelpPageSampleGenerator),
                k => sampleGenerator,
                (k, o) => sampleGenerator);
        }

        public static HelpPageApiModel GetHelpPageApiModel(this HttpConfiguration config, string apiDescriptionId)
        {
            object model;
            string modelId = ApiModelPrefix + apiDescriptionId;

            if (!config.Properties.TryGetValue(modelId, out model))
            {
                var apiDescriptions = config.Services.GetApiExplorer().ApiDescriptions;
                var apiDescription = apiDescriptions.FirstOrDefault(api => String.Equals(api.GetFriendlyId(), apiDescriptionId, StringComparison.OrdinalIgnoreCase));

                if (apiDescription != null)
                {
                    HelpPageSampleGenerator sampleGenerator = config.GetHelpPageSampleGenerator();
                    model = GenerateApiModel(apiDescription, sampleGenerator);
                    config.Properties.TryAdd(modelId, model);
                }
            }

            return (HelpPageApiModel)model;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The exception is recorded as ErrorMessages.")]
        private static HelpPageApiModel GenerateApiModel(ApiDescription apiDescription, HelpPageSampleGenerator sampleGenerator)
        {
            var apiModel = new HelpPageApiModel();

            apiModel.ApiDescription = apiDescription;

            try
            {
                foreach (var item in sampleGenerator.GetSampleRequests(apiDescription))
                {
                    apiModel.SampleRequests.Add(item.Key, item.Value);
                    LogInvalidSampleAsError(apiModel, item.Value);
                }

                foreach (var item in sampleGenerator.GetSampleResponses(apiDescription))
                {
                    apiModel.SampleResponses.Add(item.Key, item.Value);
                    LogInvalidSampleAsError(apiModel, item.Value);
                }
            }
            catch (Exception e)
            {
                apiModel.ErrorMessages.Add(String.Format(CultureInfo.CurrentCulture, "An exception has occurred while generating the sample. Exception Message: {0}", e.Message));
            }

            return apiModel;
        }

        private static void LogInvalidSampleAsError(HelpPageApiModel apiModel, object sample)
        {
            var invalidSample = sample as InvalidSample;

            if (invalidSample != null)
            {
                apiModel.ErrorMessages.Add(invalidSample.ErrorMessage);
            }
        }
    }
}