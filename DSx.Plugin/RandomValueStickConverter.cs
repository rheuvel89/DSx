using DSx.Shared;
using DualSenseAPI;

namespace DSx.Plugin;

public class RandomValueStickConverter : IMappingConverter
{
    private readonly Random _random;
    private ulong _counter = 0L;

    public RandomValueStickConverter()
    {
        _random = new Random();
    }
    
    public object Convert(IDictionary<string, object> inputs, IDictionary<string, string> args, out Feedback feedback)
    {

        feedback = (_counter++ / 100) % 2 == 0
            ? new Feedback()
            : new Feedback
            {
                Rumble = new Vec2 { X = 0.1f, Y = 0.1f }

            };
        return new Vec2
        {
            X = _random.NextSingle() * 2 - 1f,
            Y = _random.NextSingle() * 2 - 1f,
        };
    }
}