using Furion.DatabaseAccessor;

namespace Heavens.Core.Entities.Base;

public interface IBaseEntity<TKey> : IPrivateEntity
{
    public TKey Id { get; set; }
    public int? CreatedId { get; set; }
    public string CreatedBy { get; set; }
    public DateTimeOffset CreatedTime { get; set; }
    public int? UpdatedId { get; set; }
    public string UpdatedBy { get; set; }
    public DateTimeOffset UpdatedTime { get; set; }
    void SetCreateByHttpToken();
    void SetUpdateByHttpToken();
}

public interface IBaseEntity : IBaseEntity<int>
{
}
