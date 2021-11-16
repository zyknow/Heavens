using Heavens.Application.CodeGenApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace Heavens.Application.CodeGenApp;

/// <summary>
/// 根据后端实体生成代码
/// </summary>
public class CodeGenAppService : IDynamicApiController
{
    public CodeGenAppService(ICodeGenService codeGenService)
    {
        _codeGenService = codeGenService;
    }

    public ICodeGenService _codeGenService { get; }
    [HttpGet]
    public void GenApplications(string path = null)
    {
        _codeGenService.GenApplication(path);
    }
    [HttpGet]

    public void GenVueApi(string path = null)
    {
        _codeGenService.GenVueApi(path);

    }
    [HttpGet]

    public void GenVuePages(string path = null)
    {
        _codeGenService.GenVuePage(path);

    }

}
