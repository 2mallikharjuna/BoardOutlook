using System;
using BoardOutlook.Application.Features.Common;

namespace BoardOutlook.Application.Features.Executives
{
    public class GetExecutivesQuery : IQuery
    {       
        public string CompanySymbol { get; }
        public GetExecutivesQuery(string companySymbol)
        {
            CompanySymbol = companySymbol;
        }
    }
}
