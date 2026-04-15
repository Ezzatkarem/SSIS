using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.Responce
{
   public record Responce <T> (
        T Data,
        bool IsSuccess,
        string Message
    ); 
}
