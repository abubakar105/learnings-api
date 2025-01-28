using Learnings.Application.Dtos.RolesDto;
using Learnings.Application.ResponseBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Services.Interface
{
    public interface IPermissionsService
    {
        Task<ResponseBase<PermissionsDto>> CreatePermission(PermissionsDto permissionsDto);
        Task<ResponseBase<List<PermissionsDto>>> GetAllPermissions();
        Task<ResponseBase<PermissionsDto>> GetPermissionById(int id);
        Task<ResponseBase<PermissionsDto>> UpdatePermission(PermissionsDto permissionsDto);
        Task<ResponseBase<bool>> DeletePermission(int id);
        Task<ResponseBase<List<PermissionsDto>>> GetUserPermissions(string id);
    }
}
