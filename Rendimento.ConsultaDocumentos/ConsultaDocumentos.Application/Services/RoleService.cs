using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Application.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ConsultaDocumentos.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Result<IEnumerable<RoleDTO>>> GetAllAsync()
        {
            try
            {
                var roles = await _roleManager.Roles.ToListAsync();
                var roleDtos = roles.Select(r => new RoleDTO
                {
                    Id = r.Id,
                    Name = r.Name ?? string.Empty,
                    NormalizedName = r.NormalizedName
                }).ToList();

                return Result<IEnumerable<RoleDTO>>.SuccessResult(roleDtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<RoleDTO>>.FailureResult($"Erro ao buscar perfis: {ex.Message}");
            }
        }

        public async Task<Result<RoleDTO>> GetByIdAsync(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return Result<RoleDTO>.FailureResult("Perfil não encontrado");
                }

                var roleDto = new RoleDTO
                {
                    Id = role.Id,
                    Name = role.Name ?? string.Empty,
                    NormalizedName = role.NormalizedName
                };

                return Result<RoleDTO>.SuccessResult(roleDto);
            }
            catch (Exception ex)
            {
                return Result<RoleDTO>.FailureResult($"Erro ao buscar perfil: {ex.Message}");
            }
        }

        public async Task<Result<RoleDTO>> CreateAsync(CreateRoleDTO createRoleDto)
        {
            try
            {
                var roleExists = await _roleManager.RoleExistsAsync(createRoleDto.Name);
                if (roleExists)
                {
                    return Result<RoleDTO>.FailureResult("Já existe um perfil com este nome");
                }

                var role = new IdentityRole(createRoleDto.Name);
                var result = await _roleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return Result<RoleDTO>.FailureResult(errors);
                }

                var roleDto = new RoleDTO
                {
                    Id = role.Id,
                    Name = role.Name ?? string.Empty,
                    NormalizedName = role.NormalizedName
                };

                return Result<RoleDTO>.SuccessResult(roleDto);
            }
            catch (Exception ex)
            {
                return Result<RoleDTO>.FailureResult($"Erro ao criar perfil: {ex.Message}");
            }
        }

        public async Task<Result<RoleDTO>> UpdateAsync(UpdateRoleDTO updateRoleDto)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(updateRoleDto.Id);
                if (role == null)
                {
                    return Result<RoleDTO>.FailureResult("Perfil não encontrado");
                }

                // Verificar se o novo nome já existe (exceto para o próprio perfil)
                var existingRole = await _roleManager.FindByNameAsync(updateRoleDto.Name);
                if (existingRole != null && existingRole.Id != updateRoleDto.Id)
                {
                    return Result<RoleDTO>.FailureResult("Já existe um perfil com este nome");
                }

                role.Name = updateRoleDto.Name;
                var result = await _roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return Result<RoleDTO>.FailureResult(errors);
                }

                var roleDto = new RoleDTO
                {
                    Id = role.Id,
                    Name = role.Name ?? string.Empty,
                    NormalizedName = role.NormalizedName
                };

                return Result<RoleDTO>.SuccessResult(roleDto);
            }
            catch (Exception ex)
            {
                return Result<RoleDTO>.FailureResult($"Erro ao atualizar perfil: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteAsync(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return Result<bool>.FailureResult("Perfil não encontrado");
                }

                var result = await _roleManager.DeleteAsync(role);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return Result<bool>.FailureResult(errors);
                }

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult($"Erro ao excluir perfil: {ex.Message}");
            }
        }

        public async Task<bool> RoleExistsAsync(string name)
        {
            return await _roleManager.RoleExistsAsync(name);
        }
    }
}
