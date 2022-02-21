namespace Heavens.Application._Framework.CodeGenApp.Services;

public interface ICodeGenService
{
    void GenApplication(string path);
    void GenVueApi(string path);
    void GenVuePage(string path);
}
