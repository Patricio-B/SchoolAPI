using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace SchoolAPI.Controllers
{
    [Route("api/v1/organizations")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class OrganizationsController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public OrganizationsController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet(Name = "getAllOrganizations"), Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetOrganizations()
        {
            var organizations = _repository.Organization.GetAllOrganizations(trackChanges: false);

            var organizationDto = _mapper.Map<IEnumerable<OrganizationDto>>(organizations);
            //uncomment the code below to test the global exception handling
            //throw new Exception("Exception");
            return Ok(organizationDto);
        }

        [HttpGet("{id}", Name = "getOrganizationById")]
        public IActionResult GetOrganization(Guid id)
        {
            var organization = _repository.Organization.GetOrganization(id, trackChanges: false); if (organization == null)
            {
                _logger.LogInfo($"Organization with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            else
            {
                var organizationDto = _mapper.Map<OrganizationDto>(organization);
                return Ok(organizationDto);
            }
        }

        [HttpPost(Name = "createOrganization")]
        public IActionResult CreateOrganization([FromBody] OrganizationForCreationDto organization)
        {
            if (organization == null)
            {
                _logger.LogError("Organization ForCreationDto object sent from client is null.");
                return BadRequest("Organization ForCreationDto object is null");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the OrganizationForCreationDto object");
                return UnprocessableEntity(ModelState);
            }

            var organizationEntity = _mapper.Map<Organization>(organization);

            _repository.Organization.CreateOrganization(organizationEntity);
            _repository.Save();

            var organizationToReturn = _mapper.Map<OrganizationDto>(organizationEntity);

            return CreatedAtRoute("getOrganizationById", new { id = organizationToReturn.Id }, organizationToReturn);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateOrganization(Guid id, [FromBody] OrganizationForUpdateDto organization)
        {
            if (organization == null)
            {
                _logger.LogError("OrganizationForUpdateDto object sent from client is null.");
                return BadRequest("OrganizationForUpdateDto object is null");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the OrganizationForUpdateDto object");
                return UnprocessableEntity(ModelState);
            }
            var organizationEntity = _repository.Organization.GetOrganization(id, trackChanges: true);
            if (organizationEntity == null)
            {
                _logger.LogInfo($"Organization with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            _mapper.Map(organization, organizationEntity);
            _repository.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrganization(Guid id)
        {
            var organization = _repository.Organization.GetOrganization(id, trackChanges: false);
            if (organization == null)
            {
                _logger.LogInfo($"Organiation with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            _repository.Organization.DeleteOrganization(organization);
            _repository.Save();

            return NoContent();
        }
    }
}
