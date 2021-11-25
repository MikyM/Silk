﻿using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Silk.Api.Domain.Feature.Infractions;

namespace Silk.Api.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/v1/[controller]")]
	public sealed class InfractionsController : ControllerBase
	{
		private readonly IMediator _mediator;
		public InfractionsController(IMediator mediator) => _mediator = mediator;

		/// <summary> Creates an infraction for a specific user.</summary>
		/// <response code="201">The infraction was successfully created.</response>
		/// <response code="400">The provided data was invalid.</response>
		[HttpPost]
		public async Task<IActionResult> AddInfraction(AddInfraction.Request request)
		{
			var usr = User.Claims.First(c => c.Type == "ist").Value;
			
			var created = await _mediator.Send(request with {AddedBy = usr});
			
			return Created(nameof(GetInfraction), created);
		}
		
		/// <summary>Updates an infraction.</summary>
		/// <response code="200">The infraction was successfully created.</response>
		/// <response code="400">The request contained invalid data.</response>
		/// <response code="204">The infraction's information was unchanged.</response>
		/// <response code="404">The specified infraction does not exist.</response>
		[HttpPatch("{id}")]
		public async Task<IActionResult> PatchInfraction(Guid key, [FromBody] UpdateInfraction.Request request)
		{
			request.Key = key;
			
			var res = await _mediator.Send(request);

			if (res is null)
				return NotFound();
			
			if (res.Changed)
				return Ok();
			
			return NoContent();
		}
		
		
		/// <summary>
		/// Gets an infraction by it's id.
		/// </summary>
		/// <response code="300">The provided infraction exists, and was returned.</response>
		/// <response code="404">The provided infraction id points to a non-existent record.</response>
		[HttpGet("{id}")]
		public async Task<IActionResult> GetInfraction(Guid id)
		{
			var request = new GetInfraction.Request(id);
			var infraction = await _mediator.Send(request);

			if (infraction is null)
				return NotFound();

			else return Ok(infraction);
		}

		/// <summary>Gets all infractions on a specific guild.</summary>
		/// <response code="200">A guild's infractions were successfully queried</response>
		/// <response code="404">The guild was not registered with the API.</response>
		[HttpGet("guild/{guild}")]
		public async Task<IActionResult> GetGuildInfractions(ulong guild)
		{
			var req = new GetInfractionByGuild.Request(guild, User.Claims.First(c => c.Type == "ist").Value);
			var res = await _mediator.Send(req);
			
			return Ok(res);
		}
		
		
		/// <summary>Gets a user's infractions for a specific guild.</summary>
		/// <response code="200">A user's infractions were successfully queried.</response>
		[HttpGet("guild/{guild}/user/{user}")]
		public async Task<IActionResult> GetUserInfractions(ulong guild, ulong user)
		{
			var req = new GetInfractionByUser.Request(user, guild);
			var results = await _mediator.Send(req);
			return Ok(results);
		}
	}
}