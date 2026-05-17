namespace FundTrading.Domain.DomainObjects
{
    public abstract class Entity : IAuditableEntity
    {
        public int Id { get; protected set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public string CreatedBy { get; private set; } = string.Empty;
        public string UpdatedBy { get; private set; } = string.Empty;

        public bool IsDeleted { get; private set; }

        protected Entity()
        {
            IsDeleted = false;
        }

        public void Delete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        // 🔑 métodos usados pelo DbContext (auditoria controlada)
        public void SetCreationAudit(DateTime now, string user)
        {
            CreatedAt = now;
            UpdatedAt = now;
            CreatedBy = user;
            UpdatedBy = user;
        }

        public void SetUpdateAudit(DateTime now, string user)
        {
            UpdatedAt = now;
            UpdatedBy = user;
        }
    }
}