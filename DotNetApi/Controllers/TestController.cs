using System.Data;
using Dapper;
using DotnetAPI.Helpers;
using DotNetApi.Data;
using DotNetApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    public TestController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("Connection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }

    [HttpGet("Health")]
    public string ConnectionHealth()
    {
        return "Application running";
    }

}