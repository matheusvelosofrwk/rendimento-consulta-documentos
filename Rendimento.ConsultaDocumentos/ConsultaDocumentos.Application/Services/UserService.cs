using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Application.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ConsultaDocumentos.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Result<IEnumerable<UserDTO>>> GetAllAsync()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                var userDtos = new List<UserDTO>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userDtos.Add(new UserDTO
                    {
                        Id = user.Id,
                        Email = user.Email ?? string.Empty,
                        UserName = user.UserName,
                        EmailConfirmed = user.EmailConfirmed,
                        LockoutEnabled = user.LockoutEnabled,
                        Roles = roles.ToList()
                    });
                }

                return Result<IEnumerable<UserDTO>>.SuccessResult(userDtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<UserDTO>>.FailureResult($"Erro ao buscar usuários: {ex.Message}");
            }
        }

        public async Task<Result<UserDTO>> GetByIdAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return Result<UserDTO>.FailureResult("Usuário não encontrado");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var userDto = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    Roles = roles.ToList()
                };

                return Result<UserDTO>.SuccessResult(userDto);
            }
            catch (Exception ex)
            {
                return Result<UserDTO>.FailureResult($"Erro ao buscar usuário: {ex.Message}");
            }
        }

        public async Task<Result<UserDTO>> CreateAsync(CreateUserDTO createUserDto)
        {
            try
            {
                var userExists = await _userManager.FindByEmailAsync(createUserDto.Email);
                if (userExists != null)
                {
                    return Result<UserDTO>.FailureResult("Já existe um usuário com este email");
                }

                var user = new IdentityUser
                {
                    UserName = createUserDto.Email,
                    Email = createUserDto.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, createUserDto.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return Result<UserDTO>.FailureResult(errors);
                }

                var userDto = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    Roles = new List<string>()
                };

                return Result<UserDTO>.SuccessResult(userDto);
            }
            catch (Exception ex)
            {
                return Result<UserDTO>.FailureResult($"Erro ao criar usuário: {ex.Message}");
            }
        }

        public async Task<Result<UserDTO>> UpdateAsync(UpdateUserDTO updateUserDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(updateUserDto.Id);
                if (user == null)
                {
                    return Result<UserDTO>.FailureResult("Usuário não encontrado");
                }

                // Verificar se o novo email já existe (exceto para o próprio usuário)
                var existingUser = await _userManager.FindByEmailAsync(updateUserDto.Email);
                if (existingUser != null && existingUser.Id != updateUserDto.Id)
                {
                    return Result<UserDTO>.FailureResult("Já existe um usuário com este email");
                }

                user.Email = updateUserDto.Email;
                user.UserName = updateUserDto.Email;
                user.EmailConfirmed = updateUserDto.EmailConfirmed;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return Result<UserDTO>.FailureResult(errors);
                }

                // Atualizar senha se foi fornecida
                if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetResult = await _userManager.ResetPasswordAsync(user, token, updateUserDto.Password);

                    if (!resetResult.Succeeded)
                    {
                        var errors = resetResult.Errors.Select(e => e.Description).ToList();
                        return Result<UserDTO>.FailureResult(errors);
                    }
                }

                var roles = await _userManager.GetRolesAsync(user);
                var userDto = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    Roles = roles.ToList()
                };

                return Result<UserDTO>.SuccessResult(userDto);
            }
            catch (Exception ex)
            {
                return Result<UserDTO>.FailureResult($"Erro ao atualizar usuário: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return Result<bool>.FailureResult("Usuário não encontrado");
                }

                var result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return Result<bool>.FailureResult(errors);
                }

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult($"Erro ao excluir usuário: {ex.Message}");
            }
        }

        public async Task<Result<bool>> ManageRolesAsync(ManageUserRolesDTO manageUserRolesDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(manageUserRolesDto.UserId);
                if (user == null)
                {
                    return Result<bool>.FailureResult("Usuário não encontrado");
                }

                // Validar que todas as roles existem
                foreach (var roleName in manageUserRolesDto.RoleNames)
                {
                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        return Result<bool>.FailureResult($"Perfil '{roleName}' não existe");
                    }
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                // Remover todas as roles atuais
                var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
                if (!removeResult.Succeeded)
                {
                    var errors = removeResult.Errors.Select(e => e.Description).ToList();
                    return Result<bool>.FailureResult(errors);
                }

                // Adicionar roles selecionadas
                if (manageUserRolesDto.RoleNames.Any())
                {
                    var addResult = await _userManager.AddToRolesAsync(user, manageUserRolesDto.RoleNames);
                    if (!addResult.Succeeded)
                    {
                        var errors = addResult.Errors.Select(e => e.Description).ToList();
                        return Result<bool>.FailureResult(errors);
                    }
                }

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult($"Erro ao gerenciar perfis do usuário: {ex.Message}");
            }
        }
    }
}
