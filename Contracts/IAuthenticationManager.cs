using System;
using System.Threading.Tasks;
using Entities;
using Entities.DataTransferObjects;

namespace Contracts
{
    public interface IAuthenticationManager
    {
        Task<bool> ValidateStudent(StudentForAuthenticationDto studentForAuth);
        Task<string> CreateToken();
    }
}