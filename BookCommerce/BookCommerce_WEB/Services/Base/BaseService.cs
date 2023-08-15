using BookCommerce_Utility;
using BookCommerce_WEB.Models;
using BookCommerce_WEB.Models.DTO.ApplicationUser;
using BookCommerce_WEB.Services.APIMessageRequestBuilder;
using BookCommerce_WEB.Services.TokenProvider;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace BookCommerce_WEB.Services.Base
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IAPIMessageRequestBuilder _apiRequestBuilder;
        private readonly ITokenProvider _tokenProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public APIResponse response { get; set; }
        private readonly string bookCommerceUrl;

        public BaseService(IHttpClientFactory httpClient,
                           IAPIMessageRequestBuilder apiRequestBuilder,
                           ITokenProvider tokenProvider,
                           IConfiguration config,
                           IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _apiRequestBuilder = apiRequestBuilder;
            _tokenProvider = tokenProvider;
            _httpContextAccessor = httpContextAccessor;
            bookCommerceUrl = config.GetValue<string>("ServiceUrls:BookCommerce_API");
        }

        public async Task<T> SendAsync<T>(APIRequest request, bool withBearer = true)
        {
            try
            {
                var client = _httpClient.CreateClient("BookCommerce_API");
                var messageFactory = () => _apiRequestBuilder.Build(request);

                HttpResponseMessage httpResponse = null;
                httpResponse = await SendWithResfreshToken(client, messageFactory, withBearer);

                APIResponse response = new();

                try
                {
                    switch(httpResponse.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            response = Response(HttpStatusCode.NotFound, false, new List<string> { "Not Found" });
                            break;
                        case HttpStatusCode.Forbidden:
                            response = Response(HttpStatusCode.Forbidden, false, new List<string> { "Forbidden" });
                            break;
                        case HttpStatusCode.Unauthorized:
                            response = Response(HttpStatusCode.Unauthorized, false, new List<string> { "Unauthorized" });
                            break;
                        case HttpStatusCode.InternalServerError:
                            response = Response(HttpStatusCode.InternalServerError, false, new List<string> { "Internal Server Error" });
                            break;
                        case HttpStatusCode.BadRequest:
                            response = Response(HttpStatusCode.BadRequest, false, new List<string> { "Bad Request" });
                            break;
                        case HttpStatusCode.UnsupportedMediaType:
                            response = Response(HttpStatusCode.UnsupportedMediaType, false, new List<string> { "Unsupported Media Type" });
                            break;
                        case HttpStatusCode.MethodNotAllowed:
                            response = Response(HttpStatusCode.MethodNotAllowed, false, new List<string> { "Method Not Allowed" });
                            break;
                        default:
                            var content = await httpResponse.Content.ReadAsStringAsync();
                            response.IsSuccess = true;
                            response = JsonConvert.DeserializeObject<APIResponse>(content);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    response = Response(HttpStatusCode.InternalServerError, false, new List<string> { "Error Encountered", ex.Message.ToString() });
                }

                var responseObj = JsonConvert.SerializeObject(response);
                var returnObj = JsonConvert.DeserializeObject<T>(responseObj);

                return returnObj;
            }
            catch (AuthException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var errorResponse = Response(HttpStatusCode.InternalServerError, false, new List<string> { "Error Encountered", ex.Message.ToString() });
                var response = JsonConvert.SerializeObject(errorResponse);
                var apiResponse = JsonConvert.DeserializeObject<T>(response);

                return apiResponse;
            }
        }

        private async Task<HttpResponseMessage> SendWithResfreshToken(
            HttpClient httpClient,
            Func<HttpRequestMessage> requestMessage,
            bool withBearer = true)
        {
            if(!withBearer)
            {
                return await httpClient.SendAsync(requestMessage());
            }
            else
            {
                TokenDTO tokenDTO = _tokenProvider.GetToken();
                if (tokenDTO != null && !string.IsNullOrEmpty(tokenDTO.AccessToken))
                    httpClient.DefaultRequestHeaders.Authorization = new("Bearer", tokenDTO.AccessToken);

                try
                {
                    var response = await httpClient.SendAsync(requestMessage());

                    if (response.IsSuccessStatusCode)
                        return response;

                    //if this fail then we can pass refresh token
                    if(!response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        //generate new token from refresh token / sign in with new token then retry
                        await InvokeRefreshTokenEndpoint(httpClient, tokenDTO.AccessToken, tokenDTO.RefreshToken);
                        return await httpClient.SendAsync(requestMessage());
                    }

                    return response;
                }
                catch (AuthException)
                {
                    throw;
                }
                catch (HttpRequestException httpRequestException)
                {
                    if (httpRequestException.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        //refresh token and retry the request
                        await InvokeRefreshTokenEndpoint(httpClient, tokenDTO.AccessToken, tokenDTO.RefreshToken);
                        return await httpClient.SendAsync(requestMessage());
                    }
                    throw;
                }
            }
        }

        private async Task InvokeRefreshTokenEndpoint(HttpClient httpClient, string existingAccessToken, string existingRefreshToken)
        {
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new($"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/UserAuthAPI/Refresh");
            message.Method = HttpMethod.Post;
            message.Content = new StringContent(JsonConvert.SerializeObject(
                new TokenDTO
                {
                    AccessToken = existingAccessToken,
                    RefreshToken = existingRefreshToken
                }), Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(message);
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<APIResponse>(content);

            if(apiResponse?.IsSuccess != true)
            {
                await _httpContextAccessor.HttpContext.SignOutAsync();
                _tokenProvider.ClearToken();
                throw new AuthException();
            }
            else
            {
                var tokenDataString = JsonConvert.SerializeObject(apiResponse.Result);
                var tokenDTO = JsonConvert.DeserializeObject<TokenDTO>(tokenDataString);

                if(tokenDTO != null && !string.IsNullOrEmpty(tokenDTO.AccessToken))
                {
                    //new method to sign in with the new token that we receive
                    await SignInWithNewTokens(tokenDTO);
                    httpClient.DefaultRequestHeaders.Authorization = new("Bearer", tokenDTO.AccessToken);
                }
            }
        }

        private async Task SignInWithNewTokens(TokenDTO tokenDTO)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(tokenDTO.AccessToken);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(x => x.Type == "unique_name").Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(x => x.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            _tokenProvider.SetToken(tokenDTO);
        }

        private APIResponse Response(HttpStatusCode statusCode,
                                     bool isSuccess = true,
                                     List<string> ErrorMessages = null,
                                     object Result = null)
        {
            var response = new APIResponse();
            response.IsSuccess = isSuccess;
            response.StatusCode = statusCode;
            response.ErrorMessages = ErrorMessages;
            response.Result = Result;

            return response;
        }
    }
}
