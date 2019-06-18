using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedioClinic.Models
{
    public class IdentityManagerResult<TResultState>
        where TResultState : Enum
    {
        public bool Success { get; set; }
        public List<string> Errors => new List<string>();
        public TResultState ResultState { get; set; }
    }

    public class IdentityManagerResult<TResultState, TData> : IdentityManagerResult<TResultState>
        where TResultState : Enum
    {
        public TData Data { get; set; }
    }
}