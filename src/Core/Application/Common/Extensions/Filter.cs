using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanTib.Application.Common.Extensions;
public static class BaseFilterExtensions
{
    public static Filter GetValidFilter(Filter filter)
    {
        if (string.IsNullOrEmpty(filter.Field)) throw new CustomException("The field attribute is required when declaring a filter");
        if (string.IsNullOrEmpty(filter.Operator)) throw new CustomException("The Operator attribute is required when declaring a filter");
        return filter;
    }
}
