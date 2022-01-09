using System;

namespace SaCodeWhite.Shared.Models
{
    public interface IAlertable
    {
        public int PatientTotal { get; }
        public int Capacity { get; }
    }
}
