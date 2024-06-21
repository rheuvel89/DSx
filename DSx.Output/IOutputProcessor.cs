using DSx.Shared;

namespace DSx.Output;

public interface IOutputProcessor
{
    Feedback Map(Mapping.Mapping mapping, DualSenseInputState inputState);
    void ProcessOutput();
}