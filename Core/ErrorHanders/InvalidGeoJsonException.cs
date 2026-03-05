using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ErrorHanders;

public class InvalidGeoJsonException : Exception
{
    public InvalidGeoJsonException(string message)
        : base(message)
    {
    }
}