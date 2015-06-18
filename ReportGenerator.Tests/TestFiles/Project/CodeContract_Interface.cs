namespace ReportGenerator.Tests.TestFiles.Project
{
    using System.Diagnostics.Contracts;

    [ContractClass(typeof(CodeContract_Contract))]
    public interface CodeContract_Interface
    {
        int Calculate(int value);
    }
}
