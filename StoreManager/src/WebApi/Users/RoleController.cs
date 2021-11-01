using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Users.Interfaces;
using Core.Users.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("Create")]
        public async Task<ActionResult<RoleResponse>> CreateRole(RoleRequest request)
        {
            var role = await _roleService.CreateRoleAsync(request);
            return CreatedAtAction(nameof(GetRole), new {id = role.Id}, role);
        }

        [HttpPut]
        [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("Update")]
        public async Task<ActionResult<RoleResponse>> UpdateRole(RoleUpdatedRequest request)
        {
            var role = await _roleService.UpdateRoleAsync(request);

            return Ok(role);
        }

        [HttpGet]
        [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("Get/{id:int}")]
        public async Task<ActionResult<RoleResponse>> GetRole(int id)
        {
            var role = await _roleService.GetRoleAsync(id);

            return Ok(role);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("List")]
        public async Task<ActionResult<IEnumerable<RoleResponse>>> ListRoles()
        {
            var roles = await _roleService.GetRolesAsync();

            return Ok(roles);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("Delete/{id:int}")]
        public async Task<ActionResult> DeleteRole(int id)
        {
            await _roleService.DeleteRoleAsync(id);
            return NoContent();
        }
    }
}