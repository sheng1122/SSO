using DA.DataAccesses.Common;
using DA.Models;

namespace DA.DataAccesses
{
    public class AuditDA : BaseDA
    {
        public AuditDA(string connstr) : base(connstr)
        {
        }

        public int CreateAudit(Audit audit)
        {
            int rowAffected = 0;

            rowAffected = ExecuteNonQuery("usp_create_audit", audit);

            return rowAffected;
        }
    }
}
