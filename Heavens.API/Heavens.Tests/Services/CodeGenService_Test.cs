using Heavens.Application._Framework.CodeGenApp.Services;
using System;
using System.IO;
using Xunit;

namespace Heavens.Tests.Services;

public class CodeGenService_Test
{
    public CodeGenService_Test()
    {
        _codeGenService = new CodeGenService(null);
    }

    ICodeGenService _codeGenService { get; set; }
    [Fact]
    public void GenVueApi_Test()
    {
        var path = Path.Combine(Environment.CurrentDirectory, "GenVueApi");

        if(Directory.Exists(path))
            Directory.Delete(path, true);

        _codeGenService.GenVueApi(path);
    }

    [Fact]
    public void GenApplication_Test()
    {
        var path = Path.Combine(Environment.CurrentDirectory, "GenApplication");
        if (Directory.Exists(path))
            Directory.Delete(path, true);

        _codeGenService.GenApplication(path);
    }

    [Fact]
    public void GenVuePage_Test()
    {
        var path = Path.Combine(Environment.CurrentDirectory, "GenVuePage");
        if (Directory.Exists(path))
            Directory.Delete(path, true);

        _codeGenService.GenVuePage(path);
    }
}
