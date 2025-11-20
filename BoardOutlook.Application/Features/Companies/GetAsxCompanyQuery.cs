using System;
using BoardOutlook.Application.Features.Common;

namespace BoardOutlook.Application.Features.Companies
{
    public class GetAsxCompanyQuery : IQuery
    {
        public string Company { get; }
        public GetAsxCompanyQuery(string company)
        {
            Company = company;
        }
    }
}
