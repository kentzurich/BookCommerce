using BookCommerce_Utility;
using BookCommerce_WEB.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System.Collections;
using System.Text;
using static BookCommerce_Utility.StaticDetails;

namespace BookCommerce_WEB.Services.APIMessageRequestBuilder
{
    public class APIMessageRequestBuilder : IAPIMessageRequestBuilder
    {
        public HttpRequestMessage Build(APIRequest request)
        {
            HttpRequestMessage requestMessage = new();

            //Headers
            if (request.ContentType == ContentType.MultipartFormData)
                requestMessage.Headers.Add("Accept", "*/*");
            else
                requestMessage.Headers.Add("Accept", "application/json");

            //Request URL
            requestMessage.RequestUri = new(request.URL);

            //Content
            if(request.ContentType == ContentType.MultipartFormData)
            {
                //This is for file content
                var multipartFormDataContent = new MultipartFormDataContent();

                foreach(var property in request.Data.GetType().GetProperties())
                {
                    var value = property.GetValue(request.Data);
                    if(property.PropertyType == typeof(List<IFormFile>))
                    {
                        var files = (List<IFormFile>)value;

                        if (files != null)
                        {
                            foreach(var file in files)
                            {
                                multipartFormDataContent.Add(
                                new StreamContent(file.OpenReadStream()),
                                property.Name,
                                file.FileName);
                            }
                        }
                    }
                    else
                    {
                        multipartFormDataContent.Add(
                                new StringContent(value == null ? string.Empty : value.ToString()),
                                property.Name);
                    }
                }
                requestMessage.Content = multipartFormDataContent;
            }
            else
            {
                //This is for json content
                if (request.Data != null)
                {
                    requestMessage.Content = new StringContent(
                        JsonConvert.SerializeObject(request.Data),
                        Encoding.UTF8,
                        "application/json");
                }
            }

            //Verb
            switch (request.APIType)
            {
                case StaticDetails.APIType.POST:
                    requestMessage.Method = HttpMethod.Post;
                    break;
                case StaticDetails.APIType.PUT:
                    requestMessage.Method = HttpMethod.Put;
                    break;
                case StaticDetails.APIType.DELETE:
                    requestMessage.Method = HttpMethod.Delete;
                    break;
                default:
                    requestMessage.Method = HttpMethod.Get;
                    break;
            }

            return requestMessage;
        }
    }
}
