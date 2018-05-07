using System;

namespace AuctionApp.Repositories
{
    public interface ISearchable
    {
        bool Search(string searchString);
    }

    public interface ICreationAuditableEntity
    {
        string CreatedBy { get; set; }
        DateTime CreatedDate { get; set; }
    }

    public interface IAuditableEntity : ICreationAuditableEntity
    {
        string ModifiedBy { get; set; }
        DateTime? ModifiedDate { get; set; }
    }
}