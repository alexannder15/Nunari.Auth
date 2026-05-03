using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Nunari.Auth.Api.Controllers.Common;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SecurityJwtController : ControllerBase { }
