using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zonosis.Mobile.Repositories
{
    public interface IRepository
    {
        Task<HttpResponseWrapper<T>> Get<T>(string urlBase, string url);
        //Task<HttpResponseWrapper<object>> Get(string url);
        Task<HttpResponseWrapper<T>> GetUserByEmail<T>(string urlBase, string url, string tokenType, string accessToken);

        Task<HttpResponseWrapper<object>> Post<T>(string urlBase, string url, T model);

        Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string urlBase, string url, T model);

        Task<HttpResponseWrapper<object>> Put<T>(string urlBase, string url, T model);

        Task<HttpResponseWrapper<TResponse>> Put<T, TResponse>(string urlBase, string url, T model);
    }
}
