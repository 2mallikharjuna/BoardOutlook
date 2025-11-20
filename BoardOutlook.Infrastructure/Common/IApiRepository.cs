using System;

namespace BoardOutlook.Infrastructure.Core
{
    internal interface IApiRepository<T, TApiResponse>
    {
        /// <summary>
        /// HTTP Get Method
        /// </summary>
        /// <param name="identifier">Input Query Identifier</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<TApiResponse> GetAsync<TApiResponse>(string identifier, CancellationToken ct = default);
        /// <summary>
        /// HTTP Put Method
        /// </summary>
        /// <param name="model">Input model to insert</param>
        /// <returns></returns>
        Task PutAsync(T model, CancellationToken ct = default);
        /// <summary>
        /// HTTP Post Method
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Generic collection</returns>
        Task<T> PostAsync(T model, CancellationToken ct = default);
        
    }
}
