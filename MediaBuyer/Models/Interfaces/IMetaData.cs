using System;
using System.Collections.Generic;

namespace MotoProfessional.Models.Interfaces
{
    public interface IMetaData
    {
        string Name { get; set; }
        string Comment { get; set; }
        string Creator { get; set; }
        List<string> Tags { get; set; }
        DateTime Captured { get; set; }
        IExtendedMetaData ExtendedData { get; set; }
    }
}