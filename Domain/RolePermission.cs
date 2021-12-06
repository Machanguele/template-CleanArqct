namespace Domain
{
    public class RolePermission
    {
        public int Id { get; set; }
        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }
    }
}