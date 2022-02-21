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
        _codeGenService.GenVueApi(Path.Combine(Environment.CurrentDirectory, "GenVueApi"));
    }

    [Fact]
    public void GenApplication_Test()
    {
        _codeGenService.GenApplication(Path.Combine(Environment.CurrentDirectory, "GenApplication"));
    }

    [Fact]
    public void GenVuePage_Test()
    {
        _codeGenService.GenVuePage(Path.Combine(Environment.CurrentDirectory, "GenVuePage"));
    }
}
