using System;

namespace MotoProfessional.Models.Interfaces
{
    public interface ISiteMapItem
    {
        int ItemId { get; set; }
        string Title { get; set; }
        DateTime LastModified { get; set; }
        string Keywords { get; set; }
        SiteMapItemContentType ContentType { get; set; }
    }
}